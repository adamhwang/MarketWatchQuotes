using System.Collections.Generic;

namespace MarketWatchQuotes
{
    public class ApiResponseTimeInfo
    {
        public List<long> Ticks { get; set; }
    }

    public class ApiResponseSeries
    {
        public string SeriesId { get; set; }
        public string ResponseId { get; set; }
        public List<string> DesiredDataPoints { get; set; }
        public string Ticker { get; set; }
        public string CountryCode { get; set; }
        public string CommonName { get; set; }
        public string OfficialId { get; set; }
        public int UtcOffset { get; set; }
        public string TimeZoneAbbreviation { get; set; }
        public List<List<decimal?>> DataPoints { get; set; }
        public string InstrumentType { get; set; }
        public string DjId { get; set; }
    }

    public class ApiResponse
    {
        public ApiResponseTimeInfo TimeInfo { get; set; }
        public List<ApiResponseSeries> Series { get; set; }
    }
}
