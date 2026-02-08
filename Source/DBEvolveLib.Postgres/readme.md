# DBEvolveLib.Postgres

DBEvolve is a lightweight, deterministic database migration library for .NET that automates database creation, schema evolution, and version tracking using versioned SQL scripts.

## Features

- **Automatic database creation** - Creates the database if it doesn't exist

- **Version-based migration execution** - Executes SQL scripts in a controlled, versioned order

- **Persistent schema version tracking** - Tracks applied migrations in a version history table. The name of the version history table can be changed.
  
- **Idempotent upgrades** - Only new scripts are applied; previously run scripts are skipped

- **Script integrity validation** - Validates that applied scripts haven't been modified using SHA-256 hashes

- **Simple, script-first migration model** - Full control over database changes using explicit SQL scripts

- **Designed for CI/CD** - Perfect for automated deployments

## Supported Databases
- SQL Server
- PostgreSQL 14, 15, 16,17, 18 (Supported by *this* library)
- MySQL planned

## Installation
Install the NuGet package:
```
dotnet add package SByteStream.DBEvolve.Postgres
```

## Quick Start

```
using Microsoft.Extensions.Logging; 
using SByteStream.DBEvolve;

// Create a logger (using any ILogger implementation) 
ILogger logger = LoggerFactory.Create(builder => builder.AddConsole())
    .CreateLogger("DBEvolve");

// Define your connection string 
string connectionString = "Server=localhost; Port=5432; Database=MyAppDb; UserId=postgres; Password=password123;";

// Define the directory containing your SQL scripts 
string scriptsDirectory = @"C:\MyProject\DbScripts";

// Create and run the evolver 
var evolver = new PostgresDBEvolver(); 
evolver.Evolve(logger, connectionString, scriptsDirectory);
```

## SQL Script Naming Convention
DBEvolve uses a strict naming convention for SQL script files to determine the execution order and version:

### Format
```
V<MajorVersion>_<MinorVersion>__<Description>.sql
```

### Rules
1. **Must start with 'V'** (uppercase)
2. **Major and Minor versions** are separated by an underscore (`_`)
3. **Double underscore (`__`)** separates the version from the description
4. **Description** can be any meaningful text (e.g., `initial_schema`, `add_users_table`)
5. **File extension** must be `.sql`

### Examples
V01_00__initial_schema.sql -> Version 100 
V01_01__add_users_table.sql -> Version 101 
V01_02__create_indexes.sql -> Version 102 
V02_00__add_orders_module.sql -> Version 200 
V02_01__orders_stored_procs.sql -> Version 201

### Version Calculation
The numeric version is calculated as: `(MajorVersion × 100) + MinorVersion`

- `V01_00` = 1 × 100 + 0 = **100**
- `V01_01` = 1 × 100 + 1 = **101**
- `V02_00` = 2 × 100 + 0 = **200**

## Version History Table
DBEvolve automatically creates and manages a version history table to track applied migrations.

### Default Table Name
`__Version_History__`

### Schema
```
CREATE TABLE [Version_History] 
( 
    VersionNumber int not null primary key,
	Filename varchar(512) not null,
    FileHash bytea not null,
	EntryDate TIMESTAMPTZ NOT NULL DEFAULT (now() AT TIME ZONE 'UTC')
)
```

### Columns
- **VersionNumber**: The calculated version number (e.g., 100, 101, 102)
- **Filename**: Full path of the applied script file
- **FileHash**: SHA-256 hash of the script file for integrity checking
- **EntryDate**: UTC timestamp when the script was applied

### Custom Table Name
You can specify a custom version history table name:
```
var evolver = new PostgresDBEvolver();

evolver.Evolve(logger, connectionString, scriptsDirectory, versionTableName: "MyCustomVersionTable");
```

## How Migrations Work

### Process Flow
1. **Database Creation**: If the database doesn't exist, it is created automatically
2. **Version History Table**: The version history table is created if it doesn't exist
3. **Script Validation**: All previously applied scripts are validated:
   - Checks that script files still exist
   - Verifies file hashes match (no modifications)
4. **Current Version Detection**: Queries the version history table to determine the current database version
5. **Script Execution**: Runs all scripts with versions greater than the current version in order
   - All statements in a script are executed within a transation.
   - In case of an error while executing a script, the transaction for that script is rolled back and the version is not modified.   
1. **Version Recording**: Records each successfully executed script in the version history table

### Migration Example
Given these scripts in your directory:
V01_00__initial_schema.sql 
V01_01__add_users.sql 
V01_02__add_orders.sql

**First Run (Empty Database)**:
- Current version: 0
- Executes: V01_00, V01_01, V01_02
- New version: 102

