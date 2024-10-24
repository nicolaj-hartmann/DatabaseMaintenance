using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
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

                var scripts = LoadScripts(); // Load your complete SQL script
                foreach (var script in scripts)
                {
                    var commands = SplitSqlCommands(script);

                    foreach (var commandText in commands)
                    {
                        var trimmedCommand = commandText.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedCommand))
                        {
                            using (var command = new SqlCommand(trimmedCommand, connection))
                            {
                                try
                                {
                                    await command.ExecuteNonQueryAsync();
                                }
                                catch (SqlException ex)
                                {
                                    // Log or handle the error appropriately
                                    Console.WriteLine($"SQL Error: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task ExecuteIndexOptimizeProcedure(IndexOptions options, Action<string> output)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Subscribe to the InfoMessage event to capture messages
                connection.InfoMessage += (sender, e) =>
                {
                    foreach (SqlError error in e.Errors)
                    {
                        output(error.Message);
                    }
                };

                using (var command = new SqlCommand("dbo.IndexOptimize", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Set parameters based on the options object
                    command.Parameters.AddWithValue("@Databases", options.Databases ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FragmentationLow", options.FragmentationLow ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FragmentationMedium", options.FragmentationMedium ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FragmentationHigh", options.FragmentationHigh ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FragmentationLevel1", options.FragmentationLevel1);
                    command.Parameters.AddWithValue("@FragmentationLevel2", options.FragmentationLevel2);
                    command.Parameters.AddWithValue("@MinNumberOfPages", options.MinNumberOfPages ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaxNumberOfPages", options.MaxNumberOfPages ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SortInTempdb", options.SortInTempdb ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MaxDOP", options.MaxDOP ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FillFactor", options.FillFactor ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PadIndex", options.PadIndex ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LOBCompaction", options.LOBCompaction ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdateStatistics", options.UpdateStatistics ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OnlyModifiedStatistics", options.OnlyModifiedStatistics ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StatisticsModificationLevel", options.StatisticsModificationLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StatisticsSample", options.StatisticsSample ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StatisticsResample", options.StatisticsResample ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PartitionLevel", options.PartitionLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MSShippedObjects", options.MSShippedObjects ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Indexes", options.Indexes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TimeLimit", options.TimeLimit ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Delay", options.Delay ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@WaitAtLowPriorityMaxDuration", options.WaitAtLowPriorityMaxDuration ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@WaitAtLowPriorityAbortAfterWait", options.WaitAtLowPriorityAbortAfterWait ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Resumable", options.Resumable ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AvailabilityGroups", options.AvailabilityGroups ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LockTimeout", options.LockTimeout ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LockMessageSeverity", options.LockMessageSeverity);
                    command.Parameters.AddWithValue("@StringDelimiter", options.StringDelimiter ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DatabaseOrder", options.DatabaseOrder ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DatabasesInParallel", options.DatabasesInParallel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ExecuteAsUser", options.ExecuteAsUser ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@LogToTable", options.LogToTable ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Execute", options.Execute ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        private IEnumerable<string> SplitSqlCommands(string script)
        {
            var commands = new List<string>();
            var lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var commandBuilder = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (commandBuilder.Length > 0)
                    {
                        commands.Add(commandBuilder.ToString());
                        commandBuilder.Clear();
                    }
                }
                else
                {
                    commandBuilder.AppendLine(line);
                }
            }

            // Add any remaining command
            if (commandBuilder.Length > 0)
            {
                commands.Add(commandBuilder.ToString());
            }

            return commands;
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