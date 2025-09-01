using LocalShop.Shared.Infrastructure.Extensions;
using LocalShop.Shared.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// Add CORS
builder.Services.AddCorsPolicy();

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map reverse proxy
app.MapReverseProxy();

app.Run();


