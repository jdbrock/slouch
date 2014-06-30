using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    [HubName("StatusHub")]
    public class StatusHub : Hub
    {
        public void Send(String message)
        {
            Clients.All.addMessage(message);
        }
    }
}
