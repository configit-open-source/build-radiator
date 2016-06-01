using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class MessageHub: Hub<IMessageHubClient> {
    private readonly MessageService _messageService;

    public MessageHub() {
      _messageService = new MessageService();
    }

    public void Get( string messageKey ) {
      var message = _messageService.Get( messageKey );
      Clients.All.Update( message );
    }

    internal static void Update( Message message ) {
      var context = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
      context.Clients.All.Update( message );
    }
  }
}