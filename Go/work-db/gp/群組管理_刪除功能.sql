-- 確認
SELECT * FROM object WHERE NAME LIKE "每日%" OR NAME LIKE "商%";
SELECT * FROM object WHERE CODE = "DailyReport";
SELECT * FROM object WHERE NAME = "CouponCarousel";

-- 刪除
DELETE FROM object WHERE CODE = "DailyReport";
DELETE FROM object WHERE CODE = "CouponCarousel";

-- 刪除後確認
SELECT * FROM object WHERE CODE = "DailyReport";
SELECT * FROM object WHERE NAME = "CouponCarousel";



-- https://trello.com/c/c5kH1WEM/283-yapay%E9%8C%A2%E5%8C%85%E6%8E%A7%E7%AB%AF-%E7%BE%A4%E7%B5%84%E7%AE%A1%E7%90%86-%E5%8A%9F%E8%83%BD%E7%A7%BB%E9%99%A4