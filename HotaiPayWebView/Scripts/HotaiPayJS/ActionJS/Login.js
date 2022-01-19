window.onload = function () {
    //獲取元素（兩種方式都可以）
    var input = document.getElementById("demo_input")
    var imgs = document.getElementById('eyes');
    //下面是一個判斷每次點選的效果
    var flag = 0;
    imgs.onclick = function () {
        if (flag == 0) {
            input.type = 'text';
            eyes.src = '../images/eye-regular.svg';//睜眼圖
            flag = 1;
        } else {
            input.type = 'password';
            eyes.src = '../images/eye-slash-regular.svg';//閉眼圖
            flag = 0;
        }
    }
}