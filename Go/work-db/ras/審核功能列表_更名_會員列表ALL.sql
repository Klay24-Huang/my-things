-- 修改前確認
SELECT * FROM approval_event WHERE tag = "會員列表All" AND title = "重置實名驗證審核";

-- 修改
UPDATE approval_event
SET tag = "會員列表", title = "重置實名驗證"
WHERE tag = "會員列表All" AND title = "重置實名驗證審核";

-- 修改後確認
SELECT * FROM approval_event WHERE tag = "會員列表" AND title = "重置實名驗證";

-- https://trello.com/c/pb92stMy/221-yapay%E9%8C%A2%E5%8C%85%E6%8E%A7%E7%AB%AF-%E5%AF%A9%E6%A0%B8%E5%8A%9F%E8%83%BD%E5%88%97%E8%A1%A8-%E6%9B%B4%E5%90%8D%E3%80%90%E6%9C%83%E5%93%A1%E5%88%97%E8%A1%A8all%E3%80%91