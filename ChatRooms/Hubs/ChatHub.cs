using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ChatRooms.Models;

namespace ChatRooms.Hubs
{
    public class ChatHub : Hub
    {

        static List<User> Users = new List<User>();

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

       
        public void Connect(string newUserName)
        {
            var id = Context.ConnectionId;


            if (!Users.Any(x => x.ConnectionID == id))
            {
                Users.Add(new User { ConnectionID = id, Name = newUserName });

              
                Clients.Caller.onConnected(id, newUserName, Users);

                
                Clients.AllExcept(id).onNewUserConnected(id, newUserName);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }

    }
}