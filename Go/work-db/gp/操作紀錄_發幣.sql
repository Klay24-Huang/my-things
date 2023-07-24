-- 發幣
-- 確認原始資料
SELECT * FROM operation_record WHERE action_project = 'Y&U發幣' OR content = 'Y&U發幣' LIMIT 100;

--  修改
UPDATE operation_record
SET action_project =  'Y發幣', content = 'Y發幣'
WHERE (action_project = 'Y&U發幣' OR content = 'Y&U發幣');

-- 修改後確認
SELECT * FROM operation_record WHERE action_project = 'Y發幣' OR content = 'Y發幣' LIMIT 100;

