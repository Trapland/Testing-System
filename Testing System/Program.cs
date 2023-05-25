using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Testing_System.Data;
using Testing_System.Services.Hash;
using Testing_System.Services.Kdf;
using Testing_System.Services.Random;
using Testing_System.Services.RandomImg;
using Testing_System.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<IKdfService, HashKdfService>();
builder.Services.AddSingleton<IRandomImgName, RandomImgName>();
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddSingleton<IRandomService, RandomService>();


// Add services to the container.
builder.Services.AddControllersWithViews();

String? connectionString = builder.Configuration.GetConnectionString("MySqlDb");
MySqlConnection connection = new(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));

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
