using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using POSBackend.Services; // ✅ So you can reference IPasswordHasher
using System;


var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configure MongoDB Client Service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    string connectionString = "mongodb+srv://MobApp:admin123@cluster0.k28aler.mongodb.net/mobapp?retryWrites=true&w=majority";
    return new MongoClient(connectionString);
});

// 2️⃣ Register Password Hasher Service
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();


// 3️⃣ Configure CORS (to allow mobile/React apps)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 4️⃣ Add MVC controllers
builder.Services.AddControllers();

// 5️⃣ Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6️⃣ Developer settings
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7️⃣ Optional: disable HTTPS redirect for easier mobile testing
// app.UseHttpsRedirection();

// 8️⃣ Enable CORS before controllers
app.UseCors("AllowAll");

// 9️⃣ Map controllers and run
app.UseAuthorization();
app.MapControllers();
app.Run();
