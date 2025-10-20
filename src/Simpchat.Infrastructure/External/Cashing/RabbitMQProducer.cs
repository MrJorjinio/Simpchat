using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Simpchat.Application.Common.Interfaces.External.Cashing;
using Simpchat.Application.Common.Models.Orders;
using System.Text;
using System.Text.Json;
using IModel = RabbitMQ.Client.IModel;

namespace Simpchat.Infrastructure.External.Cashing;

/// <summary>
/// RabbitMQ Producer - RabbitMQ queue ga xabar yuboruvchi service.
/// Bu class Singleton sifatida ro'yxatdan o'tgan va dastur davomida bitta instance ishlaydi.
/// IDisposable interface'ini implement qiladi - ulanishlarni to'g'ri yopish uchun.
/// </summary>
public class RabbitMQProducer : IRabbitMQProducer, IDisposable
{
    private readonly IConfiguration _configuration; // appsettings.json dan sozlamalarni o'qish uchun
    private readonly ILogger<RabbitMQProducer> _logger; // Loglarni yozish uchun
    private IConnection _connection; // RabbitMQ server bilan ulanish
    private IModel _channel; // RabbitMQ channel (xabarlar shu channel orqali yuboriladi)
    private readonly string _queueName; // Queue nomi (masalan: "orders_queue")
    private readonly object _lock = new object(); // Thread-safety uchun lock obyekti (bir vaqtning o'zida faqat bitta thread EnsureConnection ni chaqirishi mumkin)

    public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
    {
        _configuration = configuration;
        _logger = logger;
        // appsettings.json dan queue nomini o'qish, agar yo'q bo'lsa "orders_queue" ishlatiladi
        // MUHIM: Bu nom RabbitMQConsumer dagi nom bilan bir xil bo'lishi kerak!
        _queueName = _configuration["RabbitMQ:QueueName"] ?? "orders_queue";
        _logger.LogInformation("RabbitMQ Producer yaratildi");
    }

