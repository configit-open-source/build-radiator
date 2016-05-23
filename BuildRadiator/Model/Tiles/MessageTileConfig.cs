using System.Collections.Generic;

namespace Configit.BuildRadiator.Model {
  public class MessageTileConfig {
    public string MessageKey { get; set; }

    public ICollection<string> Classes { get; set; }
  }
}