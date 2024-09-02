using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "IdentityCookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        //options.Events = new CookieAuthenticationEvents
        //{
        //    OnRedirectToLogin = context =>
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        return Task.CompletedTask;
        //    }
        //};
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddApplicationPart(Presentation.AssemblyRefernece.Assembly);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    try
    {
        // Apply migrations
        app.Services.ApplyMigrations();
    }
    catch (Exception ex)
    {
        // Log errors or handle them as needed
        Console.WriteLine("An error occurred while migrating the database: " + ex.Message);
    }
}

app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
