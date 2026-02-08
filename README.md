# DBEvolve

DBEvolve is a lightweight, versioned database migration tool for SQL Server, inspired by tools like Flyway and Liquibase, but designed to be simple, transparent, and script-first.

At its core, DBEvolve executes ordered SQL migration scripts, tracks their execution state, and ensures your database schema evolves safely and repeatably across environments.

The solution is split into following parts:

* **DBEvolveLib** – the reusable migration engine (core engine)
* **DBEvolveLib.MySql** – MySql specific migration
* **DBEvolveLib.Postgres** – Postgres specific migration
* **DBEvolveLib.SqlServer** – SqlServer specific migration
* **DBEvolver** – a command-line interface (CLI) built on top of the vendor specific libraries
* **Tests** – automated tests and sample migration scripts
* **PackageTesting** - sample application for testing MySql, Postgres and SqlServer DBEvolve packages

---

## Repository Structure

```text
DBEvolve/
├─ DBEvolveLib/           # Core migration library
├─ DBEvolveLib.MySql/     # MySql specific migration
├─ DBEvolveLib.Postgres/  # Postgres specific migration
├─ DBEvolveLib.SqlServer/ # SqlServer specific migration
├─ DBEvolver/             # Command-line tool (CLI) which works with MySql, Postgres and SqlServer
├─ Tests/           # Automated tests + sample SQL scripts
```

---

## DBEvolveLib (Core Library)

**Purpose:**

DBEvolveLib contains all the core logic required to perform database migrations. This project is used by 
vendor specific database migration code i.e. by DBEvolveLib.MySql, DBEvolveLib.Postgres and DBEvolveLib.SqlServer 
libraries. 

**Key Responsibilities:**

* Discovering and parsing versioned SQL migration scripts
* Managing schema version metadata
* Executing migrations in the correct order
* Providing extensibility points for different database engines
* Handling errors, logging, and execution safety

**Important Files:**

* `IDbManager.cs` – Abstraction for database-specific operations
* `DbManagerBase.cs` – Base implementation for database managers
* `ScriptFile.cs` – Represents a versioned SQL script (e.g. `V01_00__init.sql`)
* `Exceptions.cs` – Custom exception types
* `Utils.cs` – Shared helper utilities

**Target Frameworks:**

* .NET Standard (for maximum compatibility)

This project produces a NuGet package (`SByteStream.DBEvolveLib`) that can be reused across tools and applications.

---

## DBEvolveLib.MySql

**Purpose:**

This library is available as a nuget package on nuget.org & is intended to be consumed by developers 
wishing to add database migration for MySql facility to their application. 

**Key Responsibilities:**

* Implements the extension points required for migrating a MySql database
* MySql specific code for interacting with a MySql database

**Important Files:**
* `MySqlDBEvolver.cs` - The main class which a developer needs to intiate and use to peform a database migration.
* `MySqlDbManager.cs` - Internal class which has MySql specific code. 

**Target Frameworks:**

* .NET Standard (for maximum compatibility)

This project produces a NuGet package (`SByteStream.DBEvolveLib.MySql`) that can be reused across tools and applications.

---

## DBEvolveLib.Postgres

**Purpose:**

This library is available as a nuget package on nuget.org & is intended to be consumed by developers 
wishing to add database migration for Postgres facility to their application. 

**Key Responsibilities:**

* Implements the extension points required for migrating a Postgres database
* Postgres specific code for interacting with a MySql database

**Important Files:**
* `PostgresDBEvolver.cs` - The main class which a developer needs to intiate and use to peform a database migration.
* `PostgresDbManager.cs` - Internal class which has Postgres specific code. 

**Target Frameworks:**

* .NET Standard (for maximum compatibility)

This project produces a NuGet package (`SByteStream.DBEvolveLib.Postgres`) that can be reused across tools and applications.

---

## DBEvolveLib.SqlServer

**Purpose:**

This library is available as a nuget package on nuget.org & is intended to be consumed by developers 
wishing to add database migration for SqlServer facility to their application. 

**Key Responsibilities:**

* Implements the extension points required for migrating a SqlServer database
* SqlServer specific code for interacting with a MySql database

**Important Files:**
* `SqlServerDBEvolver.cs` - The main class which a developer needs to intiate and use to peform a database migration.
* `SqlServerDbManager.cs` - Internal class which has Postgres specific code. 

**Target Frameworks:**

* .NET Standard (for maximum compatibility)

This project produces a NuGet package (`SByteStream.DBEvolveLib.SqlServer`) that can be reused across tools and applications.

---

## DBEvolver (Command-Line Tool)

**Purpose:**

DBEvolver is a CLI application which uses all three database libraries to run database migrations against MySql, 
Postgres and SqlServer. It provides a simple command-line interface for applying migrations without writing any application code.

**Key Responsibilities:**

* Parsing command-line arguments
* Configuring database connections
* Locating migration scripts on disk
* Invoking the core migration engine
* Logging execution results

**Important Files:**

* `Program.cs` – Application entry point
* `CmdLine.cs` – Command-line argument parsing
* `SimpleFileLogger.cs` – Lightweight file-based logging

**Runtime Output:**

* `dbscripts/` – Folder containing migration SQL files
* `dbmaint.log` – Execution log

**Target Frameworks:**

* .NET 6 / .NET 8 / .NET 10 (multi-targeted)

The CLI is suitable for:

* CI/CD pipelines
* Local developer workflows
* Server-side automation

---

## Tests Project

**Purpose:**

The `Tests` project validates the behavior of the migration engine and demonstrates typical usage patterns.

**Key Responsibilities:**

* Verifying migration ordering and execution
* Ensuring version tracking behaves correctly
* Exercising error handling and edge cases

**Contents:**

* `TestScripts/` – Sample versioned SQL migration files

  * `V01_00__initial_schema.sql`
  * `V01_01__view.sql`

These scripts are intentionally simple and serve both as test inputs and examples for real-world usage.

---

## Migration Script Conventions

DBEvolve expects migration scripts to follow a strict naming convention:

```text
V<major>_<minor>__<description>.sql
```

Examples:

* `V01_00__initial_schema.sql`
* `V01_01__add_indexes.sql`

Scripts are executed in version order and recorded in a schema history table to ensure idempotency.

---

## Typical Usage Flow

1. Write versioned SQL scripts and place them in a `dbscripts` folder
2. Configure the database connection
3. Run the DBEvolve CLI (or call DBEvolveLib directly)
4. DBEvolve:

   * Detects pending migrations
   * Executes them in order
   * Records successful execution

---

## Who Is This For?

DBEvolve is ideal for teams who:

* Prefer **raw SQL** over DSL-based migrations
* Want **full control** over database changes
* Need a **lightweight and embeddable** migration engine
* Run migrations as part of **CI/CD pipelines**

---

## License

See the `LICENSE.TXT` file for usage terms.
