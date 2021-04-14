// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Rephrase below for report
// Note: Individual functions should later be moved to files indicating usage.
// - As we try to use HTML as frontend, we are putting most of the
// communication with the API directly in JS instead of using the backend C# to fetch data.
// If we later move to Blazor or ASP pages or decide to facilitate the API which the pages are
// served using directly, we can avoid some of the JS files.
// This is a team decision to learn more about datahandling in frontend.

// Run start functions
document.documentElement.onload += startFunctions();
function startFunctions() {
    validateLogin();
}



// General functionality
function GetFormJsonData(formId) {
    var formDataObject = {};
    for (let elm of document.forms[formId]) {
        if (elm.getAttribute("type") !== "button") {
            let elmName = elm.getAttribute("name");
            if (elmName !== undefined) {
                formDataObject[elmName] = elm.value;
            }
        }
    }
    return formDataObject
}



// Access restrictions
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
            handleLogin(xhr.responseText);
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            if (xhr.responseText === "AccessDenied") alert("Brugernavn/kodeord matchede ikke.");
            else if (xhr.responseText === "AccountFail") alert("Der er fejl på brugerkonti, kontakt administrator.");
            logOut();
        }
    }
    xhr.send('{"user": "' + loginForm.user.value + '", "password": "' + escape(loginForm.password.value) + '"}');
}

function handleLogin(responseText) {
    let jsonObject;
    try {
        jsonObject = JSON.parse(responseText);
        if (jsonObject.status === "Select") {
            alert("Select");
            // TEMP
            setCookieParam("accessToken", jsonObject.tokken);
            setCookieParam("userName", loginForm.user.value);
            dashboardToggle();
        }
        else {
            setCookieParam("accessToken", jsonObject.tokken);
            setCookieParam("userName", loginForm.user.value);
            dashboardToggle();
        }
    }
    catch {
        alert("En fejl skete under login, kontakt venligst en administrator.");
    }
}

function logOut() {
    let tokenSet = getCookieParam("accessToken");
    if (tokenSet !== "") {
        let xhr = new XMLHttpRequest();
        xhr.open("POST", '/user/logout', true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                alert("Log ud er gennemført");
                removeCookieParam("accessToken");
                removeCookieParam("userName");
                document.location.href = "/";
            }
        }
        xhr.send();
    }
    // Toggle view if for it allowed logout button, though logout had occured (cache/timeout)
    dashboardToggle();
}



// User handling
function UserCreate() {
    let userCreateForm = document.forms["userCreateForm"];
    if (userCreateForm == undefined) return;
    
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            var userCreated = (xhr.responseText.toLowerCase() === "true");
            var userCreationMessage = userCreated ? "Brugeren blev oprettet" : "Brugeren kunne ikke oprettes";
            alert(userCreationMessage);
        }
    }

    let formData = GetFormJsonData("userCreateForm");
    xhr.send(JSON.stringify(formData));
}