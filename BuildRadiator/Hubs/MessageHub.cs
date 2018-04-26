using System;
using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class MessageHub: Hub<IMessageHubClient> {
    private static readonly MessageService MessageService;

    static MessageHub() {
      MessageService = new MessageService();
      MessageService.MessagesChanged += MessageServiceMessagesChanged;
    }
    
    private static void MessageServiceMessagesChanged( object sender, EventArgs e ) {
      UpdateAll();
    }

    public void Get( string messageKey ) {
      var message = MessageService.Get( messageKey );
      Clients.All.Update( message );
    }

    internal static void Update( Message message ) {
      var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub, IMessageHubClient>();
      context.Clients.All.Update( message );
    }

    internal static void UpdateAll() {
      var keys = MessageService.GetKeys();

      foreach ( var key in keys ) {
        Update( MessageService.Get( key ) );
      }
    }
  }
}