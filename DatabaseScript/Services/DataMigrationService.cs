using DatabaseScript.Context;
using DatabaseScript.Models;
using DatabaseScript.Entities;
using Microsoft.EntityFrameworkCore; // Import entities from both contexts

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
            _destinationContext.Database.OpenConnection();
            // Enable identity insert for the destination context
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_BARGES] ON");
            
            
            /* _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUGS] ON");
             _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPES] ON");
             _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIMES] ON");
             _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] ON");*/

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
                var newMngAuxBarges = new MngAuxBarges()
                {
                    Sid = oldBarge.IdBarge,
                    BargeName = oldBarge.Barge,
                };

                _destinationContext.MngAuxBarges.Add(newMngAuxBarges);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_BARGES] OFF");
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_PILOTS] ON");
            foreach (var olPilot in oldPilot)
            {
                 var newMngAuxPilot = new MngAuxPilot()
                 {
                     Sid = olPilot.IdPilot,
                     PilotName = olPilot.Pilot,
                 };
                 _destinationContext.MngAuxPilots.Add(newMngAuxPilot);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_PILOTS] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUGS] ON");

            foreach (var oldTug in oldTugs)
            {
                var newMngAuxTug = new MngAuxTug()
                {
                    Sid = oldTug.IdTug,
                    TugName = oldTug.NameTug,
                };
                _destinationContext.MngAuxTugs.Add(newMngAuxTug);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUGS] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] ON");

            oldMovements.ForEach(movement =>
            {
                var newManeuver = new AuxManeuver()
                {
                    Sid = movement.IdMovement,
                    BunkerFieldName = movement.BunkerField,
                    UserSid = movement.IdUserStart,
                    UserEndSid = movement.IdUserStop,
                    FromTime = DateTimeOffset.FromUnixTimeSeconds(movement.StartTime).DateTime,
                    ToTime = DateTimeOffset.FromUnixTimeSeconds(movement.StopTime).DateTime,
                    PausedSec = movement.Paused,
                    RegisterTime = DateTime.Now
                };
                _destinationContext.AuxManeuvers.AddRange(newManeuver);

            });
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_TUGS] ON");

            foreach (var oldMovementTug in oldMovementTugs)
            {
                var newTugs = new AuxTug()
                {
                    Sid = (long)oldMovementTug.IdMovementTugs,
                    AuxSid = oldMovementTug.IdMovement,
                    TugSid = oldMovementTug.IdTug,
                };
                _destinationContext.AuxTugs.Add(newTugs);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_TUGS] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIMES] ON");
            foreach (var oldEstimatedTimes in oldEstimatedTime)
            {


                var newEstimatedTime = new Entities.AuxEstimatedTime()
                {
                    Sid = oldEstimatedTimes.IdAux,
                    FromBerth = oldEstimatedTimes.A.ToString(),
                    ToBerth = oldEstimatedTimes.B.ToString(),
                    SumTimeSec = (int)oldEstimatedTimes.SumTime,
                    SumMan = oldEstimatedTimes.SumMan,
                    LastRegisterTime = DateTimeOffset.FromUnixTimeSeconds(oldEstimatedTimes.LastStamp).DateTime,
                };
                _destinationContext.AuxEstimatedTimes.Add(newEstimatedTime);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIMES] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPES] ON");
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
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPES] OFF");

            _destinationContext.Database.CloseConnection();
        }
    }
}
