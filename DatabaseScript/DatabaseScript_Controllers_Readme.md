# DatabaseScript Controllers Readme

## Overview
This repository contains three ASP.NET Core controllers (`TugsController`, `PilotsController`, `BargesController`) responsible for processing uploaded Excel files and manipulating data in a database accordingly. Each controller deals with a specific type of data entity: tugs, pilots, and barges.

## Controllers

### TugsController
- **Route**: `/Tugs`
- **Endpoints**:
  - **POST** `/UploadTugs`: Accepts an uploaded Excel file containing tug data in the format `(id_tug, primary, fakes)`, processes it, and updates the database accordingly.

### PilotsController
- **Route**: `/Pilots`
- **Endpoints**:
  - **POST** `/UploadPilots`: Accepts an uploaded Excel file containing pilot data in the format `(id_pilot, primary, fakes)`, processes it, and updates the database accordingly.

### BargesController
- **Route**: `/Barges`
- **Endpoints**:
  - **POST** `/UploadBarges`: Accepts an uploaded Excel file containing barge data in the format `(id_barge, primary, fakes)`, processes it, and updates the database accordingly.

## Excel Format
- Each uploaded Excel file should have three columns: `ID`, `Primary`, and `Fakes`.
- The `ID` column contains the unique identifier for the entity (tug, pilot, or barge).
- The `Primary` column contains the name of the primary entity.
- The `Fakes` column contains a comma-separated list of names representing fake entities associated with the primary entity.

## How to Use
1. Clone this repository to your local machine.
2. Ensure you have the necessary tools installed to run an ASP.NET Core application.
3. Configure the database connection string in `appsettings.json` and `AuxVesselsContext.cs` line 34.
4. Build and run the application.
5. **Ensure that you are connected to the appropriate database before making requests to the API endpoints**.
6. Send POST requests to the respective endpoints with an uploaded Excel file in the specified format to process and update the database.

## Dependencies
- **Microsoft.AspNetCore.Mvc**: For building RESTful APIs.
- **OfficeOpenXml**: For reading Excel files.
- **Microsoft.EntityFrameworkCore**: For database interaction.
- **DatabaseScript.Models**: Contains the data models used in the application.

## Configuration
- Each controller expects a connection string named `DefaultConnection` in the configuration file (`appsettings.json`).
- The file path where the uploaded Excel files will be stored is also configurable in `appsettings.json`.
