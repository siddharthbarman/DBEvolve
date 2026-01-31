create table ReconStatus
(
	Id int not null,
	ModuleName varchar(20) not null,
	StausName varchar(20) not null,
	LastUpdated datetime not null default (GetUTCDate()),
	constraint PK_ReconStatus primary key clustered (Id, ModuleName)		
);	