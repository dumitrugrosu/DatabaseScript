using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PilotsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString = "Server=localhost;Database=aux_db;Uid=root;Pwd=Panatha4ever;";

        [HttpGet("UpdatePilots")]
        public IActionResult ProcessPilots()
        {
            string excelFilePath = _configuration["FilePaths:ExcelFilePath"]; // Update with the actual file path
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            // Read Excel file and populate data dictionary
            using (var workbook = new XLWorkbook(excelFilePath))
            {
                var worksheet = workbook.Worksheet(1); // Assuming data is on the first sheet

                var rows = worksheet.RowsUsed();
                foreach (var row in rows)
                {
                    string primary = row.Cell(2).Value.ToString(); // Assuming primary is in the first column
                    string fakesString = row.Cell(3).Value.ToString(); // Assuming fakes are in the second column
                    List<string> fakes = new List<string>(fakesString.Split(','));
                    if (!data.ContainsKey(primary))
                    {
                        data.Add(primary, fakes);
                    }
                    else
                    {
                    }
                }
            }

            // Connect to the database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                foreach (var kvp in data)
                {
                    string primary = kvp.Key;
                    List<string> fakes = kvp.Value;

                    // Retrieve id_pilot of the primary from aux_pilot
                    string query = "SELECT id_pilot FROM aux_pilot WHERE pilot = @primary";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@primary", primary);
                        int? primaryId = (int?)command.ExecuteScalar();

                        if (primaryId.HasValue)
                        {
                            // Assign id_pilot of the primary to the fakes
                            foreach (string fake in fakes)
                            {
                                // Retrieve id_pilot of the fake from aux_pilot
                                query = "SELECT id_pilot FROM aux_pilot WHERE pilot = @fake";
                                command.Parameters.Clear();
                                command.CommandText = query;
                                command.Parameters.AddWithValue("@fake", fake);
                                int? fakeId = (int?)command.ExecuteScalar();

                                if (fakeId.HasValue)
                                {
                                    // Update pilot_field values in aux_movement with id_pilot of primary
                                    query = "UPDATE aux_movement SET pilot_field = @primaryId WHERE pilot_field = @fakeId";
                                    command.Parameters.Clear();
                                    command.CommandText = query;
                                    command.Parameters.AddWithValue("@primaryId", primaryId);
                                    command.Parameters.AddWithValue("@fakeId", fakeId);
                                    command.ExecuteNonQuery();

                                    // Delete fake from aux_pilot
                                    query = "DELETE FROM aux_pilot WHERE id_pilot = @fakeId";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
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
