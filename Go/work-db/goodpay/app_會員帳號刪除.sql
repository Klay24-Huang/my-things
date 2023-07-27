.-- 確認資料
SELECT * FROM partners WHERE create_date < "2023-04-01" LIMIT 10000;

-- 刪除
DELETE FROM partners WHERE create_date < "2023-04-01";


-- 刪除後確認
SELECT * FROM partners WHERE create_date < "2023-04-01" LIMIT 10000;

-- 記得要連銀行卡資料一起刪除
-- 記得要連銀行卡資料一起刪除
-- 記得要連銀行卡資料一起刪除



-- https://trello.com/c/9fDmf6mX/228-5minpay%E6%8E%A7%E7%AB%AF-%E6%9C%83%E5%93%A1%E5%B8%B3%E8%99%9F-%E8%88%8A%E8%B3%87%E6%96%99%E6%B8%85%E9%99%A4