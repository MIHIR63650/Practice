using Microsoft.AspNetCore.Mvc;
using MyApiProject.Data;
using MyApiProject.Models;
using MongoDB.Driver;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ItemsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var items = await _context.Items.Find(_ => true).ToListAsync();
        return Ok(items);
    }

    // Add more CRUD operations here as needed
}