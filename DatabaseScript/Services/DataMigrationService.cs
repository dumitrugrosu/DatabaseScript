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
            var oldEstimatedTime = _sourceContext.AuxEstimatedTimes.ToList();
            var oldType = _sourceContext.AuxTypes.ToList();

            // Add more queries for other entities as needed

            // Transform and migrate data to the new database
            foreach (var oldBarge in oldBarges)
            {
                var newBarge = new MngAuxBarges()
                {
                    Sid = oldBarge.IdBarge,
                    BargeName = oldBarge.Barge,
                };

                _destinationContext.MngAuxBarges.Add(newBarge);
            }

            foreach (var olPilot in oldPilot)
            {
                var newPilot = new MngAuxPilot()
                {
                    Sid = olPilot.IdPilot,
                    PilotName = olPilot.Pilot,
                };
                _destinationContext.MngAuxPilots.Add(newPilot);
            }

            foreach (var oldTug in oldTugs)
            {
                var newTug = new MngAuxTug()
                {
                    Sid = oldTug.IdTug,
                    TugName = oldTug.NameTug,
                };
                _destinationContext.MngAuxTugs.Add(newTug);
            }

            foreach (var oldMovement in oldMovements)
            {
                var newMovement = new AuxManeuver()
                {
                    Sid = oldMovement.IdMovement,
                };
                _destinationContext.AuxManeuvers.Add(newMovement);
            }
            
            foreach (var oldMovementTug in oldMovementTugs)
            {
                var newMovementTug = new AuxManeuver()
                {
                    Sid = (long)oldMovementTug.IdMovementTugs,
                };
                _destinationContext.AuxManeuvers.Add(newMovementTug);
            }

            foreach (var oldEstimatedTimes in oldEstimatedTime)
            {
                var newEstimatedTime = new Entities.AuxEstimatedTime()
                {
                    Sid = oldEstimatedTimes.IdAux,
                    FromBerth = oldEstimatedTimes.A.ToString(),
                    ToBerth = oldEstimatedTimes.B.ToString(),
                    SumTimeSec = (int)oldEstimatedTimes.SumTime,
                    SumMan = oldEstimatedTimes.SumMan,
                    LastRegisterTime = DateTimeOffset.FromUnixTimeMilliseconds(oldEstimatedTimes.LastStamp).DateTime,
                };
                _destinationContext.AuxEstimatedTimes.Add(newEstimatedTime);
            }

            foreach (var oldTypes in oldType)
            {
                var newType = new MngAuxTugType()
                {
                    Sid = oldTypes.IdType,
                    Type = oldTypes.Type,
                    CountFree = oldTypes.CountEvenFreeRunning,
                    Bunker = oldTypes.BunkerField,
                    Barge = oldTypes.BargeField,
                    Ssn = oldTypes.SsnRequested

                };
                _destinationContext.MngAuxTugTypes.Add(newType);
            }

            _destinationContext.SaveChanges();
        }
    }
}
