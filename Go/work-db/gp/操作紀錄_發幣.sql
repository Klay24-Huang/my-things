-- 發幣
-- 確認原始資料
SELECT * FROM operation_record WHERE action_project = 'Y&U發幣' OR content = 'Y&U發幣' LIMIT 100;

--  修改
UPDATE operation_record
SET action_project =  'Y發幣', content = 'Y發幣'
WHERE (action_project = 'Y&U發幣' OR content = 'Y&U發幣');

-- 修改後確認
SELECT * FROM operation_record WHERE action_project = 'Y發幣' OR content = 'Y發幣' LIMIT 100;

-- trello
-- https://trello.com/c/g3tFY35L/214-yapay%E9%8C%A2%E5%8C%85%E6%8E%A7%E7%AB%AF-%E6%93%8D%E4%BD%9C%E7%B4%80%E9%8C%84%E6%9F%A5%E8%A9%A2