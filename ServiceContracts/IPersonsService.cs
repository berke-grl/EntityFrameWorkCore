using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);
        List<PersonResponse> GetAllPersons();
        PersonResponse? GetPersonByID(Guid? PersonID);
        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
        List<PersonResponse> GetSortedPerson(List<PersonResponse> allPersons, string sortBy, SortOrderOption sortOrderOption);
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        bool DeletePerson(Guid? personID);
    }
}
