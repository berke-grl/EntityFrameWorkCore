using Entities;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool? ReciveNewsLetters { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse other = (PersonResponse)obj;
            return PersonID == other.PersonID
                && PersonName == other.PersonName
                && Email == other.Email
                && DateOfBirth == other.DateOfBirth
                && Gender == other.Gender
                && CountryID == other.CountryID
                && Address == other.Address;
        }

        public override string ToString()
        {
            return $"PersonID: {PersonID}," +
                $" PersonName: {PersonName}," +
                $" Email: {Email}," +
                $" DateOfBirth: {DateOfBirth}," +
                $" Gender: {Gender}," +
                $" CountryID: {CountryID}," +
                $" Address: {Address}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Address = Address,
                CountryID = CountryID,
                ReciveNewsLetters = ReciveNewsLetters,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
            };
        }
    }
    public static class PersonExtension
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Address = person.Address,
                CountryID = person.CountryID,
                ReciveNewsLetters = person.ReciveNewsLetters,
                Gender = person.Gender,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
            };
        }
    }
}
