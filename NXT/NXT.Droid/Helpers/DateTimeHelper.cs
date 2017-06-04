using System;

namespace NXT.Droid
{
	public static class DateTimeHelper
	{
		public static DateTime FromUnixTime(long timeInMillis) {
			var epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddMilliseconds (timeInMillis);
		}

		public static Java.Util.Date ToUnixTime(DateTime time) {
			var epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return new Java.Util.Date ((long) time.Millisecond - epoch.Millisecond);
		}
	}
}

