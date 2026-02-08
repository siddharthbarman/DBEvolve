create table Authors
(
	Id int not null,
	FirstName varchar(20) not null,
	LastName varchar(20) null,
	constraint PK_Authors_ID primary key (Id)
);

create table Books
(
	Id int not null,
	Title varchar(20) not null,		
	AuthorId int not null, 
	constraint PK_Books_ID primary key (Id)		
);