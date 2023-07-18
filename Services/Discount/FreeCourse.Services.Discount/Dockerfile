
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build => mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/
COPY Services/Discount/FreeCourse.Services.Discount/*.csproj Services/Discount/FreeCourse.Services.Discount/
RUN dotnet restore Services/Discount/FreeCourse.Services.Discount/*.csproj
COPY . .
RUN dotnet publish Services/Discount/FreeCourse.Services.Discount/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime => mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet","FreeCourse.Services.Discount.dll"]


