using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PilotsController : ControllerBase
    {
        private readonly string connectionString = "Server=localhost;Database=aux_db;Uid=root;Pwd=Panatha4ever;";

        [HttpGet]
        public IActionResult ProcessPilots()
        {
            string excelFilePath = "aux_pilots.xlsx"; // Update with the actual file path
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            // Read Excel file and populate data dictionary
            // Use appropriate method to read Excel file in C#, such as EPPlus or ClosedXML

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
