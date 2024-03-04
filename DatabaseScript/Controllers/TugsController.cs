using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ClosedXML.Excel;
using System.Linq;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TugsController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost;Database=aux_db;Uid=root;Pwd=Panatha4ever;";

        [HttpGet("UpdateTugs")]
        public IActionResult ProcessTugs()
        {
            string excelFilePath = "aux_tugs.xlsx";
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            try
            {
                using (var workbook = new XLWorkbook(excelFilePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    foreach (var row in worksheet.RowsUsed())
                    {
                        string primary = row.Cell(2).Value.ToString().Trim();
                        string fakesString = row.Cell(3).Value.ToString().Trim();
                        List<string> fakes = new List<string>(fakesString.Split(',').Select(s => s.Trim()));
                        if (!data.ContainsKey(primary))
                        {
                            data.Add(primary, fakes);
                        }
                    }
                }

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var kvp in data)
                            {
                                ProcessPrimaryTug(connection, transaction, kvp.Key, kvp.Value);
                            }

                            transaction.Commit();
                            return Ok("Processing completed successfully.");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return StatusCode(500, $"An error occurred: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while reading the Excel file: {ex.Message}");
            }
        }

        private void ProcessPrimaryTug(MySqlConnection connection, MySqlTransaction transaction, string primaryName, List<string> fakes)
        {
            using (var command = new MySqlCommand())
            {
                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = "SELECT id_tug, name_tug FROM aux_tugs WHERE name_tug = @primaryName ORDER BY id_tug";
                command.Parameters.AddWithValue("@primaryName", primaryName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int primaryId = reader.GetInt32(0);
                        string primaryNameDb = reader.GetString(1);

                        Console.WriteLine($"Primary {primaryNameDb} found with id_tug: {primaryId}");

                        reader.Close();

                        foreach (var fakeName in fakes)
                        {
                            UpdateAndDeleteFakeTugs(connection, transaction, primaryId, fakeName);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No match found for primary {primaryName}");
                    }
                }
            }
        }

        private void UpdateAndDeleteFakeTugs(MySqlConnection connection, MySqlTransaction transaction, int primaryId, string fakeName)
        {
            using (var command = new MySqlCommand())
            {
                command.Connection = connection;
                command.Transaction = transaction;

                command.CommandText = "SELECT id_tug FROM aux_tugs WHERE name_tug = @fakeName";
                command.Parameters.AddWithValue("@fakeName", fakeName);
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    int fakeId = Convert.ToInt32(result);

                    command.CommandText = "UPDATE aux_movement_tugs SET id_tug = @primaryId WHERE id_tug = @fakeId";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@primaryId", primaryId);
                    command.Parameters.AddWithValue("@fakeId", fakeId);
                    command.ExecuteNonQuery();

                    Console.WriteLine($"Updated aux_movement_tugs for fake {fakeName}");

                    if (primaryId != fakeId)
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
        }
    }
}
