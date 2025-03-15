using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StationnementAPI.Models;

namespace TestStationnementAPI
{
    internal class DatabaseHelper
    {
        /// <summary>
        /// Crée un contexte de base de données pour les tests en utilisant la chaîne de connexion spécifiée dans appsettings.json.
        /// </summary>
        /// <returns>Une instance de <see cref="StationnementDbContext"/> configurée pour les tests.</returns>
        public StationnementDbContext CreateContext()
        {
            DbContextOptionsBuilder<StationnementDbContext> builder = new DbContextOptionsBuilder<StationnementDbContext>();
            string connectionString =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("Test") ?? string.Empty;

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).EnableSensitiveDataLogging();

            return new StationnementDbContext(builder.Options);
        }

        /// <summary>
        /// Supprime toutes les tables de la base de données de test pour éviter les conflits entre les tests.
        /// </summary>
        /// <param name="context">Le contexte de base de données à nettoyer.</param>
        public void DropTestTables(StationnementDbContext context)
        {
            context.Database.EnsureDeleted();
        }
    }
}