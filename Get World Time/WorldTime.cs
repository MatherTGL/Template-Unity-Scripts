using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAssets.Scripts.Service.Time
{
    public static class WorldTime
    {
        private static DateTime? cachedTime;

        private static DateTime lastFetchTime;

        private static TimeSpan cacheDuration = TimeSpan.FromSeconds(30);


        public static async Task<DateTime> GetAsync()
        {
            if (cachedTime.HasValue && DateTime.Now - lastFetchTime < cacheDuration)
                return cachedTime.Value;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response = await client.GetAsync("http://www.microsoft.com");
                    response.EnsureSuccessStatusCode();

                    IEnumerable<string> values;
                    if (response.Headers.TryGetValues("date", out values))
                    {
                        string todaysDate = values.First();

                        DateTime extract = DateTime.ParseExact(todaysDate,
                                                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                                    CultureInfo.InvariantCulture.DateTimeFormat,
                                                    DateTimeStyles.AssumeUniversal);

                        lastFetchTime = extract;
                        cachedTime = lastFetchTime;

                        return extract;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return GetLocal();
        }

        private static DateTime GetLocal()
            => DateTime.Now;
    }
}
