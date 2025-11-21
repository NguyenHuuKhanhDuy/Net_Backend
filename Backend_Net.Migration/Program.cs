using DbUp;
using DbUp.ScriptProviders;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Backend_Net.Migration;

internal static class Program
{
    private static IConfiguration? _configuration;

    private enum DatabaseName
    {
        Catalog = 1
    }

    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        if (args.Length == 1 && IsArg(args[0], "all"))
        {
            Log.Information("Running migration for ALL databases...");

            foreach (var value in Enum.GetValues(typeof(DatabaseName)).Cast<DatabaseName>())
            {
                Log.Information($"=== Running migration for {value} ===");
                Run(value);
            }
        }
        else
        {
            foreach (var arg in args)
            {
                if (IsArg(arg, "catalog"))
                {
                    Run(DatabaseName.Catalog);
                    continue;
                }
                

                throw new ArgumentOutOfRangeException($"{arg} is not a valid database name.");
            }
        }
    }

    private static void Run(DatabaseName databaseName)
    {
        var connKey = databaseName.ToString().ToLower();
        var connectionString = _configuration.GetConnectionString(connKey);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception($"Connection string not found for: {connKey}");

        var createNewDatabase = _configuration.GetValue("CreateNewDatabase", false);
        var scriptFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Scripts/{databaseName}");

        Log.Information($"CreateNewDatabase = {createNewDatabase}");

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production" && createNewDatabase)
        {
            Log.Information($"Ensuring database exists: {databaseName}");
            EnsureDatabase.For.PostgresqlDatabase(connectionString);
        }

        var upgrader = DeployChanges.To.PostgresqlDatabase(connectionString)
            .JournalToPostgresqlTable("public", "schema_version")
            .WithScripts(new CustomScriptProvider(
                new FileSystemScriptProvider(Path.Combine(scriptFolderPath, "Sequences")),
                name => $"{databaseName}_sequence{name}"
            ))
            .WithScripts(new CustomScriptProvider(
                new FileSystemScriptProvider(Path.Combine(scriptFolderPath, "Scripts")),
                name => $"{databaseName}_script{name}"
            ))
            .WithScripts(new CustomScriptProvider(
                new FileSystemScriptProvider(Path.Combine(scriptFolderPath, "Functions")),
                name => $"{databaseName}_function{name}"
            ))
            .WithScripts(new CustomScriptProvider(
                new FileSystemScriptProvider(Path.Combine(scriptFolderPath, "Alter")),
                name => $"{databaseName}_alter{name}"
            ))
            .WithScripts(new CustomScriptProvider(
                new FileSystemScriptProvider(Path.Combine(scriptFolderPath, "Seed")),
                name => $"{databaseName}_seed{name}"
            ))

            // Default settings
            .WithTransactionPerScript()
            .WithVariablesDisabled()
            .LogScriptOutput()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Log.Error(result.Error, $"Migration FAILED for database: {databaseName}");
        }
        else
        {
            Log.Information($"Migration SUCCESS for database: {databaseName}");
        }
    }

    private static bool IsArg(string candidate, string name)
        => !string.IsNullOrWhiteSpace(name) &&
           candidate.Equals(name, StringComparison.OrdinalIgnoreCase);
}
