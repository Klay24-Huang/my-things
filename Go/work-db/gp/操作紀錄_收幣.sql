-- 收幣
-- 確認原始資料
SELECT * FROM operation_record WHERE action_project = 'Y&U收幣' OR content = 'Y&U收幣' LIMIT 100;

--  修改
UPDATE operation_record
SET action_project =  'Y收幣', content = 'Y收幣'
WHERE (action_project = 'Y&U收幣' OR content = 'Y&U收幣');

-- 修改後確認
SELECT * FROM operation_record WHERE action_project = 'Y收幣' OR content = 'Y收幣' LIMIT 100;

