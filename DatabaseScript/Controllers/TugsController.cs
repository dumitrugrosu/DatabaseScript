using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TugsController : ControllerBase
    {
        private readonly string connectionString = "Server=localhost;Database=aux_db;Uid=root;Pwd=Panatha4ever;";

        [HttpGet("UpdateTugs")]
        public IActionResult ProcessTugs()
        {
            string excelFilePath = "aux_tugs.xlsx";
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            try
            {
                // Read Excel file and populate data dictionary
                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    var rows = worksheet.RowsUsed();
                    foreach (var row in rows)
                    {
                        string primary = row.Cell(1).Value.ToString();
                        string fakesString = row.Cell(2).Value.ToString();
                        List<string> fakes = new List<string>(fakesString.Split(','));
                        data.Add(primary, fakes);
                    }
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        foreach (var kvp in data)
                        {
                            string primary = kvp.Key;
                            List<string> fakes = kvp.Value;

                            command.Parameters.Clear();
                            command.CommandText = "SELECT id_tug, name_tug FROM aux_tugs WHERE name_tug = @primary ORDER BY id_tug";
                            command.Parameters.AddWithValue("@primary", primary);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int primaryId = reader.GetInt32(0);
                                    string primaryNameDb = reader.GetString(1);
                                    int minTugId = primaryId;

                                    Console.WriteLine($"Primary {primaryNameDb} found with id_tug: {primaryId}");

                                    while (reader.Read())
                                    {
                                        int tugId = reader.GetInt32(0);
                                        string tugName = reader.GetString(1);

                                        if (tugId < minTugId)
                                        {
                                            minTugId = tugId;
                                            primaryId = tugId;
                                            primaryNameDb = tugName;
                                        }

                                        var updateQuery = "UPDATE aux_movement_tugs SET id_tug = @primaryId WHERE id_tug = @tugId";
                                        command.CommandText = updateQuery;
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@primaryId", primaryId);
                                        command.Parameters.AddWithValue("@tugId", tugId);
                                        command.ExecuteNonQuery();
                                        Console.WriteLine($"Updated aux_movement_tugs for primary {primaryNameDb} with id_tug: {primaryId}");

                                        var deleteQuery = "DELETE FROM aux_tugs WHERE id_tug = @tugId";
                                        command.CommandText = deleteQuery;
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@tugId", tugId);
                                        command.ExecuteNonQuery();
                                        Console.WriteLine($"Deleted duplicate primary {tugName} with id_tug: {tugId}");
                                    }

                                    foreach (string fakeName in fakes)
                                    {
                                        command.Parameters.Clear();
                                        command.CommandText = "SELECT id_tug FROM aux_tugs WHERE name_tug = @fakeName";
                                        command.Parameters.AddWithValue("@fakeName", fakeName);
                                        var fakeId = command.ExecuteScalar();
                                        if (fakeId != null)
                                        {
                                            command.CommandText = "UPDATE aux_movement_tugs SET id_tug = @primaryId WHERE id_tug = @fakeId";
                                            command.Parameters.Clear();
                                            command.Parameters.AddWithValue("@primaryId", primaryId);
                                            command.Parameters.AddWithValue("@fakeId", fakeId);
                                            command.ExecuteNonQuery();
                                            Console.WriteLine($"Updated aux_movement_tugs for fake {fakeName}");

                                            if ((int)fakeId != primaryId)
                                            {
                                                command.CommandText = "DELETE FROM aux_tugs WHERE id_tug = @fakeId";
                                                command.Parameters.Clear();
                                                command.Parameters.AddWithValue("@fakeId", fakeId);
                                                command.ExecuteNonQuery();
                                                Console.WriteLine($"Deleted fake {fakeName} from aux_tugs");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"No match found for fake {fakeName}");
                                        }
                                    }

                                    Console.WriteLine("Transaction committed.");
                                }
                                else
                                {
                                    Console.WriteLine($"No match found for primary {primary}");
                                }
                            }
                        }
                    }
                }
            }

            return Ok("Processing completed.");
        }
    }
}
