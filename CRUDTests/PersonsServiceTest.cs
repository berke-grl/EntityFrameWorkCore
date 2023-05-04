using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

            _personsService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            _testOutputHelper = testOutputHelper;
        }
        #region AddPerson
        [Fact]
        public async Task AppPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
                await _personsService.AddPerson(personAddRequest);
            });

        }
        [Fact]
        public async Task AppPerson_NullPersonName()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _personsService.AddPerson(personAddRequest);
            });

        }
        [Fact]
        public async Task AppPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Berke",
                Address = "dummy address",
                CountryID = Guid.NewGuid(),
                Email = "dummy@hotmail.com",
                DateOfBirth = Convert.ToDateTime("2000-01-01"),
                ReciveNewsLetters = true,
                Gender = GenderOptions.Male,
            };

            //Act
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
            List<PersonResponse> personResponsesList = await _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, personResponsesList);
        }
        #endregion

        #region GetPersonByID
        [Fact]
        public async Task GetPersonByID_nullPersonID()
        {
            Guid? personID = null;

            PersonResponse? personResponse = await _personsService.GetPersonByID(personID);

            Assert.Null(personResponse);
        }
        [Fact]
        public async Task GetPersonByID_validPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            //Act
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Dummy Person",
                Address = "Dummy Address",
                CountryID = countryResponse.CountryID,
                Email = "dummy@hotmail.com",
                ReciveNewsLetters = true,
                Gender = GenderOptions.Male,
                DateOfBirth = Convert.ToDateTime("2000-01-01")
            };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
            PersonResponse? getPersonfromResponse = await _personsService.GetPersonByID(personResponse.PersonID);

            //Assert
            Assert.Equal(personResponse, getPersonfromResponse);

        }
        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> personResponses = await _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personResponses);
        }
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Act
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "England" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Brazil" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Berke",
                Email = "berke@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Isparta",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true,
                DateOfBirth = Convert.ToDateTime("2000-05-06")
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Gizem",
                Email = "gizem@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Antalya",
                Gender = GenderOptions.Female,
                ReciveNewsLetters = false,
                DateOfBirth = Convert.ToDateTime("2002-05-06")
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(personResponse);
            }

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }

        }

        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptyText()
        {
            //Act
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "England" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Brazil" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Zeynep",
                Email = "zeynep@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Adana",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true,
                DateOfBirth = Convert.ToDateTime("2000-05-06")
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Gizem",
                Email = "gizem@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Antalya",
                Gender = GenderOptions.Female,
                ReciveNewsLetters = false,
                DateOfBirth = Convert.ToDateTime("2002-05-06")
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(personResponse);
            }

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }

        }
        [Fact]
        public async Task GetFilteredPersons_SearchedText()
        {
            //Act
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "England" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Brazil" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Zeynep",
                Email = "zeynep@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Adana",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true,
                DateOfBirth = Convert.ToDateTime("2000-05-06")
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Gizem",
                Email = "gizem@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Antalya",
                Gender = GenderOptions.Female,
                ReciveNewsLetters = false,
                DateOfBirth = Convert.ToDateTime("2002-05-06")
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(personResponse);
            }

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ze");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null
                    && person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }
            }

        }
        #endregion

        #region GetSortedPerson
        [Fact]
        public async Task GetSortedPerson()
        {
            //Act
            CountryAddRequest countryAddRequest1 = new CountryAddRequest { CountryName = "England" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest { CountryName = "Brazil" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest
            {
                PersonName = "Zeynep",
                Email = "zeynep@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Adana",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true,
                DateOfBirth = Convert.ToDateTime("2000-05-06")
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest
            {
                PersonName = "Gizem",
                Email = "gizem@hotmail.com",
                CountryID = countryResponse1.CountryID,
                Address = "Antalya",
                Gender = GenderOptions.Female,
                ReciveNewsLetters = false,
                DateOfBirth = Convert.ToDateTime("2002-05-06")
            };
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(personResponse);
            }

            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPerson(allPersons, nameof(Person.PersonName), SortOrderOption.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse personResponse in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            }

        }
        #endregion

        #region PersonUpdate
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {    //Act
                await _personsService.UpdatePerson(personUpdateRequest);

            });
        }
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>( async() =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);

            });
        }

        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            //Act
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "TR" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Berke",
                Address = "Edirne",
                Email = "berke@hotmail.com",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
            };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);


            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();
            personUpdate.PersonName = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>( async() =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdate);
            });
        }
        [Fact]
        public async Task UpdatePerson_PersonWithFullDetails()
        {
            //Act
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "TR" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Berke",
                Address = "Edirne",
                CountryID = countryResponse.CountryID,
                Email = "berke@hotmail.com",
                Gender = GenderOptions.Male,
                ReciveNewsLetters = true,
                DateOfBirth = Convert.ToDateTime("2000-01-05")
            };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);


            PersonUpdateRequest personUpdate = personResponse.ToPersonUpdateRequest();
            personUpdate.PersonName = "William";
            personUpdate.Email = "william@hotmail.com";

            //Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(personUpdate);

            PersonResponse? person_response_from_get = await _personsService.GetPersonByID(personUpdate.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

         #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Berke",
                Address = "Edirne",
                Email = "berke@hotmail.com",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
            };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personsService.DeletePerson(personResponse.PersonID);

            //Assert
            Assert.True(isDeleted);
        }
        [Fact]
        public async Task DeletePerson_InValidID()
        {
            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());
            //Assert
            Assert.False(isDeleted);
        }
        #endregion
    }
}



