using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace GameAssets.Scripts.Service.Time
{
    public static class WorldTime
    {
        public static async Task<DateTime> GetAsync()
        {
            await Task.Run(() =>
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
                WebResponse response = myHttpWebRequest.GetResponseAsync().Result;

                string todaysDates = response.Headers["date"];
                return DateTime.ParseExact(todaysDates,
                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal);
            });

            return GetLocal();
        }

        private static DateTime GetLocal()
            => DateTime.Now;
    }
}
