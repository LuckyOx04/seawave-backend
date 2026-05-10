const form = document.getElementById('resetForm');
const pass = document.getElementById('newPassword');
const confirm = document.getElementById('confirmPassword');
const error = document.getElementById('errorMsg');
const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

form.onsubmit = (e) => {
    if (!regex.test(pass.value) || pass.value !== confirm.value) {
        e.preventDefault();
        error.style.display = 'block';
        return false;
    }
    return true;
};