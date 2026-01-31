# dbevolver – Database Maintenance Utility

`dbevolver` is a lightweight database maintenance tool designed to simplify database creation and 
schema evolution. It is part of the **SByteStream.DBEvolve** package and leverages `Microsoft.Extensions.Logging` 
for logging. It makes use of the DBEvolveLib library (available on nuget.org) for its core functions.

## ✨ Features
- Create a new database from scripts.
- Migrate an existing database to the latest version.
- Track schema versions using a customizable version table.
- Configurable command timeout and script folder location.
- Simple command-line interface suitable for automation and CI/CD pipelines.


## Supported databases
- SqlServer (supported)
- MySQL (planned)
- PostgreSQL (planned)


## Installing

From nuget.org
```
dotnet tool install -g DBEvolver
dotnet tool list --global
```

From local nuget package
```
dotnet tool install --global DBEvolver --add-source DBEvolver
dotnet tool list --global
```

---

## ⚙️ Syntax

```bash
dbevolver -c <connection string> -f <scripts folder> -v <version> -n <version tablename> -t <command timeout>
