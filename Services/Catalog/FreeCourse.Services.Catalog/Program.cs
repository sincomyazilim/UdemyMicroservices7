using FreeCourse.Services.Catalog.Services.Abstract;
using FreeCourse.Services.Catalog.Services.Concrete;
using FreeCourse.Services.Catalog.Settings.Abstract;
using FreeCourse.Services.Catalog.Settings.Concrete;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


//190 MassTransit.AspNetCore RabbitMq ayarlar� paketler yukled�n

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
builder.Services.AddMassTransitHostedService();
//--------------------------------------------------190








//41 burda authentication tan�ml�yoruz ayr� ktamanda olan proje kend� ayaga kalk�yor ve ordan dag�t�lan authento�n �le ayar ver�yrouz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerURL"];
    opt.Audience = "resourse_catalog";//�dentityserver �c�ndek� conf�d dosyas�nda recourse_catalog ald�m ordan kontrol edecek
    opt.RequireHttpsMetadata = false;
});//41



builder.Services.AddScoped<ICategoryService, CategoryService>();//24 ekled�k
builder.Services.AddScoped<ICourseService, CourseService>();//25 ekled�k



builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());// 22 automapper ekled�k burayada bunu tan�mlad�k
                                                //services.AddControllers();//41 alttak� g�b� gen�slet�yoruz
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());//41 buray� gen�slet�yoruz ve butun kontroller  token alamdan bagalanamz,user sart� yok
});




//appsetingtek� datalar� okuma DatabaseSettings bu s�n�f uzer�nden
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));//23 eklend�
builder.Services.AddSingleton<IDatabaseSettings>(x =>
{
    return x.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});
//23 bu kod datbasebaglant� kurmak �c�n kulland�k
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();










var app = builder.Build();

using (var scope=app.Services.CreateScope())
{
    var servicesProvider = scope.ServiceProvider;
    var categoryService = servicesProvider.GetRequiredService<ICategoryService>();
    if (!(await categoryService.GetAllAsync()).Data.Any())
    {
        await   categoryService.CreateCategoryAsync(new FreeCourse.Services.Catalog.Dtos.CategorieDto.CategoryCreateDto { Name = "Asp.net Core Kursu" });
       await categoryService.CreateCategoryAsync(new FreeCourse.Services.Catalog.Dtos.CategorieDto.CategoryCreateDto { Name = "Asp.net Core Api Kursu" });
    }
}







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




























