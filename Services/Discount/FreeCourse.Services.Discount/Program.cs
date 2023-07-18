using FreeCourse.Services.Discount.Services.Abstract;
using FreeCourse.Services.Discount.Services.Conrete;
using FreeCourse.Shared.Services.Abstract;
using FreeCourse.Shared.Services.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//userId �c�n
builder.Services.AddHttpContextAccessor();//74 ekled�k k� shredttek� b�r s�n�fa a�t metot �dent�ye baglan�p ordak� context uer�nden user�d ulasab�ls�n

builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();// refrenas ald�g�m�z b�r projen�n s�n�flar�n� ve �nterfaceler�n� kaulalnab�lmez normalde bunlar ISharedIdentityService,SharedIdentityService shared projes�nded�r
                                                                    //74--------------------------------------------------------


builder.Services.AddScoped<IDiscountService, DiscountService>();//75 normal proje �c�ndek� �nterfaceler�n� ve s�n�flar�n� tan�ml�yoruz



//giri� i�in

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//74 giri� yap�m�s user b�lg�s� sart� ve token alacak 74

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

//74 burda authentication tan�ml�yoruz ayr� ktamanda olan proje kend� ayaga kalk�yor ve ordan dag�t�lan authento�n �le ayar ver�yrouz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerURL"];
    opt.Audience = "resourse_discount";//�dentityserver �c�ndek� conf�d dosyas�nda recourse_basket ald�m ordan kontrol edecek
    opt.RequireHttpsMetadata = false;
});//74 basket m�croserv�ste kullan�c�l� baglant�l�rd�r

//services.AddControllers();//74 alttak� g�b� gen�slet�yoruz  koruma al�t�na al�yoruz yan� g�r�s sart� var
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));//74 buray� gen�slet�yoruz ve butun kontroller  token alamdan bagalanamz,user �art� var  requireAuthorizePolicy bunu yukarda tan�mlad�k
});









var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();//74 g�r�s �c�n k�ml�kdogrulama
app.UseAuthorization();

app.MapControllers();

app.Run();



