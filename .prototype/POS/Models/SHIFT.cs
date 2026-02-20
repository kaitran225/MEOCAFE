using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class SHIFT
    {
        public static TimeSpan SHIFT1IN { get; } = TimeSpan.Parse("07:00:00");
        public static TimeSpan SHIFT2IN { get; } = TimeSpan.Parse("12:00:00");
        public static TimeSpan SHIFT3IN { get; } = TimeSpan.Parse("18:00:00");
        public static TimeSpan SHIFT4IN { get; } = TimeSpan.Parse("22:00:00");


        public static TimeSpan SHIFT1OUT { get; } = TimeSpan.Parse("12:00:00");
        public static TimeSpan SHIFT2OUT { get; } = TimeSpan.Parse("18:00:00");
        public static TimeSpan SHIFT3OUT { get; } = TimeSpan.Parse("22:00:00");
        public static TimeSpan SHIFT4OUT { get; } = TimeSpan.Parse("07:00:00");

        public static string GetCheckinStatus(TimeSpan checkin, TimeSpan checkout)
        {
            TimeSpan margin = TimeSpan.FromMinutes(30);
            if ( (checkin >= SHIFT.SHIFT1IN - margin && checkin <= SHIFT.SHIFT1IN  && checkout >= SHIFT.SHIFT1OUT )
                ||
                (checkin >= SHIFT.SHIFT2IN - margin && checkin <= SHIFT.SHIFT2IN && checkout >= SHIFT.SHIFT2OUT) ||
              (checkin >= SHIFT.SHIFT3IN - margin && checkin <= SHIFT.SHIFT3IN && checkout >= SHIFT.SHIFT3OUT) ||
                (checkin >= SHIFT.SHIFT4IN - margin && checkin <= SHIFT.SHIFT4IN && checkout >= SHIFT.SHIFT4OUT))
            {
                return "ON_TIME";
            }

            return "LATE";
        }
    }

}
