-- trello yapay錢包控端-操作紀錄查詢-舊資料顯示更名

-- 檢查資料用
-- SELECT * FROM operation_record WHERE action_project = '會員錢包明細表(223後)' OR content = '會員錢包明細表(223後)' LIMIT 10;
-- SELECT * FROM operation_record WHERE action_project = '會員錢包明細表'  LIMIT 100;
-- SELECT * FROM operation_record WHERE action_project = '會員錢包明細'  LIMIT 100;
-- 
-- SELECT * FROM operation_record WHERE id = 722122;

-- 修改操作細項 和 內容 '會員錢包明細表(223後)' ->  '會員錢包明細'
UPDATE operation_record
SET action_project =  '會員錢包明細表', content = '會員錢包明細表'
WHERE (action_project = '會員錢包明細表(223後)' OR content = '會員錢包明細表(223後)'); 

-- 修改後檢查
SELECT * FROM operation_record WHERE action_object = 'MemberWalletReport' LIMIT 100;
