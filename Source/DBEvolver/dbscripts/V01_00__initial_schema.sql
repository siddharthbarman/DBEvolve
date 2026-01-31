create table Books
(
	Id int not null,
	Title varchar(20) not null,	
	LastUpdated datetime not null default (GetUTCDate()),
	constraint PK_Books_ID primary key clustered (Id)		
)