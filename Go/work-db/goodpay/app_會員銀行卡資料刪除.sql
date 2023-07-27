-- 確認
SELECT COUNT(*) FROM partner_bank_cards;

SELECT 
pbc.id 
FROM partner_bank_cards pbc 
LEFT JOIN partners p ON p.id = pbc.partner_id
WHERE p.id IS NULL;

-- 刪除partner 不存在的 5min 會員 銀行卡資料
DELETE FROM partner_bank_cards WHERE id IN (
SELECT 
pbc.id 
FROM partner_bank_cards pbc 
LEFT JOIN partners p ON p.id = pbc.partner_id
WHERE p.id IS NULL
);

-- 刪除後確認確認
SELECT 
* 
FROM partner_bank_cards pbc 
LEFT JOIN partners p ON p.id = pbc.partner_id
WHERE p.id IS NULL;

-- https://trello.com/c/oJsJonHc/288-5minpay%E6%8E%A7%E7%AB%AF-%E8%B3%87%E6%96%99%E5%BA%AB%E9%AB%92%E8%B3%87%E6%96%99%E5%88%AA%E9%99%A4%E5%BE%8C%EF%BC%8C%E5%B0%8D%E6%87%89%E7%9A%84%E9%8A%80%E8%A1%8C%E5%8D%A1%E8%B3%87%E6%96%99%E6%9C%AA%E5%88%AA%E9%99%A4