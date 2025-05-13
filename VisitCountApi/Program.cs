using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VisitCountApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name : "_myAllowSpecificOrigins" ,
        policy =>
        {
            policy.WithOrigins("https://localhost:7195") // your frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // important for cookie support
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<VisitorCountContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VisitCountDB"))
);

builder.Services.AddLogging(b => b.AddSeq("http://192.168.1.193:9020/", "Oaku4wNw79cfNHOqwkQo"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins"); // <- use it here

app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VisitorCountContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

app.Run();
