using Microsoft.Data.Sqlite; // ADICIONE ESTA LINHA
using Microsoft.EntityFrameworkCore;
using SimpleCrudCSharpAPI.Data;

var builder = WebApplication.CreateBuilder(args);

var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
keepAliveConnection.Open();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(keepAliveConnection));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();