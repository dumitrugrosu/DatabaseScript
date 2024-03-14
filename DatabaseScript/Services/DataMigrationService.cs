﻿using DatabaseScript.Context;
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
                var newBarge = new MngAuxBarges()
                {
                    Sid = oldBarge.IdBarge,
                    BargeName = oldBarge.Barge,
                };

                _destinationContext.MngAuxBarges.Add(newBarge);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_BARGES] OFF");
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_PILOTS] ON");
            foreach (var olPilot in oldPilot)
            {
                 var newPilot = new MngAuxPilot()
                 {
                     Sid = olPilot.IdPilot,
                     PilotName = olPilot.Pilot,
                 };
                 _destinationContext.MngAuxPilots.Add(newPilot);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_PILOTS] OFF");
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUGS] ON");

            foreach (var oldTug in oldTugs)
            {
                var newTug = new MngAuxTug()
                {
                    Sid = oldTug.IdTug,
                    TugName = oldTug.NameTug,
                };
                _destinationContext.MngAuxTugs.Add(newTug);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUGS] OFF");
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] ON");
            foreach (var oldMovement in oldMovements)
            {
                var newMovement = new AuxManeuver()
                {
                    Sid = oldMovement.IdMovement,
                    BunkerFieldName = oldMovement.BunkerField,
                    UserSid = oldMovement.IdUserStart,
                    UserEndSid = oldMovement.IdUserStop,
                    FromTime = DateTimeOffset.FromUnixTimeMilliseconds(oldMovement.StartTime).DateTime,
                    ToTime = DateTimeOffset.FromUnixTimeMilliseconds(oldMovement.StopTime).DateTime,
                    PausedSec = oldMovement.Paused,


                };
                _destinationContext.AuxManeuvers.Add(newMovement);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] OFF");
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
            _destinationContext.Database.OpenConnection();
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] ON");

            foreach (var oldMovementTug in oldMovementTugs)
            {
                var newMovementTug = new AuxManeuver()
                {
                    Sid = (long)oldMovementTug.IdMovementTugs,
                };
                _destinationContext.AuxManeuvers.Add(newMovementTug);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_MANEUVERS] OFF");
            _destinationContext.SaveChanges();
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
                    LastRegisterTime = DateTimeOffset.FromUnixTimeMilliseconds(oldEstimatedTimes.LastStamp).DateTime,
                };
                _destinationContext.AuxEstimatedTimes.Add(newEstimatedTime);
            }
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[AUX_ESTIMATED_TIMES] OFF");
            _destinationContext.SaveChanges();
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
            _destinationContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[MNG_AUX_TUG_TYPES] OFF");
            _destinationContext.SaveChanges();
            _destinationContext.Database.CloseConnection();
        }
    }
}
