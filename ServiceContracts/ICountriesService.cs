﻿using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Counry entitiy
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a Country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>returns the Country object after adding it (including newly generated country id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list
        /// </summary>
        /// <returns>All countries from the list as List of Country</returns>
        Task<List<CountryResponse>> GetAllCountries();

        Task<CountryResponse?> GetCountryByID(Guid? countryID);
    }
}