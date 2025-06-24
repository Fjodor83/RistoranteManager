using Microsoft.EntityFrameworkCore;
using RistoranteManager.Data;
using RistoranteManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi questa riga per configurare il DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Aggiungi i controller e le viste
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ITableService, TableService>();

var app = builder.Build();

// Configura la pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();