-- 清除前確認
SELECT COUNT(*) FROM operation_record WHERE create_time < '2023-07-01';
SELECT * FROM operation_record WHERE create_time < '2023-07-01' LIMIT 100;

-- 清除
DELETE FROM operation_record WHERE create_time < '2023-07-01';

-- 清除後確認
SELECT COUNT(*) FROM operation_record WHERE create_time < '2023-07-01';
SELECT * FROM operation_record WHERE create_time < '2023-07-01' LIMIT 100;
SELECT * FROM operation_record ORDER BY create_time LIMIT 100;

-- trello
-- https://trello.com/c/g3tFY35L/214-yapay%E9%8C%A2%E5%8C%85%E6%8E%A7%E7%AB%AF-%E6%93%8D%E4%BD%9C%E7%B4%80%E9%8C%84%E6%9F%A5%E8%A9%A2