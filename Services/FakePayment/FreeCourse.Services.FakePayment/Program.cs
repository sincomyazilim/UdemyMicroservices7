using MassTransit;
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


//183 MassTransit.AspNetCore RabbitMq ayarlar� paketler yukled�n

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>  //"RabbitMQUrl": "localhost", tek� �smle ayn� ollmal� Configuration["RabbitMQUrl"]
        {
            host.Username("guest");//burdak� kullan�c� ad� ve �ifre  defult gel�yor

            host.Password("guest");
        });

    });

});
//5672 kullan�lan default port ayaga kalk�yor,onu tak�p etmek �c�n �se 15672 portu uzer�nde takpedeb��rz
//builder.Services.AddMassTransitHostedService();// bu koda 8.1 vers�nda gerekyok
//--------------------------------------------------183   




//giri� i�in

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//101 giri� yap�m�s user b�lg�s� sart� ve token alacak 

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");//sub kel�mes� d�nuste s�l�yordu kend� mapleme�snde Id yap�yordu b�zde bu kodla donusu sub b�rakd�yorzu


//96 burda authentication tan�ml�yoruz ayr� ktamanda olan proje kend� ayaga kalk�yor ve ordan dag�t�lan authento�n �le ayar ver�yrouz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerURL"];//bu k�s�mda order-appsett�ngs.josn l�nk ver�lecek.l�nk budur "IdentityServerURL": "http://localhost:5001", //identiserver ie bu m�croserv�s habdar oluyor
    opt.Audience = "resourse_fakepayment";//�dentityserver �c�ndek� conf�g dosyas�nda recourse_order ald�m ordan kontrol edecek
    opt.RequireHttpsMetadata = false; //https �stem�yoruz
});//96 basket m�croserv�ste kullan�c�l� baglant�l�rd�r

//services.AddControllers();//101 alttak� g�b� gen�slet�yoruz  koruma al�t�na al�yoruz yan� g�r�s sart� var
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));//101 buray� gen�slet�yoruz ve butun kontroller  token alamdan bagalanamz,user �art� var  requireAuthorizePolicy bunu yukarda tan�mlad�k
});






var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();//101
app.UseAuthorization();

app.MapControllers();

app.Run();
