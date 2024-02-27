using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ClosedXML.Excel;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BargesController : ControllerBase
    {
        private readonly string connectionString = "Server=localhost;Database=aux_db;Uid=root;Pwd=Panatha4ever;";

        [HttpGet("UpdateBarges")]
        public IActionResult ProcessBarges()
        {
            string excelFilePath = "C:\\Users\\dumitru.grosu\\Documents\\DataReading\\DatabaseScript\\DatabaseScript\\CorrectData.xlsx"; // Update with the actual file path
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

                    // Retrieve id_barge of the primary from aux_barge
                    string query = "SELECT id_barge FROM aux_barge WHERE barge = @primary";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@primary", primary);
                        int? primaryId = (int?)command.ExecuteScalar();

                        if (primaryId.HasValue)
                        {
                            // Assign id_barge of the primary to the fakes
                            foreach (string fake in fakes)
                            {
                                // Retrieve id_barge of the fake from aux_barge
                                query = "SELECT id_barge FROM aux_barge WHERE barge = @fake";
                                command.Parameters.Clear();
                                command.CommandText = query;
                                command.Parameters.AddWithValue("@fake", fake);
                                int? fakeId = (int?)command.ExecuteScalar();

                                if (fakeId.HasValue)
                                {
                                    // Update barge_field values in aux_movement with id_barge of primary
                                    query = "UPDATE aux_movement SET barge_field = @primaryId WHERE barge_field = @fakeId";
                                    command.Parameters.Clear();
                                    command.CommandText = query;
                                    command.Parameters.AddWithValue("@primaryId", primaryId);
                                    command.Parameters.AddWithValue("@fakeId", fakeId);
                                    command.ExecuteNonQuery();

                                    // Delete fake from aux_barge
                                    query = "DELETE FROM aux_barge WHERE id_barge = @fakeId";
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
