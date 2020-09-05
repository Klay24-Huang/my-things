CREATE VIEW [dbo].[VW_QueryTableSchema]
AS
SELECT  a.TABLE_NAME AS 表格名稱, b.COLUMN_NAME AS 欄位名稱, b.DATA_TYPE AS 資料型別, b.CHARACTER_MAXIMUM_LENGTH AS 最大長度, 
               b.COLUMN_DEFAULT AS 預設值, b.IS_NULLABLE AS 允許空值,
                   (SELECT  VALUE
                   FROM     fn_listextendedproperty(NULL, 'schema', 'dbo', 'table', a.TABLE_NAME, 'column', DEFAULT) AS fn_listextendedproperty_1
                   WHERE   (name = 'MS_Description') AND (objtype = 'COLUMN') AND (objname COLLATE Chinese_Taiwan_Stroke_CI_AS = b.COLUMN_NAME)) 
               AS 欄位備註
FROM     INFORMATION_SCHEMA.TABLES AS a LEFT OUTER JOIN
               INFORMATION_SCHEMA.COLUMNS AS b ON a.TABLE_NAME = b.TABLE_NAME
WHERE   (a.TABLE_TYPE = 'BASE TABLE')
GO
