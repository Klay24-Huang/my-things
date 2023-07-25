-- https://trello.com/c/9viSDqrQ/220-yapay%E9%8C%A2%E5%8C%85%E6%8E%A7%E7%AB%AF-%E5%AF%A9%E6%A0%B8%E5%8A%9F%E8%83%BD%E5%88%97%E8%A1%A8-%E7%A7%BB%E9%99%A4%E3%80%90%E4%BF%AE%E6%94%B9%E6%9C%83%E5%93%A1%E6%8E%92%E5%BA%8F%E5%80%BC%E3%80%91

-- 修改前確認
SELECT * FROM approval_event WHERE tag = "會員列表" AND title = "修改會員排序值";

-- 刪除
DELETE FROM approval_event WHERE tag = "會員列表" AND title = "修改會員排序值";

-- 修改後確認
SELECT * FROM approval_event WHERE tag = "會員列表" AND title = "修改會員排序值";