    /// <summary>
    /// RabbitMQ ga ulanishni ta'minlash va tekshirish.
    /// Agar ulanish yo'q yoki yopilgan bo'lsa, qaytadan ulanish o'rnatadi.
    /// Bu metod thread-safe (bir vaqtning o'zida faqat bitta thread kirishi mumkin).
    /// </summary>
    private void EnsureConnection()
    {
        // Lock - bir vaqtning o'zida faqat bitta thread bu blokni bajarishi mumkin
        // Bu zarur, chunki bir nechta request bir vaqtda xabar yuborishi mumkin
        lock (_lock)
        {
            // Agar ulanish yo'q yoki yopilgan bo'lsa, yangi ulanish yaratish
            if (_connection == null || !_connection.IsOpen)
            {
                // Eski ulanishlarni tozalash
                _channel?.Dispose();
                _connection?.Dispose();

                try
                {
                    // 1. ConnectionFactory yaratish - RabbitMQ server sozlamalari
                    var factory = new ConnectionFactory()
                    {
                        HostName = _configuration["RabbitMQ:HostName"] ?? "localhost", // RabbitMQ server manzili
                        UserName = _configuration["RabbitMQ:UserName"] ?? "guest",     // Login
                        Password = _configuration["RabbitMQ:Password"] ?? "guest",     // Parol
                        Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),   // Port

                        // Heartbeat - RabbitMQ server bilan bog'lanishni tekshirish uchun
                        // Har 10 soniyada RabbitMQ ga "men hali tirikman" deb signal yuboriladi
                        RequestedHeartbeat = TimeSpan.FromSeconds(10),

                        // Agar ulanish uzilsa, 10 soniya kutib qaytadan ulanishga harakat qiladi
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),

                        // Avtomatik qayta ulanishni yoqish (masalan, RabbitMQ qayta ishga tushganda)
                        AutomaticRecoveryEnabled = true
                    };

                    // 2. RabbitMQ server bilan ulanish o'rnatish
                    _connection = factory.CreateConnection();

                    // 3. Channel yaratish (xabarlar shu channel orqali yuboriladi)
                    _channel = _connection.CreateModel();


                    // 4. Queue e'lon qilish yoki mavjud queue ga ulashish
                    // Agar queue mavjud bo'lmasa, uni yaratadi
                    _channel.QueueDeclare(
                        queue: _queueName,       // Queue nomi
                        durable: true,           // Queue RabbitMQ server qayta ishga tushganda ham saqlanadi
                        exclusive: false,        // Boshqa connectionlar ham shu queue ga kirishi mumkin
                        autoDelete: false,       // Producer yo'q bo'lganda ham queue o'chirilmaydi
                        arguments: null);        // Qo'shimcha parametrlar

                    _logger.LogInformation("RabbitMQ Producer muvaffaqiyatli ulandi,  Ishlatilayotgan Queue nomi: {QueueName}", _queueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ Producer ulanishda xatolik: {Message}", ex.Message);
                    throw; // Xatolikni yuqoriga uzatish (controller exception handler qilib beradi)
                }
            }
        }
    }

    /// <summary>
    /// OrderCreatedDto xabarini RabbitMQ queue ga yuborish.
    /// Bu metod Controller yoki Service layer dan chaqiriladi.
    /// </summary>
    /// <param name="message">Yuborilayotgan xabar (OrderCreatedDto)</param>
    public void SendMessage(OrderCreatedDto message)
    {
        // 1. RabbitMQ ga ulanish mavjudligini ta'minlash
        // Agar ulanish yo'q bo'lsa, EnsureConnection() uni yaratadi
        EnsureConnection();

        try
        {
            // 2. Channel holatini tekshirish
            // Agar channel yopiq yoki mavjud bo'lmasa, xatolik
            if (_channel == null || _channel.IsClosed)
            {
                _logger.LogError("RabbitMQ channel yopiq yoki mavjud emas, xabar yuborilmadi.");
                throw new InvalidOperationException("RabbitMQ channel is not open or available.");
            }

            // 3. DTO obyektini JSON stringga aylantirish (serializatsiya)
            // Masalan: { "ProductName": "Laptop" } -> JSON string
            string json = JsonSerializer.Serialize(message);

            // 4. JSON stringni byte array ga aylantirish
            // RabbitMQ xabarlarni byte array formatida yuboradi
            var body = Encoding.UTF8.GetBytes(json);

            // 5. Xabar xususiyatlarini yaratish
            var properties = _channel.CreateBasicProperties();
            // Persistent = true - xabar disk ga yoziladi, RabbitMQ qayta ishga tushganda ham saqlanadi
            // false bo'lsa, xabar faqat xotirada saqlanadi (tezroq, lekin xavfli)
            properties.Persistent = true;

            // 6. Xabarni RabbitMQ queue ga yuborish
            _channel.BasicPublish(
                exchange: "",               // Default exchange (bo'sh string)
                                            // Exchange - xabarlarni qaysi queue ga yuborishni belgilaydi
                                            // Bo'sh exchange = to'g'ridan-to'g'ri queue ga yuborish
                routingKey: _queueName,     // Queue nomi (masalan: "orders_queue")
                                            // routingKey = xabar qaysi queue ga yuborilishini belgilaydi
                basicProperties: properties, // Xabar xususiyatlari (Persistent va h.k.)
                body: body);                // Xabar tarkibi (byte array)

            _logger.LogInformation("Xabar RabbitMQ ga yuborildi: {Message}", json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ ga xabar yuborishda xatolik: {Message}", ex.Message);
            throw; // Xatolikni yuqoriga uzatish (controller qaytadan exception handling qiladi)
        }
    }


    // QOSHIMCHA MALUMOT:
    // Agar turli xil DTO'lar uchun xabar yuborish kerak bo'lsa, generik metod yaratish mumkin:
    // public void SendMessage<T>(T message) where T : class
    // {
    //     EnsureConnection();
    //     string json = JsonSerializer.Serialize(message);
    //     var body = Encoding.UTF8.GetBytes(json);
    //     _channel.BasicPublish(exchange: "", routingKey: _queueName, body: body);
    // }

    /// <summary>
    /// Dispose metodi - RabbitMQ ulanishlarini to'g'ri yopish va resurslarni tozalash.
    /// Bu metod dastur to'xtaganda yoki service dispose qilinganda avtomatik chaqiriladi.
    /// IDisposable pattern'i - .NET da resurslarni to'g'ri boshqarish uchun.
    /// </summary>
    public void Dispose()
    {
        // GC.SuppressFinalize - Finalizerni chaqirmaslik (chunki biz o'zimiz Dispose qildik)
        // Bu garbage collector uchun optimizatsiya
        GC.SuppressFinalize(this);

        try
        {
            // 1. Channel ni yopish
            // Close() - to'g'ri yopish (RabbitMQ serverga "men ketdim" deb xabar yuboradi)
            _channel?.Close();

            // 2. Connection ni yopish
            _connection?.Close();
        }
        catch (Exception ex)
        {
            // Agar yopishda xatolik bo'lsa, logga yozish
            // Lekin exception tashlamaymiz, chunki dispose xatolik bermasligi kerak
            _logger.LogError(ex, "RabbitMQ Producer dispose qilishda xatolik");
        }
        finally
        {
            // 3. Resurslarni butunlay tozalash
            // Dispose() - xotira va boshqa resurslarni bo'shatish
            // finally blokida, chunki xatolik bo'lsa ham bu bajarilishi kerak
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
/* ============================================
 * XULOSA - RabbitMQProducer qanday ishlaydi:
 * ============================================
 *
 * 1. BOSHLASH (Constructor):
 *    - Configuration va Logger injected qilinadi
 *    - Queue nomi appsettings.json dan o'qiladi
 *    - Hali RabbitMQ ga ulanish o'rnatilmaydi (lazy initialization)
 *
 * 2. XABAR YUBORISH (SendMessage):
 *    a) EnsureConnection() chaqiriladi - ulanish mavjud bo'lishini ta'minlaydi
 *    b) OrderCreatedDto -> JSON string -> byte array
 *    c) BasicPublish() orqali xabar queue ga yuboriladi
 *    d) Log yoziladi
 *
 * 3. ULANISHNI TA'MINLASH (EnsureConnection):
 *    - Lock bilan thread-safety ta'minlanadi
 *    - Agar ulanish yo'q yoki yopilgan bo'lsa:
 *      * ConnectionFactory yaratiladi
 *      * Connection va Channel ochiladi
 *      * Queue declare qilinadi
 *    - AutomaticRecoveryEnabled = true - avtomatik qayta ulanish
 *
 * 4. TOZALASH (Dispose):
 *    - Channel va Connection to'g'ri yopiladi
 *    - Barcha resurslar tozalanadi
 *
 * ============================================
 * MUHIM TUSHUNCHALAR:
 * ============================================
 *
 * - Connection: RabbitMQ server bilan TCP/IP ulanish
 * - Channel: Connection ichidagi virtual ulanish (xabarlar shu orqali yuboriladi)
 * - Queue: Xabarlar saqlanadigan navbat
 * - Exchange: Xabarlarni qaysi queue ga yuborishni belgilaydi (bo'sh = default)
 * - RoutingKey: Xabarni qaysi queue ga yuborish kerakligini ko'rsatadi
 * - Persistent: Xabar disk ga yoziladi (RabbitMQ qayta ishga tushganda ham saqlanadi)
 * - Durable: Queue disk ga yoziladi (RabbitMQ qayta ishga tushganda ham saqlanadi)
 *
 * ============================================
 * HAYOTIY MISOL (Uber Taxi):
 * ============================================
 *
 * 1. Foydalanuvchi taksi buyurtma qildi
 * 2. Controller: RabbitMQProducer.SendMessage(orderDto) chaqiradi
 * 3. Producer: Xabarni "taxi_orders" queue ga yuboradi
 * 4. Producer: Darhol javob qaytaradi (blocking yo'q!)
 * 5. Foydalanuvchi: "Buyurtmangiz qabul qilindi" xabarini oladi
 * 6. RabbitMQConsumer (background): Xabarni queue dan olib, haydovchilarga yuboradi
 *
 * Natija: Foydalanuvchi tez javob oladi, og'ir ishlar background da bajariladi!
 */