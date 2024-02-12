using BAL.Interface;
using DAL.DataContext;
using BAL.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogin, LoginRepo>();
builder.Services.AddScoped<ICreateAcc,  CreateAccRepo>();
builder.Services.AddScoped<IPatientRequest, PatientRequestRepo>();
builder.Services.AddScoped<IRequests, OtherRequestRepo>();   

// Add DbContext 
builder.Services.AddDbContext<ApplicationDbContext>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=PatientSite}/{id?}");

app.Run();
