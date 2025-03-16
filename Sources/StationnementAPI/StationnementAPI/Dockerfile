# Utiliser l'image officielle de .NET SDK pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier les fichiers du projet et restaurer les dépendances
COPY . ./
RUN dotnet restore

# Compiler et publier l'application
RUN dotnet publish -c Release -o /out

# Utiliser une image plus légère pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./

# Exposer le port 8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "StationnementAPI.dll"]
