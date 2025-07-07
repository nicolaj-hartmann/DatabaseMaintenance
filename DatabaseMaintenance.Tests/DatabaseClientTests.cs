using System.Text;
using Microsoft.Data.SqlClient;
using Xunit;
// ReSharper disable AccessToDisposedClosure

namespace DatabaseMaintenance.Tests
{
    public class DatabaseClientTests : IAsyncLifetime
    {
        private const int SqlServerPort = 1433; // Default SQL Server port
        private readonly string _connectionString = $"Server=localhost,{SqlServerPort};User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";
        private readonly string _databaseName = $"TestDatabase_{Guid.NewGuid():N}";

        public async Task InitializeAsync()
        {
            await CreateDatabase();
        }

        private async Task CreateDatabase()
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand($"CREATE DATABASE {_databaseName};", connection);
            await command.ExecuteNonQueryAsync();
        }

        [Theory]
        [InlineData("CommandExecute")]
        [InlineData("IndexOptimize")]
        public async Task Initialize_ShouldRunScriptsSuccessfully(string storedProcName)
        {
            var dbClient = new DatabaseClient(GetTestConnectionString());
            await dbClient.Initialize();

            await using var connection = new SqlConnection(GetTestConnectionString());
            await connection.OpenAsync();

            await using var command = new SqlCommand($"SELECT COUNT(*) FROM sys.objects WHERE name = '{storedProcName}' AND type IN (N'P', N'PC')", connection);
            var procExists = await command.ExecuteScalarAsync() as int?;

            Assert.True(procExists > 0, $"The stored procedure '{storedProcName}' was not created in the database.");
        }

        [Fact]
        public async Task Run_IndexOptimize()
        {
            var dbClient = new DatabaseClient(GetTestConnectionString());
            await dbClient.Initialize();

            var options = new IndexOptions
            {
                FragmentationLevel2 = 50,
                Databases = _databaseName,
                LogToTable = "Y"
            };
            var stringBuilder = new StringBuilder();

            await dbClient.ExecuteIndexOptimizeProcedure(options, message => stringBuilder.AppendLine(message));

            Assert.Contains("@FragmentationLevel2 = 50", stringBuilder.ToString());
            Assert.Contains(_databaseName, stringBuilder.ToString());
        }

        [Fact]
        public async Task Run_IndexOptimize_On_NonExisting_Database()
        {
            var dbClient = new DatabaseClient(GetTestConnectionString());
            await dbClient.Initialize();

            var options = new IndexOptions
            {
                FragmentationLevel2 = 50,
                Databases = "unknown_database"
            };
            var stringBuilder = new StringBuilder();

            await dbClient.ExecuteIndexOptimizeProcedure(options, message => stringBuilder.AppendLine(message));

            Assert.Contains("The following databases in the @Databases parameter do not exist: [unknown_database]", stringBuilder.ToString());
        }


        private string GetTestConnectionString()
        {
            return $"Server=localhost,{SqlServerPort};Database={_databaseName};User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";
        }

        public async Task DisposeAsync()
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using (var command = new SqlCommand($"ALTER DATABASE {_databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await using var command = new SqlCommand($"DROP DATABASE {_databaseName};", connection);
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException)
                {
                    // Ignore all exceptions
                }
            });
        }
    }
}