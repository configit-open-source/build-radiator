using System;

namespace Configit.BuildRadiator.Helpers {
  internal class LayoutEventArgs: EventArgs {
    public LayoutEventArgs( string layoutName ) {
      LayoutName = layoutName;
    }

    public string LayoutName { get; }
  }
}