**Second Run (Add New Script)**:
V01_00__initial_schema.sql 
V01_01__add_users.sql 
V01_02__add_orders.sql 
V01_03__add_indexes.sql <- New script

- Current version: 102
- Executes: V01_03 only
- New version: 103

### Targeting Specific Versions
You can upgrade to a specific version instead of the latest:
```
var evolver = new PostgresDBEvolver(); // Upgrade only up to version 101 
evolver.Evolve(logger, connectionString, scriptsDirectory, maxVersion: 101);
```

## Script Integrity
DBEvolve protects against accidental script modifications by:

1. **Hashing**: Computing a SHA-256 hash when a script is applied
2. **Validation**: Checking hashes on subsequent runs
3. **Failing Fast**: Throwing an exception if a previously applied script has been modified

### Important

⚠️ **Never modify a script that has already been applied to any environment.** Always create a new script with a higher version number for schema changes.

## Usage Examples

### Basic Usage
```
using Microsoft.Extensions.Logging; using SByteStream.DBEvolve;

var logger = LoggerFactory.Create(builder => builder.AddConsole())
    .CreateLogger("DBEvolve"); 

var evolver = new PostgresDBEvolver();
evolver.Evolve(logger: logger, 
    connectionString: "Server=localhost; Port=5432; Database=MyAppDb; UserId=postgres; Password=password123;", 
    scriptsDirectory: @".\DbScripts" );
```

### With Custom Version Table
```
var evolver = new PostgresDBEvolver();
evolver.Evolve( logger: logger, 
    connectionString: connectionString, 
    scriptsDirectory: scriptsDirectory, 
    versionTableName: "MyApp_VersionHistory" );
```

### Upgrade to Specific Version
```
var evolver = new PostgresDBEvolver();

// Upgrade only to version 105 
evolver.Evolve( logger: logger, 
    connectionString: connectionString, 
    scriptsDirectory: scriptsDirectory, 
    maxVersion: 105 );
```

### Specify DBEvolve's command timeout  
```
var evolver = new PostgresDBEvolver();

// Upgrade only to version 105 
evolver.Evolve( logger: logger, 
    connectionString: connectionString, 
    scriptsDirectory: scriptsDirectory, 
    commandTimeoutSec: 60 );
```


### Complete Example with Error Handling
```
using Microsoft.Extensions.Logging; using SByteStream.DBEvolve;

class Program 
{ 
    static void Main() 
    { 
        var loggerFactory = LoggerFactory.Create(builder => 
        { 
            builder.AddConsole(); 
            builder.SetMinimumLevel(LogLevel.Information); 
        });
    
        var logger = loggerFactory.CreateLogger<Program>();

        try
        {
            var evolver = new PostgresDBEvolver();
            evolver.Evolve(
                logger: logger,
                connectionString: "Server=localhost; Port=5432; Database=MyAppDb; UserId=postgres; Password=password123;",
                scriptsDirectory: @"C:\MyProject\DbScripts"
            );

            logger.LogInformation("Database migration completed successfully!");
        }
        catch (DBEvolveException ex)
        {
            logger.LogError(ex, "Database migration failed: {Message}", ex.Message);
            Environment.Exit(1);
        }
    }
}
```

## What happens if a script has an error?
All SQL statements of a script block are executed within a transaction. 
In case a statement generates an error, the transaction is rolled back and 
no entry is made for the script in the version table.

## SQL Script Best Practices

### One Migration Per Script

Keep scripts focused on a single logical change:

- ✅ `V01_05__add_users_table.sql`
- ✅ `V01_06__add_orders_table.sql`
- ❌ `V01_05__add_all_tables.sql` (too broad)

### Use Descriptive Names

Make script descriptions meaningful:

- ✅ `V02_03__add_email_column_to_users.sql`
- ❌ `V02_03__update.sql`

### Never Modify Applied Scripts

Once a script has been applied to any environment:

- ✅ Create a new script with a higher version
- ❌ Modify the existing script (will fail hash validation)

## Target Framework

- **.NET Standard 2.0** (compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+, .NET 6+, .NET 8+)

## Dependencies

- Microsoft.Extensions.Logging.Abstractions (>= 6.0.4)
- Npgsql (7.0..10)

## License

Copyright (C) 2026, Siddharth R Barman

# Attributions
<a href="https://www.flaticon.com/free-icons/data-migration" title="data migration icons">Data migration icons created by Ida Desi Mariana - Flaticon</a>

## Licensing
This project is source-available under the SByteStream No-Modification License.

## Support

For issues, questions, or contributions, please visit the project repository.

You can also contact me at sbytestream@outlook.com.