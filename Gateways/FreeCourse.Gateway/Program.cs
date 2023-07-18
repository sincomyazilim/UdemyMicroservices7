using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json").AddEnvironmentVariables();
//bu mettot �le configuration.development ve configuration.production ulasab�l�yoruz 106

//services.AddHttpClient<TokenExhangeDelegateHandler>();//195 hanler� tan�ml�yoruz



builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority =builder.Configuration["IdentityServerURL"];
    options.Audience = "resourse_gateway";
    options.RequireHttpsMetadata = false;
});



builder.Services.AddOcelot();//105 kutuhanem�z� tan�mlad�k
                     // services.AddOcelot().AddDelegatingHandler<TokenExhangeDelegateHandler>();195 ekl�yoruz ama b�zkullanmazyacaz



var app = builder.Build();


await app.UseOcelot();

app.UseAuthorization();
app.UseDeveloperExceptionPage();
app.MapControllers();

app.Run();







































