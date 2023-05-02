using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {

        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }
        #region addCountry
        //when CounrtyAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });


        }

        //when CountryName is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });


        }

        //when CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public void AddCountry_CountryNameDuplicated()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Turkey" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Turkey" };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        //when you supply proper CountryName, it should insert(add) the Country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            //Act
            CountryResponse countryResponse = _countriesService.AddCountry(request);
            List<CountryResponse> countris_from_GetAllCountries = _countriesService.GetAllCountries();

            //Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countris_from_GetAllCountries);

        }
        #endregion

        #region GetAllCountries
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest> {
                new CountryAddRequest() {CountryName="USA" },
                new CountryAddRequest() {CountryName="Germany" },
            };

            //Act
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //read each element from countries_list_from_add_country
            foreach (CountryResponse expexted_country in countries_list_from_add_country)
            {
                Assert.Contains(expexted_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByID
        [Fact]
        public void GetCountryByID_NullID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? countryResponse = _countriesService.GetCountryByID(countryID);

            //Assert
            Assert.Null(countryResponse);
        }

        [Fact]
        public void GetCountryByID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponseFromGet = _countriesService.GetCountryByID(countryResponse.CountryID);

            //Assert
            Assert.Equal(countryResponse, countryResponseFromGet);
        }
        #endregion
    }
}
