using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using TaskManager.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskmanagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLConnection"))
);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

//builder.Services.AddDbContext<TaskmanagerContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MySQLConnection"),
//        new MySqlServerVersion(new Version(8, 0, 23))
//    )
//);

//builder.Services.AddDbContext<TaskmanagerContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MySQLConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQLConnection"))
//    ));


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
