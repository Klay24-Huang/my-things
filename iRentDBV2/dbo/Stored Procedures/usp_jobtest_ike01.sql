/****************************************************************
Job test
*****************************************************************/
/*
	EXEC usp_jobtest_ike01 '1'
*/

CREATE PROCEDURE [dbo].[usp_jobtest_ike01]
	@IDNO                   VARCHAR(10)           
AS

IF (SELECT ROUND(RAND(),0))= 0
BEGIN
	PRINT 'SUCCESS'
	RETURN 0
END
ELSE
BEGIN
	PRINT 'ERROR'
	RETURN 1
END