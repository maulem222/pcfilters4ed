CREATE PROCEDURE INSERT_produktnaam_with_outputparameter_ID
(@Produktnaam varchar(150),
@ID int output)
AS
BEGIN
SET NOCOUNT ON
   IF NOT EXISTS (SELECT Produktnaam FROM Produkten 
                   WHERE Produktnaam = @Produktnaam)
   BEGIN
		INSERT INTO Produkten (Produktnaam) VALUES (@Produktnaam)

		SELECT @ID= SCOPE_IDENTITY()
   END
END
