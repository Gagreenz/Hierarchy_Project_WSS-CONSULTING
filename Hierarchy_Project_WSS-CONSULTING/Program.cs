using Hierarchy_Project_WSS_CONSULTING.Models.DB;
using Hierarchy_Project_WSS_CONSULTING.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetSection("AppSettings:DefaultConnection").Value;
if (connection == null) throw new Exception("Connection string is null. Check appsettings.json to solve this.");
builder.Services.AddDbContext<DivisionContext>(opt => opt.UseSqlServer(connection, x => x.UseHierarchyId()));
builder.Services.AddTransient<DivisionService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Division}/{action=Index}");

app.Run();
