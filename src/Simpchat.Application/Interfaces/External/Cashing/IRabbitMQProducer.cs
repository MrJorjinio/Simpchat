using Simpchat.Application.Models.Orders;
using SimpchatWeb.Services.Db.Contexts.Default.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpchat.Application.Interfaces.External.Cashing
{
    public interface IRabbitMQProducer
    {
        void SendMessage(OrderCreatedDto message);
    }
}
