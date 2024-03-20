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
                var fromBerth = _sourceContext.AuxBerths.FirstOrDefault(b => b.IdBerth == movement.IdFrom)?.Berth;
                var toBerth = _sourceContext.AuxBerths.FirstOrDefault(b => b.IdBerth == movement.IdTo)?.Berth;
                var newManeuver = new AuxManeuver()
                {
                    Sid = movement.IdMovement,
                    BunkerFieldName = movement.BunkerField,
                    UserSid = movement.IdUserStart,
                    UserEndSid = movement.IdUserStop,
                    FromTime = DateTimeOffset.FromUnixTimeSeconds(movement.StartTime).DateTime,
                    ToTime = DateTimeOffset.FromUnixTimeSeconds(movement.StopTime).DateTime,
                    PausedSec = movement.Paused,
                    RegisterTime = DateTime.Now,
                    FromPosition = fromBerth,
                    ToPosition = toBerth
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
                if (!_destinationContext.MngAuxTugs.Any(tug => tug.Sid == oldMovementTug.IdTug))
                {
                    // Log or handle the case where the TUG_SID does not exist
                    // This could involve logging an error or taking other actions as needed
                    Console.WriteLine($"Skipping entry: TUG_SID {oldMovementTug.IdTug} does not exist in MNG_AUX_TUGS");
                    continue; // Move to the next iteration
                }

                if (!_destinationContext.AuxManeuvers.Any(maneuver => maneuver.Sid == oldMovementTug.IdMovement))
                {
                    Console.WriteLine($"Skipping entry: MOVEMENT_SID {oldMovementTug.IdMovement} does not exist in AUX_MANEUVERS");
                    continue;
                }
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
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIME] ON");
            // Ensure unique key values
            foreach (var oldEstimatedTimes in oldEstimatedTime)
            {
                long key = oldEstimatedTimes.IdAux; // Ensure key is of type long

                // Find the entity with the specified key
                var existingEntity = _destinationContext.AuxEstimatedTimes.Find(key);

                if (existingEntity != null)
                {
                    // If found, modify the existing entity or detach it
                    // Example: existingEntity.SomeProperty = newValue;
                    _destinationContext.Entry(existingEntity).State = EntityState.Detached;
                }

                // Create a new entity instance
                var newEstimatedTime = new Entities.AuxEstimatedTime()
                {
                    Sid = oldEstimatedTimes.IdAux,
                    FromBerth = oldEstimatedTimes.A.ToString(),
                    ToBerth = oldEstimatedTimes.B.ToString(),
                    SumTimeSec = (int)oldEstimatedTimes.SumTime,
                    SumMan = oldEstimatedTimes.SumMan,
                    LastRegisterTime = DateTimeOffset.FromUnixTimeSeconds(oldEstimatedTimes.LastStamp).DateTime,
                };

                // Add the new entity to the context
                _destinationContext.AuxEstimatedTimes.Add(newEstimatedTime);
            }

            // Save changes
            _destinationContext.SaveChanges();

            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIME] OFF");

            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            foreach (var oldMovement in oldMovements)
            {
                if (!_destinationContext.MngAuxPilots.Any(pilot => pilot.Sid == oldMovement.PilotField))
                {
                    Console.WriteLine($"Skipping entry: PILOT_SID {oldMovement.PilotField} does not exist in MNG_AUX_PILOTS");
                    continue;
                }

                if (!_destinationContext.AuxManeuvers.Any(maneuver => maneuver.Sid == oldMovement.IdMovement))
                {
                    Console.WriteLine($"Skipping entry: MOVEMENT_SID {oldMovement.IdMovement} does not exist in AUX_MANEUVERS");
                    continue;
                }
                var newPilots = new Entities.AuxPilot()
                {
                    AuxSid = oldMovement.IdMovement,
                    PilotSid = oldMovement.PilotField,
                };
                _destinationContext.AuxPilots.Add(newPilots);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            foreach (var oldMovement in oldMovements)
            {
                if (!_destinationContext.MngAuxBarges.Any(barge => barge.Sid == oldMovement.BargeField))
                {
                    Console.WriteLine($"Skipping entry: BARGE_SID {oldMovement.BargeField} does not exist in MNG_AUX_BARGES");
                    continue;
                }

                if (!_destinationContext.AuxManeuvers.Any(maneuver => maneuver.Sid == oldMovement.IdMovement))
                {
                    Console.WriteLine($"Skipping entry: MOVEMENT_SID {oldMovement.IdMovement} does not exist in AUX_MANEUVERS");
                    continue;
                }
                var newBarges = new Entities.AuxBarge()
                {
                    AuxSid = oldMovement.IdMovement,
                    BargeSid = (long)oldMovement.BargeField,
                };
                _destinationContext.AuxBarges.Add(newBarges);
            }
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPE] ON");
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
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPE] OFF");

            _destinationContext.Database.CloseConnection();
            Console.WriteLine("Data migration completed successfully.");
        }

    }
}
