CREATE PROCEDURE [dbo].[SP_GetTestSSDT]
AS
	SELECT top 1 * from Employee;
RETURN 0
