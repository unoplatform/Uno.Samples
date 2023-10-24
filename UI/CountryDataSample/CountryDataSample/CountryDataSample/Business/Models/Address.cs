namespace CountryDataSample.Business.Models
{
    /// <summary>
    /// Entity representing the Address of User or Client.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets first line of the Address record.
        /// </summary>
        public string AddressOne
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets second line of the Address record.
        /// </summary>
        public string AddressTwo
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets city/Village of the Address record. 
        /// </summary>
        public string City
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets state/County of the Address record.
        /// </summary>
        public string State
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets country of the Address record. 
        /// </summary>
        public string Country
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets postal/Zip code of the Address record. (Required).
        /// </summary>
        public string PostalCode
        {
            get; set;
        }
    }

}