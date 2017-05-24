using Configit.BuildRadiator.Model;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace BuildRadiator.Test {
  public class TestEntities {
    [Test]
    public void TimeZoneValidatesIana() {
      var validTimezoneId = "Europe/London";
      var invalidTimezoneId = "Space/Otters";
      var ct = new ClockTile {TimeZoneId = validTimezoneId};

      Assert.That( ct.TimeZoneId.Equals( validTimezoneId ) );

      Assert.Throws<DateTimeZoneNotFoundException>(
          () => ct.TimeZoneId = invalidTimezoneId
        );
    }
  }
}