using CountryData.Standard;

namespace CountryDataSample.Business
{
    internal static class CountryConstants
    {
        private static readonly CountryHelper helper = new();
        
        #region Public Properties(y)

        public static List<string> CountryNames => helper.GetCountries().ToList();
        //public static List<string> _CountryNames => countryLoader.GetCo
        public static List<string> CountryName => helper.GetCountries().ToList();

        #endregion

        #region Method(s)

        public static string GetCountryCode(string country)
        {
            if (string.IsNullOrEmpty(country) || string.IsNullOrWhiteSpace(country))
            {
                return string.Empty;
            }

            return helper.GetCountryData().FirstOrDefault(country1 => country1.CountryName == country)?.CountryShortCode ?? string.Empty;
        }

        public static List<string> GetRegions(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrWhiteSpace(countryCode))
            {
                return new List<string>();
            }

            return helper.GetRegionByCountryCode(countryCode).Select(region => region.Name).ToList();
        }

        #endregion
    }
}
