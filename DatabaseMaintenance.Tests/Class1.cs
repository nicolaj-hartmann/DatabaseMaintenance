using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DatabaseMaintenance;
using Microsoft.Data.SqlClient; // Adjust this according to your project's namespace
using Xunit;

namespace DatabaseMaintenance.Tests
{
    public class DatabaseClientTests : IAsyncLifetime
    {
        private const int SqlServerPort = 1433; // Default SQL Server port
        private const string ConnectionStringTemplate = "Server=localhost,{0};Database={1};User Id=sa;Password=YourPassword123;";
        private string _databaseName;
        private string _connectionString;

        public async Task InitializeAsync()
        {
            _databaseName = "TestDatabase_" + Guid.NewGuid().ToString("N");
            _connectionString = string.Format(ConnectionStringTemplate, SqlServerPort, _databaseName);

            await CreateDatabase();
        }

        private async Task CreateDatabase()
        {
            using (var connection = new SqlConnection($"Server=localhost,{SqlServerPort};User Id=sa;Password=YourPassword123;"))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand($"CREATE DATABASE {_databaseName};", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        [Fact]
        public async Task Initialize_ShouldRunScriptsSuccessfully()
        {
            var dbClient = new DatabaseClient(_connectionString);
            await dbClient.Initialize();

            // Add assertions to verify that the scripts were executed correctly.
            // Example: Check if a specific table was created or if data exists.
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM sys.tables", connection))
                {
                    var tableCount = (int)await command.ExecuteScalarAsync();
                    Assert.True(tableCount > 0, "No tables were created in the database.");
                }
            }
        }

        public async Task DisposeAsync()
        {
            using (var connection = new SqlConnection($"Server=localhost,{SqlServerPort};User Id=sa;Password=YourPassword123;"))
        
