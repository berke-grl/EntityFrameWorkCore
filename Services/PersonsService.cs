using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
        }

        private PersonResponse personToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByID(person.CountryID)?.CountryName;
            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if PersonAddRequest is not null 
            if (personAddRequest == null) throw new ArgumentNullException();

            //Model Validations
            ValidationHelpers.ModelValidation(personAddRequest);

            //Convert PersonAddRequest to Person Object
            Person person = personAddRequest.ToPerson();

            person.PersonID = Guid.NewGuid();

            //_db.Persons.Add(person);
            //Save Changes into db
            //_db.SaveChanges();
            _db.sp_InsertPerson(person);

            //convert Person Object to PersonResponse type
            PersonResponse personResponse = personToPersonResponse(person);


            return personResponse;
        }

        public List<PersonResponse> GetAllPersons()
        {
            /*
            //Select * from Persons
            return _db.Persons.ToList().Select(person => personToPersonResponse(person)).ToList();
            */
            return _db.sp_GetAllPersons().Select(person => personToPersonResponse(person)).ToList(); ;
        }

        public PersonResponse? GetPersonByID(Guid? PersonID)
        {
            if (PersonID == null) return null;

            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == PersonID);

            if (person == null) return null;

            return personToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy)) return allPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(tmp =>
                    (!string.IsNullOrEmpty(tmp.PersonName) ?
                    tmp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(tmp =>
                    (!string.IsNullOrEmpty(tmp.Email) ?
                    tmp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(tmp =>
                    (tmp.DateOfBirth != null) ?
                    tmp.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(tmp =>
                    (!string.IsNullOrEmpty(tmp.Address) ?
                    tmp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(tmp =>
                    (!string.IsNullOrEmpty(tmp.Gender) ?
                    tmp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(tmp =>
                    (!string.IsNullOrEmpty(tmp.Country) ?
                    tmp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;
                default: matchingPersons = allPersons; break;
            }

            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPerson(List<PersonResponse> allPersons, string sortBy, SortOrderOption sortOrderOption)
        {
            if (string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrderOption) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOption.ASC)
                => allPersons.OrderBy(tmp => tmp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOption.DESC)
                => allPersons.OrderByDescending(tmp => tmp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOption.ASC)
                => allPersons.OrderBy(tmp => tmp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOption.DESC)
                => allPersons.OrderByDescending(tmp => tmp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOption.ASC)
                => allPersons.OrderBy(tmp => tmp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOption.DESC)
                => allPersons.OrderByDescending(tmp => tmp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOption.ASC)
                => allPersons.OrderBy(tmp => tmp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOption.DESC)
                => allPersons.OrderByDescending(tmp => tmp.Age).ToList(),

                _ => allPersons
            };

            return sortedPersons;

        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null) throw new ArgumentNullException();
            ValidationHelpers.ModelValidation(personUpdateRequest);

            Person? matchingPerson = _db.Persons.FirstOrDefault(tmp => tmp.PersonID == personUpdateRequest.PersonID);
            if (matchingPerson == null) throw new ArgumentNullException("Given PersonID dosen't exists");

            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.ReciveNewsLetters = personUpdateRequest.ReciveNewsLetters;
            matchingPerson.CountryID = personUpdateRequest.CountryID;

            //Save Update Changes into db
            _db.SaveChanges();

            return personToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException();

            Person? matchingPerson = _db.Persons.FirstOrDefault(tmp => tmp.PersonID == personID);

            if (matchingPerson == null) return false;

            _db.Persons.Remove(_db.Persons.First(tmp => tmp.PersonID == personID));

            //Save Remove Changes into db
            _db.SaveChanges();

            return true;
        }
    }
}
