using System.Diagnostics.Eventing.Reader;
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
async Task<IResult> CreateEBookAsync([FromBody] AddEbookDto ebookDto, DataContext context)
{
    if (string.IsNullOrEmpty(ebookDto.Title) || string.IsNullOrEmpty(ebookDto.Author) || string.IsNullOrEmpty(ebookDto.Genre) || string.IsNullOrEmpty(ebookDto.Format) || ebookDto.Price < 0)
        return TypedResults.BadRequest("Invalid data provided");
    var existingEbook = await context.EBooks.Where(e => e.Title == ebookDto.Title && e.Author == ebookDto.Author).FirstOrDefaultAsync();
    if (existingEbook is not null)
        return TypedResults.BadRequest("eBook already exists");
    EBook eBook = new ()
    {
        Title = ebookDto.Title,
        Author = ebookDto.Author,
        Genre = ebookDto.Genre,
        Format = ebookDto.Format,
        IsAvailable = true,
        Price = ebookDto.Price,
        Stock = 0
    };
    context.Add(eBook);
    await context.SaveChangesAsync();
    return TypedResults.Ok(eBook);
}

async Task<IResult> UpdateBook(int id, [FromBody] EditEbookDto ebookDto, DataContext context)
{
    var existingEbook = await context.EBooks.FindAsync(id);
    if (existingEbook is null)
        return TypedResults.NotFound("eBook doesn't exist");
    if (!string.IsNullOrEmpty(ebookDto.Title))
        existingEbook.Title = ebookDto.Title;
    if (!string.IsNullOrEmpty(ebookDto.Author))
        existingEbook.Author = ebookDto.Author;
    if (!string.IsNullOrEmpty(ebookDto.Genre))
        existingEbook.Genre = ebookDto.Genre;
    if (!string.IsNullOrEmpty(ebookDto.Format))
        existingEbook.Format = ebookDto.Format;
    if (ebookDto.Price >= 0)
        existingEbook.Price = ebookDto.Price;
    context.Entry(existingEbook).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return TypedResults.Ok(existingEbook);
}

async Task<IResult> ChangeAvailability(int id, DataContext context)
{
    var existingEbook = await context.EBooks.FindAsync(id);
    if (existingEbook is null)
        return TypedResults.NotFound("eBook doesn't exist");
    existingEbook.IsAvailable = !existingEbook.IsAvailable;
    context.Entry(existingEbook).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return TypedResults.Ok(existingEbook);
}

async Task<IResult> IncrementStock(int id, [FromBody] IncrementStockDto ebookDto, DataContext context)
{
    var existingEbook = await context.EBooks.FindAsync(id);
    if (existingEbook is null)
        return TypedResults.NotFound("eBook doesn't exist");
    if (ebookDto.Stock <= 0)
        return TypedResults.BadRequest("Invalid data provided");
    existingEbook.Stock += ebookDto.Stock;
    context.Entry(existingEbook).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return TypedResults.Ok(existingEbook);
}