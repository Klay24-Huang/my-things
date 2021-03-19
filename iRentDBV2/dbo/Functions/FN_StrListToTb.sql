-- =============================================
-- Author:		eason
-- Create date: 2021-03-19
-- Description:	字串轉Tale
-- =============================================
CREATE FUNCTION [dbo].[FN_StrListToTb]
(
    @strList varchar(max) --List字串如a1,a2,a3..
)
RETURNS 
@re TABLE 
(
   sValue nvarchar(max)
)
AS
BEGIN		
		SET @strList = ISNULL(@strList,'')
		IF @strList<> ''
		BEGIN
			DECLARE @returnList TABLE ([Name] [nvarchar] (max))
			DECLARE @stringToSplit VARCHAR(MAX) = @strList		
			DECLARE @name NVARCHAR(max)
			DECLARE @pos INT
			WHILE CHARINDEX(',', @stringToSplit) > 0
			BEGIN
			    SELECT @pos  = CHARINDEX(',', @stringToSplit)  
			    SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)
			    INSERT INTO @returnList 
			    SELECT @name
			    SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
			END
			INSERT INTO @returnList
			SELECT @stringToSplit 

			DECLARE @re_Count INT = 0
			SELECT @re_Count = COUNT(*) FROM @returnList r WHERE r.Name IS NOT NULL AND r.Name <> ''			
			
			IF @re_Count > 0
			BEGIN
				INSERT INTO @re
				SELECT r.Name FROM @returnList r WHERE r.Name IS NOT NULL AND r.Name <> ''
			END
		END 
	RETURN 
END
GO