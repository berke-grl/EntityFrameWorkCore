using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using static System.Net.Mime.MediaTypeNames;
using System;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllersWithViews();

//Services Ioc Container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

//EntityFrameWork it's default Scoped Service
builder.Services.AddDbContext<PersonsDbContext>(options =>
{
    //connection string
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run();
