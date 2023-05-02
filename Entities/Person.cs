using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Person
    {
        //User shouldn't declare Guid

        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)] //nvarchar(40)
        public string? PersonName { get; set; }
        [StringLength(40)] //nvarchar(40)
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)] //nvarchar(8)
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public bool? ReciveNewsLetters { get; set; }
        public double? Age { get; set; }
    }
}
