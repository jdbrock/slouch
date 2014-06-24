using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    [HubName("hellohub")]
    public class MyHub : Hub
    {
        public void Send(String message)
        {
            Clients.All.addMessage(message);
        }
    }
}
