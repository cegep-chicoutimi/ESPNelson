# �tape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . . 
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

# �tape 2 : Ex�cuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .
EXPOSE 8080
CMD ["dotnet", "StationnementAPI.dll"]
