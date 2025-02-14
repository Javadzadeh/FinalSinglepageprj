using Microsoft.EntityFrameworkCore;
using SinglePage.Sample01.ApplicationServices.Contracts;
using SinglePage.Sample01.ApplicationServices.Services;
using SinglePage.Sample01.Models;
using SinglePage.Sample01.Models.Services.Contracts;
using SinglePage.Sample01.Models.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region [- AddDbContext() -]
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ProjectDbContext>(options => options.UseSqlServer(connectionString)); 
#endregion

#region [- Models IOC -]
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
#endregion

#region [- ApplicationServices IOC -]
builder.Services.AddScoped<IPersonService, PersonService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
