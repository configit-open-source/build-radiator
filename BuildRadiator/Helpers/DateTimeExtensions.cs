using System;

namespace Configit.BuildRadiator.Helpers {
  public static class DateTimeExtensions {
    public static DateTime ToSecondPrecision( this DateTime value ) {
      return value.AddTicks( -( value.Ticks % TimeSpan.TicksPerSecond ) );
    }
  }
}