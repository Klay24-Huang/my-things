-- 確認資料
SELECT * FROM log_manager_transfers LIMIT 100;

-- 刪除
DELETE  FROM log_manager_transfers WHERE id > 0;

-- 確認資料
SELECT * FROM log_manager_transfers LIMIT 100;

-- https://trello.com/c/gIOMvJIV/227-5minpay%E6%8E%A7%E7%AB%AF-%E6%94%B6%E5%9B%9Em%E5%B9%A3-%E8%88%8A%E8%B3%87%E6%96%99%E6%B8%85%E9%99%A4