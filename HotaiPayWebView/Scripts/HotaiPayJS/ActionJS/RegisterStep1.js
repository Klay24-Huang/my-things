window.onload = function () {
    const optCode = document.getElementById('otpCode');
    const nextStep = document.getElementById('nextStep');

    optCode.addEventListener('input', updateValue);

    function updateValue(e) {
        nextStep.disabled = false ;
        nextStep.className = 'btn btn-blue mb-2';
    }
}

    