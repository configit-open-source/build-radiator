using System.Collections.Generic;

namespace Configit.BuildRadiator.Model {
  public class Message {
    public Message() {
    }

    public Message( string key, string content, params string[] classes ) {
      Key = key;
      Content = content;
      Classes = classes;
    }

    public string Key { get; set; }
    public string Content { get; set; }
    public ICollection<string> Classes { get; set; }
  }
}