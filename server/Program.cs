using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using server;  
var builder = WebApplication.CreateBuilder(args);
// קריאת משתנה הסביבה או שימוש בערך ברירת המחדל מ-appsettings.json
// var connectionString = Environment.GetEnvironmentVariable("ToDoDB") 
//                        ?? builder.Configuration.GetConnectionString("ToDoDB");
//הזרקת מחקת דיבי קונטקס לשימוש מול הדאטה בייס 
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql("server=localhost;user=root;password=mstehila920;database=tododb",
//         Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql")));
//הרשאות כדי שיוכלו לתקשר עם השרת
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // כל דומיין
              .AllowAnyMethod()  // כל שיטה (GET, POST, PUT, DELETE וכו')
              .AllowAnyHeader(); // כל כותרת
    });
});

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
// Console.WriteLine();
System.Console.WriteLine($"Connection String: {connectionString}");
builder.Services.AddDbContext<ToDoDbContext>(options =>

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseCors("AllowAll");
app.UseRouting();
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }
app.MapGet("/", () => "Hello World!"); // לצורך ההרצה שורת הקוד החשובה

// שליפת כל המשימות
app.MapGet("/items", (ToDoDbContext context) =>
{
    return Results.Ok(context.Items.ToList());
});

// שליפת משימה לפי ID
app.MapGet("/items/{id}", (int id, ToDoDbContext context) =>
{
    var item = context.Items.Find(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});
//הוספה
app.MapPost("/items", ([FromBody] Item item,
                       
                       ToDoDbContext context) =>
{
    context.Items.Add(item);  // ה-id יינתן אוטומטית
    context.SaveChanges();  // ה-id יתעדכן אוטומטית במסד
    return Results.Created($"/items/{item.Id}", new { item, item.Id });
});
// עדכון משימה לפי ID

app.MapPut("/items/{id}", (int id, [FromBody] Item updatedItem, ToDoDbContext context) =>
{
    var item = context.Items.Find(id);
    if (item is null) return Results.NotFound();

    item.Name = string.IsNullOrEmpty(updatedItem.Name) ? item.Name : updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    context.SaveChanges();

    return Results.NoContent();
});
// מחיקת משימה
app.MapDelete("/items/{id}", (int id, ToDoDbContext context) =>
{
    var item = context.Items.Find(id);
    if (item is null) return Results.NotFound();

    context.Items.Remove(item);
    context.SaveChanges();
    return Results.NoContent();
});

app.Run();
