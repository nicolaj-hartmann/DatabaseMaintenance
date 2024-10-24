using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DatabaseMaintenance
{
    public class DatabaseClient
    {
        private readonly string _connectionString;

        public DatabaseClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Initialize()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var scripts = LoadScripts();
                foreach (var script in scripts)
                {
                    using (var command = new SqlCommand(script, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        private static string[] LoadScripts()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var scripts = new List<string>();

            foreach (var resourceName in resourceNames)
            {
                // Filter the resource names to match your embedded scripts
                if (resourceName.EndsWith("CommandExecute.sql") || resourceName.EndsWith("IndexOptimize.sql"))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    using (var reader = new StreamReader(stream))
                    {
                        scripts.Add(reader.ReadToEnd());
                    }
                }
            }

            return scripts.ToArray();
        }
    }
}