CREATE USER [NBE] WITHOUT LOGIN WITH DEFAULT_SCHEMA = dbo
/*ALTER ROLE db_owner ADD MEMBER NBE*/ exec sp_addrolemember 'db_owner', 'NBE'
GO
