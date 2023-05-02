using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Linq;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private PersonsDbContext _db;

        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest parameter can't be null 
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            //Validation: countryAddRequest.CountryName parameter can't be null 
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest));
            }
            //Validation: countryAddRequest.CountryName parameter can't be duplicated 
            if (_db.Countries.Count(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given CountryName already exists. CountryName can't be duplicated");
            }

            //Convert object from CountryAddRequest to Country type
            Country? country = countryAddRequest.ToCountry();

            //Generate CountryID
            country.CountryID = Guid.NewGuid();
            //Add Country object into _db list
            _db.Add(country);

            //Save Changes into db
            _db.SaveChanges();

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByID(Guid? countryID)
        {
            if (countryID == null) return null;

            Country? country = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country == null) return null;

            return country?.ToCountryResponse();
        }
    }
}