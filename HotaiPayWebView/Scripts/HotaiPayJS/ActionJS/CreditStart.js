
//點擊新增信用卡事件：導向至bind-newcard供使用者選擇綁中信/非中信卡
function goBindNewCard() {

    document.location.href ="/HotaiPay/BindNewCard";
    var Today = new Date();
    alert("今天日期是 " + Today.getFullYear() + " 年 " + (Today.getMonth() + 1) + " 月 " + Today.getDate() + " 日");
}