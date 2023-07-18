using FreeCourse.Web.Handler;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Abstract;
using FreeCourse.Web.Services.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System;
using FreeCourse.Web.Extensions;
using FluentValidation.AspNetCore;
using FreeCourse.Web.Validator;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<ServiceApiSettings>(builder.Configuration.GetSection("ServiceApiSettings"));//115 burda appsett�ng dosyas�ndan ver� okuyoruz ServiceApiSettings bu tag alt�ndak�nden. oda model klasorunde ServiceApiSettings s�n�f�dn 
builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));//116 burda appsett�ng dosyas�ndan ver� okuyoruz ClientSettings bu tag alt�ndak�nden. oda model klasorunde ClientSettings s�n�f�dn 

builder.Services.AddHttpContextAccessor();//119 UserId ulasmak �c�ndr� shred katman�ndan
builder.Services.AddAccessTokenManagement();//137 IClientAccessTokenCache bunun �c�n ekled�k tan�s�n d�ye paket yuklem�st�k
builder.Services.AddSingleton<PhotoHelper>();//bu helper photostoc m�croservsten fogrof yolunu urls�n� al�yor 146
builder.Services.AddHttpClient<IIdentityService, IdentityService>();//119--services.AddHttpClien �le ekmek sebeb�m�z Identitservice _httpClient dondurgumuz ��nd�r yoksa AddScoped olarak ekleyecekt�k                 


builder.Services.AddScoped<ResourceOwnerPasswordTokenHandler>();//123 ekleme unutuldu-126 ekled�n butun handler serv�s olarak eklen�yr
builder.Services.AddScoped<ClientCredentialTokenHandler>();//136 ekleme unutuldu-126 ekled�n butun handler serv�s olarak eklen�yr

//154 ServiceExtension yazd�k b�r k�s�m� oraya ald�kk� buras� kalaba�koldu baya
builder.Services.AddHttpClientServices(builder.Configuration);//154 exta�on klasoru ServiceExtension s�n�f�









////120 cookie yetk�lend�rme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
{
    opt.LoginPath = "/Auth/signIn";
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);//60 gun klacak
    opt.SlidingExpiration = true;//60 gun doldukca 60 gun daha uzas�nm� evet uzas�n yap�yoruz
    opt.Cookie.Name = "sincomwebcookiemicroservice";
});//120 cook� tan�tma




builder.Services.AddFluentValidationAutoValidation();

//services.AddControllersWithViews();//159 bunu gen�slet�ryoruz
builder.Services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCourseInputValidator>());//159







var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseAuthentication();//120 eklendi
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
