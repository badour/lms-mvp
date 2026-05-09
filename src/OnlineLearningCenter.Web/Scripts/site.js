(function () {
    var countdownEl = document.querySelector('[data-exam-countdown]');
    if (!countdownEl) {
        return;
    }

    var durationMinutes = Number(countdownEl.getAttribute('data-duration-minutes') || '60');
    var remaining = durationMinutes * 60;

    function renderCountdown() {
        var minutes = Math.floor(remaining / 60);
        var seconds = remaining % 60;
        countdownEl.textContent = String(minutes).padStart(2, '0') + ':' + String(seconds).padStart(2, '0');

        if (remaining <= 300) {
            countdownEl.classList.add('text-danger');
        }
    }

    renderCountdown();
    var timer = setInterval(function () {
        remaining -= 1;
        renderCountdown();

        if (remaining <= 0) {
            clearInterval(timer);
            alert('Exam time has expired. The session will be auto-submitted.');
        }
    }, 1000);
})();
