// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function login() {
    var loginForm = document.forms["loginForm"];
    if (loginForm == undefined) return;

    var xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/login', true);

    //Send the proper header information along with the request
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () { // Call a function when the state changes.
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            alert("The login token is: " + xhr.responseText);
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            alert("The username and or password did not match");
        }
    }
    //xhr.send(new FormData(document.getElementsByTagName("form")[0]));
    xhr.send('{"user": "' + loginForm.user.value + '", "password": "' + loginForm.password.value + '"}');
}