CREATE PROCEDURE Guidexists
(@guid nvarchar(50),
@guidexists nvarchar(10) OUT)
AS  
BEGIN 
	BEGIN TRAN
 
 	IF NOT EXISTS (SELECT * FROM Guids 
	with (TABLOCK, ROWLOCK  HOLDLOCK)
            WHERE Guid=@guid)

	BEGIN
		INSERT INTO Guids
		(Guid) VALUES (@guid)
		SET @guidexists = "NOTEXISTS"
	END
    ELSE
	BEGIN
		SET @guidexists = "EXISTS"
	END
	COMMIT TRAN
END
