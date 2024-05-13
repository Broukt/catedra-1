using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);
ebooks.MapGet("/?genre={genre}&author={author}&format={format}", GetAllBooks);
ebooks.MapPut("/{id}", UpdateBook);
ebooks.MapPut("/{id}/change-availability", ChangeAvailability);
ebooks.MapPut("/{id}/increment-stock", IncrementStock);
ebooks.MapPost("/purchase", BuyEbook);
ebooks.MapDelete("/{id}", DeleteEbook);

app.Run();

// TODO: Add more methods
async Task<IResult> CreateEBookAsync(DataContext context)
{
    return Results.Ok();
}
