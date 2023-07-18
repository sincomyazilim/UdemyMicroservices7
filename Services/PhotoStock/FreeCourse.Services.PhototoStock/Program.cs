using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerURL"];
    opt.Audience = "resourse_photo_stock";//�dentityserver �c�ndek� conf�d dosyas�nda photo_stock_catalog ald�m ordan kontrol edecek
    opt.RequireHttpsMetadata = false;
});//50





//services.AddControllers();//50 alttak� g�b� gen�slet�yoruz
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());//50 buray� gen�slet�yoruz ve butun kontroller  token alamdan bagalanamz,user sart� yok
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseAuthentication();//50 log�n �c�n g�s�
app.UseAuthorization();

app.MapControllers();

app.Run();
