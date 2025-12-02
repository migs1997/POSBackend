using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using POSBackend.Hubs; // ✅ Add this for the SignalR hub
using POSBackend.Services; // ✅ So you can reference IPasswordHasher

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configure MongoDB Client Service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    string connectionString = "mongodb+srv://cantina:admin123@cluster0.jip0f8i.mongodb.net/";
    return new MongoClient(connectionString);
});

// 2️⃣ Register Password Hasher Service
// Register backend services
builder.Services.AddSingleton<GoogleMapsService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();



// 3️⃣ Configure CORS (to allow mobile/React apps)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // ✅ needed for SignalR
            .SetIsOriginAllowed(origin => true); // allow all origins for testing
    });
});

// 4️⃣ Add MVC controllers
builder.Services.AddControllers();

// 5️⃣ Add SignalR service
builder.Services.AddSignalR();

// 6️⃣ Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 7️⃣ Developer settings
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8️⃣ Optional: disable HTTPS redirect for easier mobile testing
// app.UseHttpsRedirection();

// 9️⃣ Enable CORS before controllers
app.UseCors("AllowAll");

app.UseAuthorization();

// 1️⃣ Map controllers
app.MapControllers();

// 2️⃣ Map SignalR hub
app.MapHub<OrderHub>("/orderHub"); // ✅ your real-time notifications hub

app.Run();
