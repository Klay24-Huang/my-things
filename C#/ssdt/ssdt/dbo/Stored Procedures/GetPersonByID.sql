﻿CREATE PROCEDURE GetPersonByID
    @PersonID INT
AS
BEGIN
    SELECT * FROM ExampleTable WHERE ID = @PersonID
END