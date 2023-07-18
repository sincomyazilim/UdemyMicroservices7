FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build => mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
WORKDIR /app
EXPOSE 80
COPY  Gateways/FreeCourse.Gateway/*.csproj  Gateways/FreeCourse.Gateway/
RUN dotnet restore Gateways/FreeCourse.Gateway/*.csproj
COPY . .
RUN dotnet publish Gateways/FreeCourse.Gateway/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime => mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet","FreeCourse.Gateway.dll"]

