using System.Linq;
using DatabaseScript.Context;
using DatabaseScript.Models;
using DatabaseScript.Entities; // Import entities from both contexts

namespace DatabaseScript.Services
{
    public class DataMigrationService : IDataMigrationService
    {
        private readonly ScriptDbContext _sourceContext;
        private readonly AuxVesselsContext _destinationContext;

        public DataMigrationService(ScriptDbContext sourceContext, AuxVesselsContext destinationContext)
        {
            _sourceContext = sourceContext;
            _destinationContext = destinationContext;
        }

        public void MigrateData()
        {
            // Retrieve data from the old database
            var oldBarges = _sourceContext.AuxBarges.ToList();
            var oldMovement = _sourceContext.AuxMovements.ToList();
            var oldMovementTugs = _sourceContext.AuxMovementTugs.ToList();
            var oldPilot = _sourceContext.AuxPilots.ToList();
            var oldTugs = _sourceContext.AuxTugs.ToList();

            // Add more queries for other entities as needed

            // Transform and migrate data to the new database
            foreach (var oldBarge in oldBarges)
            {
                var newBarge = new Entities.MngAuxBarge()
                {
                    // Map fields from oldBarge to newBarge according to the schema differences
                    // Example: newBarge.Property = oldBarge.Property;
                };

                _destinationContext.MngAuxBarge.Add(newBarge);
            }

            foreach (var olPilot in oldPilot)
            {
                
            }

            // Add similar transformation logic for other entities

            // Save changes to the new database
            _destinationContext.SaveChanges();
        }
    }
}
