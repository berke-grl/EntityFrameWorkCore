using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class GetPersons_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            CREATE PROCEDURE [dbo].[GetAllPerson]
            AS BEGIN
            SELECT PersonID, PersonName, Email, DateOfBirth, Gender, 
            CountryID, Address, ReciveNewsLetters, Age FROM [dbo].[Persons]
            END";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
            DROP PROCEDURE [dbo].[GetAllPerson]";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
