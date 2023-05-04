using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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


        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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
            await _db.SaveChangesAsync();
            //_db.sp_InsertPerson(person);

            //convert Person Object to PersonResponse type
            PersonResponse personResponse = person.ToPersonResponse();

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var person = await _db.Persons.Include("Country").ToListAsync();
            //Select * from Persons
            return person.Select(tmp => tmp.ToPersonResponse()).ToList();

            /*
            return _db.sp_GetAllPersons().Select(person => person.ToPersonResponse()).ToList();
            */
        }

        public async Task<PersonResponse?> GetPersonByID(Guid? PersonID)
        {
            if (PersonID == null) return null;

            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == PersonID);

            if (person == null) return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
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

        public async Task<List<PersonResponse>> GetSortedPerson(List<PersonResponse> allPersons, string sortBy, SortOrderOption sortOrderOption)
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
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
            await _db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException();

            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(tmp => tmp.PersonID == personID);

            if (matchingPerson == null) return false;

            _db.Persons.Remove(_db.Persons.First(tmp => tmp.PersonID == personID));

            //Save Remove Changes into db
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            //csvWriter.WriteHeader<PersonResponse>();//PersonID, PersonName, ...
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReciveNewsLetters));
            csvWriter.NextRecord();

            List<PersonResponse> personResponses = _db.Persons.Include("Country").Select(tmp => tmp.ToPersonResponse()).ToList();

            foreach (PersonResponse person in personResponses)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth != null)
                {
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                }
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReciveNewsLetters);

                //added data to memory stream
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            //await csvWriter.WriteRecordsAsync(personResponses);

            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["A1"].Value = "Email";
                worksheet.Cells["A1"].Value = "Date Of Birth";
                worksheet.Cells["A1"].Value = "Age";
                worksheet.Cells["A1"].Value = "Gender";
                worksheet.Cells["A1"].Value = "Country";
                worksheet.Cells["A1"].Value = "Address";
                worksheet.Cells["A1"].Value = "Recive News Letter";

                using(ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }

                int row = 2;

                List<PersonResponse> persons = _db.Persons.Include("Country").Select(tmp => tmp.ToPersonResponse()).ToList();

                foreach (PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;

                    if (person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    else
                        worksheet.Cells[row, 3].Value = person.DateOfBirth;

                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.Country;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReciveNewsLetters;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
