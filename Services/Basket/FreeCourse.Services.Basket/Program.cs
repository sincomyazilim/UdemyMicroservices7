

using FreeCourse.Services.Basket.ConsumersRabbitmq.PublisherEvet;
using FreeCourse.Services.Basket.Services.Abstract;
using FreeCourse.Services.Basket.Services.Concrete;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services.Abstract;
using FreeCourse.Shared.Services.Concrete;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CourseNameChangedFotBasketEventConsumer>();//191  evetlar� d�nlemek �c�n ekl�yruz
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>  //"RabbitMQUrl": "localhost", tek� �smle ayn� ollmal� Configuration["RabbitMQUrl"]
        {
            host.Username("guest");//burdak� kullan�c� ad� ve �ifre  defult gel�yor

            host.Password("guest");
        });
        //-------------------------191
        cfg.ReceiveEndpoint("course-name-changed-event-basket-service", e =>// burda entpoint tan�ml�yorz k� buraya baglasn
        {
            e.ConfigureConsumer<CourseNameChangedFotBasketEventConsumer>(context);//  endPoint teki ver�ler� okuyoruz
        });

        //-----------------------------------------------------------------------------------

    });

});
//5672 kullan�lan default port ayaga kalk�yor,onu tak�p etmek �c�n �se 15672 portu uzer�nde takpedeb��rz
//builder.Services.AddMassTransitHostedService();//8 vers�yonda �ptal ed�yorzu
//--------------------------------------------------183 


//userId
builder.Services.AddHttpContextAccessor();//59 ekled�k k� shredttek� b�r s�n�fa a�t metot �dent�ye baglan�p ordak� context uer�nden user�d ulasab�ls�n

builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();// refrenas ald�g�m�z b�r projen�n s�n�flar�n� ve �nterfaceler�n� kaulalnab�lmez normalde bunlar ISharedIdentityService,SharedIdentityService shared projes�nded�r
                                                                            //59---------------------------------------------------------

builder.Services.AddScoped<IBasketService, BasketService>();//60 normal proje �c�ndek� �nterfaceler�n� ve s�n�flar�n� tan�ml�yoruz



//giri� i�in

var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//62 giri� yap�m�s user b�lg�s� sart� ve token alacak 62

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");


//62 burda authentication tan�ml�yoruz ayr� ktamanda olan proje kend� ayaga kalk�yor ve ordan dag�t�lan authento�n �le ayar ver�yrouz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerURL"];
    opt.Audience = "resourse_basket";//�dentityserver �c�ndek� conf�d dosyas�nda recourse_basket ald�m ordan kontrol edecek
    opt.RequireHttpsMetadata = false;
});//62 basket m�croserv�ste kullan�c�l� baglant�l�rd�r






//55 sett�ngs dosyas�n� buraya tan�ml�yorus appsett�ngs.jpsn RedisSettings tag�ndan  okuyacak eslet�recek
    builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));//55

builder.Services.AddSingleton<RedisService>(sp =>// 57  redisservise baglan�p onunla b�rl�te redissettings ulas�p host ve portlar�n� al�yor ve red�s.connet �le baglan�yor redis ver�tabana baglant�s�d�r
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var redis = new RedisService(redisSettings.Host, redisSettings.Port);
    redis.Connect();
    return redis;
});//57







builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));//62 buray� gen�slet�yoruz ve butun kontroller  token alamdan bagalanamz,user �art� var  requireAuthorizePolicy bunu yukarda tan�mlad�k
});









var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();































































