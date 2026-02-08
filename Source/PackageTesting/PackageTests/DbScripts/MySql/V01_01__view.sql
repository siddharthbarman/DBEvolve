create view AuthorBooksView as
select a.FirstName, a.LastName, b.Title from Authors a inner join Books b on a.Id = b.AuthorId;   
