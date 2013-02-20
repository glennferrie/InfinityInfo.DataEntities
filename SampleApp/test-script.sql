--Create Database EFSample

use EFSample

Drop Table SampleTable

CREATE TABLE SampleTable
(
	Id int,
	LastName varchar(255) null,
	FirstName varchar(255) null,
	AddressLine varchar(255) null,
	City varchar(255) null, 
	CreateDate dateTime2 null	
)