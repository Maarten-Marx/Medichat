using System.Text.Json.Serialization;
using API.Data;
using API.Repositories;
using Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MedichatContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddScoped<IRepository<Appointment>, GenericRepository<Appointment>>();
builder.Services.AddScoped<IRepository<Doctor>, GenericRepository<Doctor>>();
builder.Services.AddScoped<IRepository<Medicine>, GenericRepository<Medicine>>();
builder.Services.AddScoped<IRepository<Prescription>, GenericRepository<Prescription>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var medichatContext = scope.ServiceProvider.GetRequiredService<MedichatContext>();
    DbInitializer.Initialize(medichatContext);
}

app.Run();