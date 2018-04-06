using System.Collections.Generic;
using System.Linq;

namespace Configit.BuildRadiator.Model {
  public class Message {
    public Message() {
    }

    public Message( string key, string content, IEnumerable<string> classes ) {
      Key = key;
      Content = content;
      Classes = classes?.ToList();
    }

    public string Key { get; set; }
    public string Content { get; set; }
    public ICollection<string> Classes { get; set; }
  }
}