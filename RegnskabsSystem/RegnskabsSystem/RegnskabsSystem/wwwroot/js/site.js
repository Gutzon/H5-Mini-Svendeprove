// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Rephrase below for report
// Note: Individual functions should later be moved to files indicating usage.
// - As we try to use HTML as frontend, we are putting most of the
// communication with the API directly in JS instead of using the backend C# to fetch data.
// If we later move to Blazor or ASP pages or decide to facilitate the API which the pages are
// served using directly, we can avoid some of the JS files.




// Access restrictions
document.documentElement.onload += validateLogin();
function validateLogin() {
    let currentPage = document.location.pathname;
    if (currentPage === "/") {
        dashboardToggle();
        return;
    }

    // Redirect to frontpage/login on login restricted pages
    if (!validateToken()) {
        let nonLoginRequiredPages = ["/Api/Privacy"];
        if (nonLoginRequiredPages.indexOf(currentPage) == -1) {
            alert("This page requires login. Please login again.");
            document.location.href = "/";
        }
    }            
}

function validateToken() {
    let isLoggedIn = (getCookieParam("accessToken") !== "");
    // Todo: Change to query server on each query
    return isLoggedIn;
}



// Class functionality
function addClass(elm, className) {
    let currentClass = elm.getAttribute("class");
    if (currentClass === null) currentClass = "";
    let classParts = currentClass.split(" ");
    for (let i = 0; i < classParts.length; i++) {
        if (classParts[i] === className) {
            return;
        }
    }
    elm.setAttribute("class", currentClass + " " + className);
}

function removeClass(elm, className) {
    let newClass = "";
    let currentClass = elm.getAttribute("class");
    if (currentClass === null) currentClass = "";
    let classParts = currentClass.split(" ");
    for (let i = 0; i < classParts.length; i++) {
        if (classParts[i] === className) {
            continue;
        }
        newClass += (newClass.length > 0 ? " " : "") + classParts[i]
    }
    elm.setAttribute("class", newClass);
}



// Cookie functions
function setCookieParam(param, value) {
    // Currently no expiration implemented for cookie params set
    document.cookie = param + "=" + value;
}

function getCookieParam(param) {
    let paramIdentifier = param + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let cookieParts = decodedCookie.split(';');
    for (let i = 0; i < cookieParts.length; i++) {
        let cookieParamText = cookieParts[i];
        while (cookieParamText.charAt(0) == ' ') {
            cookieParamText = cookieParamText.substring(1);
        }
        if (cookieParamText.indexOf(paramIdentifier) == 0) {
            return cookieParamText.substring(paramIdentifier.length, cookieParamText.length);
        }
    }
    return "";
}

function removeCookieParam(param) {
    document.cookie = param + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}



// Log-in/out functionality
function dashboardToggle() {
    let loggedIn = (getCookieParam("accessToken") !== "");
    let activeView = document.getElementById(loggedIn ? "loggedIn" : "loggedOut");
    if (activeView == null) {
        return; // Not on main landing page (login/dashboard)
    }
    removeClass(activeView, "hideElm");
    let inActiveView = document.getElementById(loggedIn ? "loggedOut" : "loggedIn");
    addClass(inActiveView, "hideElm");
}

function login() {
    let loginForm = document.forms["loginForm"];
    if (loginForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/login', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            setCookieParam("accessToken", xhr.responseText);
            dashboardToggle();
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            alert("The username and or password did not match");
            logOut();
        }
    }
    xhr.send('{"user": "' + loginForm.user.value + '", "password": "' + loginForm.password.value + '"}');
}

function logOut() {
    let tokenSet = getCookieParam("accessToken");
    if (tokenSet !== "") {
        removeCookieParam("accessToken");
        dashboardToggle();
    }
}




// User handling
function UserEdit() {
    let userEditForm = document.forms["userEditForm"];
    if (userEditForm == undefined) return;
    let userId = userEditForm.userId.value;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/edit/' + userId, true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            alert(xhr.responseText);
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            alert(xhr.responseText);
        }
    }
    xhr.send(new FormData(userEditForm));
}