using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Todo API", Version = "v1" });
});

// Add DB Context
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

// Database initialization with retry logic
var retryCount = 0;
const int maxRetries = 10;
const int retryDelaySeconds = 5;

while (retryCount < maxRetries)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
            db.Database.EnsureCreated();
            break;
        }
    }
    catch (Exception)
    {
        retryCount++;
        if (retryCount == maxRetries)
            throw;
        Console.WriteLine($"Failed to connect to database (attempt {retryCount} of {maxRetries}). Retrying in {retryDelaySeconds} seconds...");
        Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
    }
}

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync())
    .WithName("GetTodos")
    .WithOpenApi();

app.MapGet("/todos/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound())
    .WithName("GetTodoById")
    .WithOpenApi();

app.MapPost("/todos", async (TodoRequest todoRequest, TodoDb db) =>
{
    var todo = new Todo
    {
        Title = todoRequest.Title
    };
    
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo")
.WithOpenApi();

app.Run();

// Model classes
public class Todo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public record TodoRequest(string Title);

// DbContext
public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
    public DbSet<Todo> Todos => Set<Todo>();
}