import { cookie } from './cookie.js';

/**
 * Fetches data using ajax
 * @param {any} method - Define POST/GET
 * @param {any} path - Define path for the request
 * @param {any} bodyData - Define body if applicable
 */
function fetch(method, path, bodyData) {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.open(method, path, true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                try {
                    resolve(xhr.responseText);
                }
                catch {
                    reject("InterpretationError");
                }
            }
            else if (this.readyState === XMLHttpRequest.DONE && this.status !== 200) reject(xhr.responseText);
        }

        if (bodyData === undefined || bodyData == null) xhr.send();
        else xhr.send(bodyData);
    });
}

/**
 * Fetches data using ajax - as an object
 * @param {any} method - Define POST/GET
 * @param {any} path - Define path for the request
 * @param {any} bodyData - Define body if applicable
 */
function fetchData(method, path, bodyData) {
    return fetch(method, path, bodyData)
        .then((objData) => {
            return JSON.parse(objData);
        })
        .catch((error) => {
            helper.errorNotify("hentning af data.", error);
        });
}

/**
 * Fetches data using ajax - returns txt
 * @param {any} method - Define POST/GET
 * @param {any} path - Define path for the request
 * @param {any} bodyData - Define body if applicable
 */
function fetchDataTxt(method, path, bodyData) {
    return fetch(method, path, bodyData);
}


/**
 * Provides a basic error message usable for ex promise rejection.
 * @param {any} activity - Alerted with base message to user
 * @param {any} error - Printed to console for debugging
 */
function errorNotify(activity, error) {
    alert("Der opstod en fejl på serveren under " + activity);
    console.log(error);
}


/**
 * Adds a class to the class attribute
 * @param {any} elm Element
 * @param {any} className Class to add
 */
function addClass(elm, className) {
    let currentClass = elm.getAttribute("class");
    if (currentClass === null) currentClass = "";
    let classParts = currentClass.split(" ");
    for (let i = 0; i < classParts.length; i++) {
        if (classParts[i] === className) {
            return;
        }
    }
    elm.setAttribute("class", currentClass + (currentClass.length > 0 ? " " : "") + className);
}

/**
 * Removes a class to the class attribute
 * @param {any} elm Element
 * @param {any} className Class to remove
 */
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


/**
 * Adds a method for a button (one method only)
 * @param {any} buttonId ButtonId name, InitButton appended
 * @param {any} method Method to run
 */
function addInitButtonEvent(buttonId, method) {
    let elm = document.getElementById(buttonId + "InitButton");
    if (elm == null) return;

    /* Ensure only attaching method once */
    if (elm.getAttribute("class").indexOf("eventAdded") == -1) {
        elm.addEventListener("click", function (e) { method(e) });
        helper.addClass(elm, "eventAdded");
    }
}



/**
 * Get data in form as an object
 * @param {any} formId The form id
 */
function getFormJsonData(formId) {
    var formDataObject = {};
    let form = document.forms[formId];
    if (form == null) return formDataObject;
    for (let elm of document.forms[formId]) {
        placeElmValueInObject(formDataObject, elm);
    }
    return formDataObject
}

function placeElmValueInObject(formDataObject, elm) {
    let elmType = elm.getAttribute("type");
    if (elm.tagName != "SELECT" && (elmType === "button" || elmType == null)) return;
    let disabled = elm.getAttribute("disabled");
    if (disabled != null) return;

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


/**
 * Logs out a user in case a session expired
 * @param {any} e Event
 * @param {any} confirmLogout Whether to alert user of logout
 */
function logOut(e, confirmLogout) {
    if (e != undefined || e != null) e.preventDefault();
    let tokenSet = cookie.get("accessToken");
    if (tokenSet !== "") {
        helper.fetchDataTxt("POST", "/user/logout")
            .then((objData) => {
                cookie.remove("corporations");
                cookie.remove("selectedCorp");
                cookie.remove("accessToken");
                cookie.remove("userName");
                cookie.remove("user");
                if (confirmLogout) alert("Du er blevet logget ud.")
                document.location.href = "/";
            })
            .catch((error) => {
                // A logout is always either successful or error occured due to session was removed already
            });
    }
    showNavbar(false);
}



/**
 * Sets the permissions of the user
 * @param {any} permission The permissions
 */
function setPermissions(permission) {
    let userData = cookie.get("user");
    if (userData == "") return null;
    let userObject = JSON.parse(userData);
    userObject.permissions = permission;
    cookie.set("user", JSON.stringify(userObject));
}



/**
 * Gets the permissions of the user
 */
function getPermissions() {
    let ownData = cookie.get("user");
    if (ownData == "") {
        helper.logOut(null, true);
        return null;
    }
    let ownDataObj = JSON.parse(ownData);
    return ownDataObj.permissions;
}



/**
 * Shows a button depending on user rights
 * @param {any} elmId Element id of button
 * @param {any} givesRights Rights that gives access, admin addCorporation is checked by default
 */
function showButton(elmId, givesRights) {
    let button = document.getElementById(elmId);
    if (hasPermission(givesRights)) helper.removeClass(button, "hideElm");
    else helper.addClass(button, "hideElm");
}

/**
 * Defines whether a user matches permission
 * @param {any} giverRights Rights that gives access, admin addCorporation is checked by default
 */
function hasPermission(giverRights) {
    let ownPermissions = getPermissions();
    let hasRights = (ownPermissions.addCorporation || ownPermissions.admin || hasAMatchingRight(givesRights));
    return hasRights;
}



function hasAMatchingRight(ownRights, givesRights) {
    for (let right of givesRights) {
        if (ownRights[right]) return true;
    }
    return false;
}



export let helper = {
    errorNotify: errorNotify,
    fetchData: fetchData,
    fetchDataTxt: fetchDataTxt,
    addClass: addClass,
    removeClass: removeClass,
    addInitButtonEvent: addInitButtonEvent,
    getFormJsonData: getFormJsonData,
    logOut: logOut,
    showButton: showButton,
    getPermissions: getPermissions,
    setPermissions: setPermissions,
    hasPermission: hasPermission
};