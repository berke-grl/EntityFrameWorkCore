using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "PersonName can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be valid email address")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender can't be blank")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage ="CountryID must specify")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool? ReciveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Address = Address,
                CountryID = CountryID,
                Gender = Gender.ToString(),
                ReciveNewsLetters = ReciveNewsLetters
            };
        }
    }
}
