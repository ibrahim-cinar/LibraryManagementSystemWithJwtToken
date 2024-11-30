using LibraryManagementSystemWithJwtToken.Data;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using LibraryManagementSystemWithJwtToken.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text;
using LibraryManagementSystemWithJwtToken.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    }).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => { options.TokenValidationParameters = new TokenValidationParameters { 
    ValidateIssuer = true,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

}; 
});
builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    Options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}*/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.Run();
