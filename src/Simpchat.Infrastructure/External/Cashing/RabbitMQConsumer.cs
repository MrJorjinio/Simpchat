using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Simpchat.Application.Common.Models.Orders;
using Simpchat.Domain.Entities.Orders;
using Simpchat.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Simpchat.Infrastructure.External.Cashing
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration; // appsettings.json dan RabbitMQ sozlamalarini o'qish uchun
        private readonly IServiceScopeFactory _scopeFactory; // Background thread da scoped service (DbContext) yaratish uchun
        private readonly ILogger<RabbitMQConsumer> _logger; // Loglarni yozish uchun
        private IConnection? _connection; // RabbitMQ server bilan ulanish
        private IModel? _channel; // RabbitMQ channel (xabarlar shu channel orqali olinadi)
        private readonly string _queueName; // Queue nomi (masalan: "orders_queue")

        public RabbitMQConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMQConsumer> logger)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
            // appsettings.json dan queue nomini o'qish, agar yo'q bo'lsa "orders_queue" ishlatiladi
            // MUHIM: Bu nom RabbitMQProducer dagi nom bilan bir xil bo'lishi kerak!
            _queueName = _configuration["RabbitMQ:QueueName"] ?? "orders_queue";
        }

        /// <summary>
        /// BackgroundService ning asosiy metodi.
        /// Bu metod dastur ishga tushganda avtomatik ishga tushadi va
        /// dastur to'xtatilmaguncha doimiy ishlaydi.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Dastur to'liq ishga tushishi uchun 2 soniya kutamiz
            // (DbContext va boshqa servicelar tayyor bo'lishi kerak)
            await Task.Delay(2000, stoppingToken);

            // Doimiy loop - dastur to'xtatilmaguncha ishlaydi
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // RabbitMQ Consumer ni ishga tushirish
                    await StartConsumer(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Agar xatolik yuz bersa (masalan, RabbitMQ ishlamayotgan bo'lsa)
                    _logger.LogError(ex, "RabbitMQ Consumer xatolik: {Message}", ex.Message);
                    // 5 soniya kutib, qaytadan ulanishga harakat qilamiz
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// RabbitMQ Consumer ni ishga tushirish va xabarlarni qabul qilish.
        /// </summary>
        private async Task StartConsumer(CancellationToken stoppingToken)
        {
            // 1. RabbitMQ server ga ulanish uchun ConnectionFactory yaratish
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost", // RabbitMQ server manzili
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",     // Login
                Password = _configuration["RabbitMQ:Password"] ?? "guest",     // Parol
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672")    // Port (default: 5672)
            };

            // 2. RabbitMQ server bilan ulanish o'rnatish
            _connection = factory.CreateConnection();
            // 3. Channel yaratish (xabarlar shu channel orqali olinadi)
            _channel = _connection.CreateModel();

            // 4. Queue e'lon qilish (agar mavjud bo'lmasa, yaratiladi)
            _channel.QueueDeclare(
                queue: _queueName,       // Queue nomi
                durable: true,           // Queue RabbitMQ server qayta ishga tushganda ham saqlanadi
                exclusive: false,        // Boshqa connectionlar ham shu queue ga kirishi mumkin
                autoDelete: false,       // Consumer yo'q bo'lganda ham queue o'chirilmaydi
                arguments: null);        // Qo'shimcha parametrlar

            // 5. Fair dispatch - har bir consumer ga bir vaqtning o'zida faqat 1ta xabar beriladi
            // Consumer xabarni qayta ishlab bo'lguncha (BasicAck yuborguncha), yangi xabar keltirilmaydi
            // Bu load balancing uchun juda foydali (bir nechta consumer bo'lganda)
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // 6. Consumer yaratish (xabar kelganda ishga tushadigan event handler)
            var consumer = new EventingBasicConsumer(_channel);

            // 7. Xabar kelganda ishga tushadigan event handler
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    // 7.1. Xabarni byte array dan string ga aylantirish
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("Xabar qabul qilindi: {Message}", message);

                    // 7.2. JSON string ni OrderCreatedDto obyektiga aylantirish (deserializatsiya)
                    var orderDto = JsonSerializer.Deserialize<OrderCreatedDto>(message);

                    // 7.3. Scoped service (DbContext) yaratish
                    // Background thread da to'g'ridan-to'g'ri DbContext inject qilib bo'lmaydi,
                    // chunki DbContext scoped, BackgroundService esa singleton.
                    // Shuning uchun har safar yangi scope yaratishimiz kerak.
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SimpchatDbContext>();

                    if (orderDto != null)
                    {
                        // 7.4. DTO dan yangi Order entitisini yaratish
                        var newOrder = new Order
                        {
                            ProductName = orderDto.ProductName
                            // Id ni belgilamaymiz, chunki baza avtomatik generatsiya qiladi
                        };

                        _logger.LogInformation("Xabar qayta ishlanishidan oldin 5 soniya kutilmoqda...");
                        // 7.5. 5 soniya kutish (test uchun - sekin ishni simulyatsiya qilish)
                        await Task.Delay(30000);

                        // 7.6. Order ni bazaga qo'shish
                        db.Orders.Add(newOrder);
                        await db.SaveChangesAsync();

                        // SaveChanges dan keyin newOrder.Id bazadan kelgan qiymat bilan to'ldiriladi
                        _logger.LogInformation("Bazaga yozildi: {ProductName} (ID: {Id})", newOrder.ProductName, newOrder.Id);

                        // 7.7. RabbitMQ ga "xabar muvaffaqiyatli qayta ishlandi" deb xabar yuborish (Acknowledgment)
                        // Bu xabar queue dan o'chiriladi
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        // 7.8. Agar deserializatsiya muvaffaqiyatsiz bo'lsa
                        _logger.LogWarning("Order DTO deserializatsiya qilinmadi. Xabar tarkibi: {Message}", message);
                        // Xabarni rad etish va qayta queue ga qo'ymaslik (requeue: false)
                        // Chunki bu xabar noto'g'ri formatda va uni qayta ishlashning ma'nosi yo'q
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    // 7.9. Agar xatolik yuz bersa (masalan, baza bilan muammo)
                    _logger.LogError(ex, "RabbitMQ message ni qayta ishlashda xatolik. Xabar: {Message}", Encoding.UTF8.GetString(ea.Body.ToArray()));

                    // Xabarni rad etish va qayta queue ga qo'yish (requeue: true)
                    // Bu vaqtinchalik xatolar uchun foydali (masalan, baza vaqtinchalik ishlamayotgan bo'lsa)
                    // Xabar qaytadan navbatga tushadi va keyinroq qayta ishlanadi
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };


            // 8. Consumer ni queue ga bog'lash va xabarlarni kutishni boshlash
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,  // Manual acknowledgment (qo'lda tasdiqlash)
                                 // false bo'lsa, biz o'zimiz BasicAck/BasicNack ni chaqirishimiz kerak
                                 // true bo'lsa, xabar avtomatik o'chiriladi (xavfli, chunki xatolik bo'lsa xabar yo'qoladi)
                consumer: consumer);

            _logger.LogInformation("RabbitMQ Consumer ishga tushdi");

            // 9. Background service davom etishi uchun doimiy loop
            // Bu loop to'xtatilmaguncha consumer ishlab turadi
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        /// <summary>
        /// Dastur to'xtatilganda ishga tushadigan metod.
        /// RabbitMQ ulanishlarini to'g'ri yopish uchun ishlatiladi.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer to'xtatilmoqda...");
            _channel?.Close();     // Channel ni yopish
            _connection?.Close();  // Connection ni yopish
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Resurslarni tozalash (cleanup).
        /// RabbitMQ ulanishlari va channellarni dispose qilish.
        /// </summary>
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
