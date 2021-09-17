using CsvHelper;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MarketWatchQuotes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var symbol = args[0];

            var entitlement = "cecc4267a0194af89ca343805a3e57af";
            var url = $"https://api-secure.wsj.net/api/michelangelo/timeseries/history?json=%7B%22Step%22%3A%22P1D%22%2C%22TimeFrame%22%3A%22ALL%22%2C%22EntitlementToken%22%3A%22{entitlement}%22%2C%22IncludeMockTick%22%3Atrue%2C%22FilterNullSlots%22%3Afalse%2C%22FilterClosedPoints%22%3Atrue%2C%22IncludeClosedSlots%22%3Afalse%2C%22IncludeOfficialClose%22%3Atrue%2C%22InjectOpen%22%3Afalse%2C%22ShowPreMarket%22%3Afalse%2C%22ShowAfterHours%22%3Afalse%2C%22UseExtendedTimeFrame%22%3Atrue%2C%22WantPriorClose%22%3Atrue%2C%22IncludeCurrentQuotes%22%3Afalse%2C%22ResetTodaysAfterHoursPercentChange%22%3Afalse%2C%22Series%22%3A%5B%7B%22Key%22%3A%22{symbol}%22%2C%22Dialect%22%3A%22Charting%22%2C%22Kind%22%3A%22Ticker%22%2C%22SeriesId%22%3A%22ohlc%22%2C%22DataTypes%22%3A%5B%27Open%27%2C%20%27High%27%2C%20%27Low%27%2C%20%27Last%27%5D%7D%5D%7D&ckey={entitlement.Substring(0, 10)}";
            var result = await url.WithHeader("Dylan2010.EntitlementToken", entitlement).GetJsonAsync<ApiResponse>();
            var ohlcs = Zip(result.TimeInfo.Ticks, result.Series.Single().DataPoints);

            using (var writer = new StreamWriter($"{symbol}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(ohlcs.OrderBy(o => o.Date));
            }
        }

        static IEnumerable<OHLC> Zip(List<long> dates, List<List<decimal?>> dataPoints)
        {
            if (dates.Count != dataPoints.Count)
            {
                throw new Exception("Missing data");
            }

            using (var date = dates.GetEnumerator())
            using (var dp = dataPoints.GetEnumerator())
            {
                while (date.MoveNext() && dp.MoveNext())
                {
                    if (dp.Current.All(o => o == null)) continue;

                    yield return new OHLC()
                    {
                        Date = DateTimeOffset.FromUnixTimeMilliseconds(date.Current).DateTime,
                        Open = dp.Current[0],
                        High = dp.Current[1],
                        Low = dp.Current[2],
                        Close = dp.Current[3],
                    };
                }
            }
        }
    }
}
