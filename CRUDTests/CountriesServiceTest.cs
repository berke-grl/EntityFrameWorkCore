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
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });


        }

        //when CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });


        }

        //when CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameDuplicated()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Turkey" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Turkey" };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        //when you supply proper CountryName, it should insert(add) the Country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

            //Act
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> countris_from_GetAllCountries = await _countriesService.GetAllCountries();

            //Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countris_from_GetAllCountries);

        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
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
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //read each element from countries_list_from_add_country
            foreach (CountryResponse expexted_country in countries_list_from_add_country)
            {
                Assert.Contains(expexted_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByID
        [Fact]
        public async Task GetCountryByID_NullID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByID(countryID);

            //Assert
            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponseFromGet = await _countriesService.GetCountryByID(countryResponse.CountryID);

            //Assert
            Assert.Equal(countryResponse, countryResponseFromGet);
        }
        #endregion
    }
}
