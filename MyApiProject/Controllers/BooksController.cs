using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly MongoDbContext _context;

    public BooksController(MongoDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<List<Book>> Get()
    {
        return await _context.GetCollection<Book>("Books").Find(_ => true).ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Book newBook)
    {
        await _context.GetCollection<Book>("Books").InsertOneAsync(newBook);
        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

}