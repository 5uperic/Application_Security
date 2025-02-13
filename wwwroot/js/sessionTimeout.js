let timeout;

function resetTimer() {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        fetch('/Logout', { method: 'POST' }) // Trigger server-side logout
            .then(() => {
                window.location.href = "/Login"; // Redirect to login page after logout
            });
    }, 30 * 60 * 1000);
}


// document.addEventListener("mousemove", resetTimer);
// document.addEventListener("keypress", resetTimer);

resetTimer(); // Start the timer