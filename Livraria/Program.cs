using Microsoft.AspNetCore.Authentication.Cookies;
using Livraria.Models;
using Microsoft.AspNetCore.Identity;
using Livraria.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPasswordHasher<Login>, PasswordHasher<Login>>();

builder.Services.AddScoped<ILoginRepository, LoginRepository>();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
    });

//Controla o acesso as páginas
builder.Services.AddAuthorization();

//Permite gerar e verificar hashes de senhas
builder.Services.AddScoped<
    IPasswordHasher<Login>, PasswordHasher<Login>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

//Verifica se o usuário está autenticado
app.UseAuthentication();

//Verifica se tem permissão para acessar a página
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();



