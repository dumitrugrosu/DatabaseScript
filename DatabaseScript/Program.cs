using DatabaseScript.Context;
using DatabaseScript.Models;
using DatabaseScript.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IDataMigrationService, DataMigrationService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DatabaseScript", Version = "v1" });
});

// Configure form options for file upload
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// Configure DbContext for the old database (MySQL)
builder.Services.AddDbContext<ScriptDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)), // Example: Adjust the version as needed
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Configure DbContext for the new database (SQL Server)
builder.Services.AddDbContext<AuxVesselsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection2")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatabaseScript V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<AuxVesselsContext>();
}
app.MapControllers();

app.Run();
