using BAL.Interface;
using DAL.DataContext;
using BAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogin, LoginRepo>();
builder.Services.AddScoped<ICreateAcc,  CreateAccRepo>();
builder.Services.AddScoped<IPatientRequest, PatientRequestRepo>();
builder.Services.AddScoped<IRequests, OtherRequestRepo>();   
builder.Services.AddScoped<IuploadFile, UploadFileRepo>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IJWTTokenservice, JWTTokenservicerepo>();


// Add DbContext 
builder.Services.AddDbContext<ApplicationDbContext>();

// JWT Configuration

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=PatientSite}/{id?}");

app.Run();
