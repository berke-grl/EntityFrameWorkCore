using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace xUnitExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }
        [Route("/")]
        [Route("[action]")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOption sortOrder = SortOrderOption.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name"},
                { nameof(PersonResponse.Email), "Email"},
                { nameof(PersonResponse.DateOfBirth), "Date of Birth"},
                { nameof(PersonResponse.Age), "Age"},
                { nameof(PersonResponse.Gender), "Gender"},
                { nameof(PersonResponse.Country), "Country"},
                { nameof(PersonResponse.Address), "Address"},
                { nameof(PersonResponse.ReciveNewsLetters), "Recive News Letters"},
            };
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPerson(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(tmp => tmp.Errors).Select(tmp => tmp.ErrorMessage).ToList();
                return View();
            }
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //url : Persons/Edit/PersonID
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? currentPerson = _personsService.GetPersonByID(personID);

            if (currentPerson == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = currentPerson.ToPersonUpdateRequest();

            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")] //url : Persons/Edit/PersonID
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = _personsService.GetPersonByID(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(tmp => tmp.Errors).Select(tmp => tmp.ErrorMessage).ToList();
                return View();
            }
        }

        [HttpGet]
        [Route("[action]/{PersonID}")]
        public IActionResult Delete(Guid personID)
        {
            PersonResponse? person = _personsService.GetPersonByID(personID);
            if (person == null)
            {
                return RedirectToAction("Index");
            }

            return View(person);
        }

        [HttpPost]
        [Route("[action]/{PersonID}")]
        public IActionResult Delete(PersonResponse personResponse)
        {
            if(_personsService.DeletePerson(personResponse.PersonID))
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
