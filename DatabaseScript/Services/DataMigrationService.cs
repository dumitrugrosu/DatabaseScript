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
            var oldMovements = _sourceContext.AuxMovements.ToList();
            var oldMovementTugs = _sourceContext.AuxMovementTugs.ToList();
            var oldPilot = _sourceContext.AuxPilots.ToList();
            var oldTugs = _sourceContext.AuxTugs.ToList();

            // Add more queries for other entities as needed

            // Transform and migrate data to the new database
            foreach (var oldBarge in oldBarges)
            {
                var newBarge = new MngAuxBarges()
                {
                    // Map fields from oldBarge to newBarge according to the schema differences
                    // Example: newBarge.Property = oldBarge.Property;
                    Sid = oldBarge.IdBarge,
                    BargeName = oldBarge.Barge,
                };

                _destinationContext.MngAuxBarges.Add(newBarge);
            }

            foreach (var olPilot in oldPilot)
            {
                var newPilot = new MngAuxPilot()
                {
                    // Map fields from oldPilot to newPilot according to the schema differences
                    // Example: newPilot.Property = oldPilot.Property;
                    Sid = olPilot.IdPilot,
                    PilotName = olPilot.Pilot,
                };
                _destinationContext.MngAuxPilots.Add(newPilot);
            }

            foreach (var oldTug in oldTugs)
            {
                var newTug = new MngAuxTug()
                {
                    // Map fields from oldTug to newTug according to the schema differences
                    // Example: newTug.Property = oldTug.Property;
                    Sid = oldTug.IdTug,
                    TugName = oldTug.NameTug,
                };
                _destinationContext.MngAuxTugs.Add(newTug);
            }

            foreach (var oldMovement in oldMovements)
            {
                var newMovement = new AuxManeuver()
                {
                    // Map fields from oldMovement to newMovement according to the schema differences
                    // Example: newMovement.Property = oldMovement.Property;
                    Sid = oldMovement.IdMovement,
                };
                _destinationContext.AuxManeuvers.Add(newMovement);
            }

            // Add similar transformation logic for other entities

            // Save changes to the new database
            _destinationContext.SaveChanges();
        }
    }
}
