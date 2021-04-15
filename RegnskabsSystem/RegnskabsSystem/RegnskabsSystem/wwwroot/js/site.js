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
function getFormJsonData(formId) {
    var formDataObject = {};
    let form = document.forms[formId];
    if (form == null) return formDataObject;
    for (let elm of document.forms[formId]) {
        placeElmValueInObject(formDataObject, elm);
    }
    console.log(formDataObject); // <- For debugging purposes in console.
    return formDataObject
}

function placeElmValueInObject(formDataObject, elm) {
    let elmType = elm.getAttribute("type");
    if (elmType === "button") return;

    let elmName = elm.getAttribute("name");
    if (elmName === undefined) return;

    let subObjectPosition = getJsonObjectForValue(elm);
    if (subObjectPosition.length > 0) {
        for (let pos of subObjectPosition) {
            if (formDataObject[pos] == undefined) formDataObject[pos] = {};
            formDataObject[pos][elmName] = (elmType === "checkbox") ? elm.checked : elm.value;
        }
    }
    else {
        formDataObject[elmName] = (elmType === "checkbox") ? elm.checked : elm.value;
    }
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

function getJsonObjectForValue(elm) {
    let jsonObjectToPlace = [];
    let currentClass = elm.getAttribute("class");
    if (currentClass === null) currentClass = "";
    let classParts = currentClass.split(" ");
    for (let i = 0; i < classParts.length; i++) {
        if (classParts[i].indexOf("ToJsonObject") != 0) {
            continue;
        }
        jsonObjectToPlace.push(classParts[i].substring("ToJsonObject".length));
    }
    return jsonObjectToPlace;
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
    if (loggedIn) populateCorporationSelector();
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
        setCookieParam("selectedCorp", jsonObject.corporations[0].id);
        setCookieParam("corporations", escape(JSON.stringify(jsonObject.corporations)));
        setCookieParam("accessToken", jsonObject.tokken);
        setCookieParam("userName", loginForm.user.value);
        dashboardToggle();
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
                removeCookieParam("corporations");
                removeCookieParam("selectedCorp");
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



// Corporation handling
function populateCorporationSelector() {
    let corporationSelector = document.getElementById("corporationInjection");
    while (corporationSelector.childNodes.length > 0) {
        corporationSelector.removeChild(corporationSelector.childNodes[0]);
    }
    let corporations = JSON.parse(getCookieParam("corporations"));
    let selectedCorp = getCookieParam("selectedCorp");
    if (corporations.length == 1) {
        corporationSelector.appendChild(document.createTextNode(corporations[0].name));
    }
    else {
        let select = document.createElement("select");
        select.setAttribute("id", "corporationSelector");
        select.setAttribute("class", "selectors");
        select.addEventListener("change", () => changeCorporation());

        for (let corp of corporations) {
            let option = document.createElement("option")
            option.appendChild(document.createTextNode(corp.name));
            option.setAttribute("value", corp.id);
            if (selectedCorp == corp.id) option.setAttribute("Selected", "Selected");
            select.appendChild(option);
        }
        corporationSelector.appendChild(select);
    }
}

function changeCorporation() {
    let corporationSelector = document.getElementById("corporationSelector");

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/corporation/change', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            let corporationChanged = (xhr.responseText.toLowerCase() === "true");
            alert("Organisationen blev " + (!corporationChanged ? "ikke " : "") + "skiftet");
            if (corporationChanged) {
                setCookieParam("selectedCorp", corporationSelector.value);
            }
        }
    }

    xhr.send('{"corporationSelection": ' + corporationSelector.value + '}');
}



// User handling

function populateUsers() {
    let userTableList = document.getElementById("userTableList");
    
    let xhr = new XMLHttpRequest();
    xhr.open("GET", '/user/overview', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            jsonObject = JSON.parse(xhr.responseText);
            console.log(jsonObject);
        }
    }
    xhr.send();
}



function userCreate() {
    let userCreateForm = document.forms["userCreateForm"];
    if (userCreateForm == undefined) return;
    
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                jsonObject = JSON.parse(xhr.responseText);
                var userCreationMessage = jsonObject.userCreated ? "Brugeren blev oprettet" : "Brugeren kunne ikke oprettes";
                alert(userCreationMessage);
                if (jsonObject.userCreated) {
                    console.log("Midlertidig levering af kodeord, da vi ikke har webhotel på app'en:");
                    console.log(jsonObject.userPassword);
                }
                else if (jsonObject.TokenExpired) {
                    alert("Dit login er udløbet, log på igen.");
                    logOut();
                }
            }
            catch {
                alert("En fejl opstod under bruger oprettelsen");
            }
        }
    }

    let formData = getFormJsonData("userCreateForm");
    xhr.send(JSON.stringify(formData));
}

function userEdit() {
    //console.log(getFormJsonData(userEditForm));
    alert("Not ready");
}









// Member handling
function memberCreate() {
    console.log(getFormJsonData(memberCreateForm));
    alert("Not ready");
}

function memberEdit() {
    console.log(getFormJsonData(memberEditForm));
    alert("Not ready");
}