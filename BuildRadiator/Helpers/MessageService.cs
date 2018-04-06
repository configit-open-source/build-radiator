using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Helpers {
  internal class MessageService: IDisposable {
    private readonly string _messageFilename;
    private readonly MessageReader _messageReader;

    private IDictionary<string, Message> _messages;

    private readonly FileSystemWatcher _fileSystemWatcher;

    public event EventHandler MessagesChanged;

    public MessageService() {
      _messageFilename = HostingEnvironment.MapPath( "~/App_Data/messages.json" );
      _messageReader = new MessageReader();

      _fileSystemWatcher = new FileSystemWatcher( Path.GetDirectoryName( _messageFilename ), Path.GetFileName( _messageFilename ) );
      _fileSystemWatcher.Changed += MessageFileOnChanged;
      _fileSystemWatcher.EnableRaisingEvents = true;

      LoadMessages();
    }

    private async void MessageFileOnChanged( object sender, FileSystemEventArgs e ) {
      await Task.Delay( 100 );

      LoadMessages();

      MessagesChanged?.Invoke( this, EventArgs.Empty );
    }

    private void LoadMessages() {
      _messages = _messageReader.Deserialize( _messageFilename );
    }


    public Message Get( string messageKey ) {
      if ( _messages.ContainsKey( messageKey ) ) {
        return _messages[messageKey];
      }

      throw new ArgumentOutOfRangeException( nameof( messageKey ), "Message not found" );
    }

    public IReadOnlyCollection<string> GetKeys() {
      return _messages.Keys.ToList();
    }

    public void Dispose() {
      _fileSystemWatcher.EnableRaisingEvents = false;
      _fileSystemWatcher.Dispose();
    }
  }
}
