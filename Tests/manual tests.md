# Postgres

Run DB Evolver using the command:
```
dbevolver -t Postgres -c "Server=localhost; Port=5432; Database=MyAppDb; UserId=postgres; Password=password123;" -f .\dbscripts\Postgres
```

## Postgres 14
```
docker pull postgres:14
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:14
docker stop postgres
docker rm postgres
docker image rm postgres:14
```

To connect from DBeaver, edit the dbeaver.ini and add the line:
```
-Duser.timezone=UTC
```

## Postgres 15
```
docker pull postgres:15
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:15
docker stop postgres
docker rm postgres
docker image rm postgres:15
```

To connect from DBeaver, edit the dbeaver.ini and add the line:
```
-Duser.timezone=UTC
```

## Postgres 16
```
docker pull postgres:16
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:16
docker stop postgres
docker rm postgres
docker image rm postgres:16
```

To connect from DBeaver, edit the dbeaver.ini and add the line:
```
-Duser.timezone=UTC
```

## Postgres 17
```
docker pull postgres:17
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:17
docker stop postgres
docker rm postgres
docker image rm postgres:17
```

To connect from DBeaver, edit the dbeaver.ini and add the line:
```
-Duser.timezone=UTC
```

## Postgres 18
```
docker pull postgres:18
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:18
docker stop postgres
docker rm postgres
docker image rm postgres:18
```

To connect from DBeaver, edit the dbeaver.ini and add the line:
```
-Duser.timezone=UTC
```

# MySql

```
dbevolver -t MySql -c "server=localhost:3306; uid=root; pwd=password123; database=MyAppDb;" -f .\dbscripts\MySql

```


## MySql 5.7

```
docker pull mysql:5.7.22
docker run --name mysql -e MYSQL_ROOT_PASSWORD=password123 -p 3306:3306 -d mysql:5.7.22
docker stop mysql
docker rm mysql
docker image rm mysql:5.7.22
```

## MySql 8.0

```
docker pull mysql:8.0.45
docker run --name mysql -e MYSQL_ROOT_PASSWORD=password123 -p 3306:3306 -d mysql:8.0.45
docker stop mysql
docker rm mysql
docker image rm mysql:8.0.45
```

### DBeaver settings
Add a new driver property named "allowPublicKeyRetrieval" and set it to "true". 
This can be done by Edit Connection -> Driver Properties -> Right click -> Add new property


## MySql 9.0
```
docker pull mysql:9.1.0
docker run --name mysql -e MYSQL_ROOT_PASSWORD=password123 -p 3306:3306 -d mysql:9.1.0
docker stop mysql
docker rm mysql
docker image rm mysql:5.7.22
```

### DBeaver settings
Add a new driver property named "allowPublicKeyRetrieval" and set it to "true". 
This can be done by Edit Connection -> Driver Properties -> Right click -> Add new property