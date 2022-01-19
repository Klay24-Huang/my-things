window.onload = function () {
    let inputConfirm = document.getElementById("confirmPwd");
    let nextStep1 = document.getElementById('nextStep1');
    let nextStep2 = document.getElementById('nextStep2');

    inputConfirm.addEventListener('input', updateValue);

    function updateValue(e) {
        nextStep1.disabled = false;
        nextStep1.className = 'btn btn-blue mb-2';
        nextStep2.disabled = false;
        nextStep2.className = 'btn btn-blue mb-2';
    }

    //獲取元素（兩種方式都可以）
    let inputPwd = document.getElementById("demo_input");
    let imgsPwd = document.getElementById("eyes");
    //下面是一個判斷每次點選的效果
    let flagPwd = 0;
    imgsPwd.onclick = function () {
        if (flagPwd == 0) {
            inputPwd.type = 'text';
            inputConfirm.type = 'text';
            eyes.src = '../images/eye-regular.svg';//睜眼圖
            flagPwd = 1;
        } else {
            inputPwd.type = 'password';
            inputConfirm.type = 'password';
            eyes.src = '../images/eye-slash-regular.svg';//閉眼圖
            flagPwd = 0;
        }
    }
}