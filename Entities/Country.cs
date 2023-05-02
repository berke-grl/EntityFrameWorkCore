using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model For Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryID { get; set; }
        [StringLength(100)]
        public string? CountryName { get; set; }
    }
}