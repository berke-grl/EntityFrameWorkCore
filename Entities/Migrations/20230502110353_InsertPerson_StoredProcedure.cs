using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class InsertPerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            CREATE PROCEDURE [dbo].[InsertPerson]
            (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40), @DateOfBirth datetime2(7),
            @Gender nvarchar(max), @CountryID uniqueidentifier, @Address nvarchar(200), @ReciveNewsLetters bit)
            AS BEGIN
                INSERT INTO [dbo].[Persons](PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReciveNewsLetters) VALUES
                (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReciveNewsLetters)
            END";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
