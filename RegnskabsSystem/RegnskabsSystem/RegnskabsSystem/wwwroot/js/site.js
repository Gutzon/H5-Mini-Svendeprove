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
document.documentElement.addEventListener("load", startFunctions());
function startFunctions() {
    let loggedIn = validateLogin();
    if (loggedIn) {
        populateUsers();
        populateMembers();
        highlightMenu();
        showNavbar(true);
        populateCorporationSelector();
        showFinances();
    }
}


// Navigation
function highlightMenu() {
    let menu = document.getElementsByTagName("navbar")[0];
    for (var navChild of menu.childNodes) {
        if (navChild.localName != "a") continue;
        let linkPath = new URL(navChild.href).pathname;
        if (document.location.pathname == linkPath) addClass(navChild, "activeNav");
    }
}

// General functionality
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
    if (elmType === "button" || elmType == null) return;
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



// Access restrictions
function validateLogin() {
    let loggedIn = validateToken();
    let currentPage = document.location.pathname;
    if (currentPage === "/" && loggedIn) location.href = "/account";
    else if(currentPage === "/") return loggedIn;

    // Redirect to frontpage/login on login restricted pages
    if (!loggedIn) {
        let nonLoginRequiredPages = [];
        if (nonLoginRequiredPages.indexOf(currentPage) == -1) {
            alert("Denne side kræver login, log venligst ind igen.");
            document.location.href = "/";
        }
    }
    return loggedIn;
}

function validateToken() {
    let isLoggedIn = (getCookieParam("accessToken") !== "");
    if (isLoggedIn) {
        let xhr = new XMLHttpRequest();
        xhr.open("GET", '/user/login/validate', true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                // Ignore, as user is still logged in
            }
            else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
                alert("Dit login er udløbet, log venligst på igen.");
                logOut();
            }
        }

        xhr.send();
    }
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
    elm.setAttribute("class", currentClass + (currentClass.length > 0 ? " " : "") + className);
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

function login(e) {
    e.preventDefault();
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

    let formData = getFormJsonData("loginForm");
    xhr.send(JSON.stringify(formData));
}

function handleLogin(responseText) {
    let jsonObject;
    try {
        jsonObject = JSON.parse(responseText);
        setCookieParam("selectedCorp", jsonObject.corporations[0].id);
        setCookieParam("corporations", escape(JSON.stringify(jsonObject.corporations)));
        setCookieParam("accessToken", jsonObject.tokken);
        setCookieParam("userName", loginForm.User.value);
        setCookieParam("user", escape(JSON.stringify(jsonObject.user)));
        document.location.href = "/account";
    }
    catch {
        alert("En fejl skete under login, kontakt venligst en administrator.");
    }
}

function logOut(e, confirmLogout) {
    if(e != null) e.preventDefault();
    let tokenSet = getCookieParam("accessToken");
    if (tokenSet !== "") {
        let xhr = new XMLHttpRequest();
        xhr.open("POST", '/user/logout', true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                removeCookieParam("corporations");
                removeCookieParam("selectedCorp");
                removeCookieParam("accessToken");
                removeCookieParam("userName");
                removeCookieParam("user");
                if (confirmLogout) alert("Du er blevet logget ud.")
                document.location.href = "/";
            }
        }
        xhr.send();
    }
    // Toggle view if for it allowed logout button, though logout had occured (cache/timeout)
    dashboardToggle();
    showNavbar(false);
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
            let corporationChanged = JSON.parse(xhr.responseText);
            if (corporationChanged.changeSuccess) {
                setCookieParam("selectedCorp", corporationSelector.value);
                setPermissions(corporationChanged.permissions);
                populateUsers();
                populateMembers();
                changeAccount();
                    
                removeCookieParam("selectedAcc");
                showFinances();
            }
        }
    }

    xhr.send('{"corporationSelection": ' + corporationSelector.value + '}');
}



// User handling
function populateUsers() {
    let userTableList = document.getElementById("userTable");
    if (userTableList == null) return;

    let xhr = new XMLHttpRequest();
    xhr.open("GET", '/user/overview', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            jsonObject = JSON.parse(xhr.responseText);
            addUsersToOverview(userTableList, jsonObject);
        }
    }
    xhr.send();
}

function setPermissions(permission) {
    let userData = getCookieParam("user");
    if (userData == "") return null;
    let userObject = JSON.parse(userData);
    userObject.permissions = permission;
    setCookieParam("user", JSON.stringify(userObject));
}

function getPermissions() {
    let userData = unescape(getCookieParam("user"));
    if (userData == "") return null;
    return JSON.parse(userData).permissions;
}

function CreateUserColumnElm(user, columnNumber) {
    switch (columnNumber) {
        case 0:
            return document.createTextNode(user["username"]);
        case 1:
            return document.createTextNode(user["mail"]);
        case 2:
            return document.createTextNode(user["firstname"] + " " + user["lastname"]);
        case 3:
            let lastSeenTime = Date.parse(user["lastSeen"]);
            let parsedDate = (new Date());
            parsedDate.setTime(lastSeenTime);
            let lastSeen = parsedDate.toLocaleString() != "1.1.1 00.00.00" ? parsedDate.toLocaleString(): "Ikke logget på endnu";
            return document.createTextNode(lastSeen);
        case 4:
            return !getPermissions().editUser ? null : getEditUserElm(user);
        case 5:
            return !getPermissions().deleteUser ? null : document.createTextNode("");
        default:
            return document.createTextNode("Undefined case");
    }
}

function getEditUserElm(user) {
    let elmHref = document.createElement("a");
    elmHref.setAttribute("href", "/user/edit");
    elmHref.addEventListener("click", function () { userEdit(event, user) });

    let elmImage = document.createElement("img");
    elmImage.setAttribute("src", "/Media/EditIcon.png");
    addClass(elmImage, "tableImgEdit");

    elmHref.appendChild(elmImage);
    return elmHref;
}



function addUsersToOverview(userTableList, userList) {
    let trUsers = userTableList.getElementsByTagName("tr");
    while (trUsers.length > 2) trUsers[2].parentNode.removeChild(trUsers[2]);

    let userCloneRow = userTableList.getElementsByTagName("tr")[1].cloneNode(true);
    cleanUserOverviewElements(userCloneRow);
    removeClass(userCloneRow, "hideElm");

    for (var user of userList) {
        let rowCloned = userCloneRow.cloneNode(true);
        let rowTds = rowCloned.getElementsByTagName("td");

        let removeTds = [];
        for (var i = 0; i < rowTds.length; i++) {
            let objectToAppend = CreateUserColumnElm(user, i);
            if (objectToAppend != null) rowTds[i].appendChild(objectToAppend);
            else removeTds.push(rowTds[i]);
        }

        while (removeTds.length > 0) {
            let td = removeTds.pop();
            td.parentNode.removeChild(td);
        }
        userTableList.appendChild(rowCloned);
    }
}


function cleanUserOverviewElements(userCloneRow) {
    let editHeader = document.getElementById("editUserHeader");
    if (!getPermissions().editUser) {
        editHeader.parentNode.removeChild(editHeader);
    }
    else editHeader.style.display = "table-cell";

    let deleteHeader = document.getElementById("deleteUserHeader");
    if (!getPermissions().deleteUser) {
        deleteHeader.parentNode.removeChild(deleteHeader);
    }
    else deleteHeader.style.display = "table-cell";
}


function performUserCreate(e) {
    e.preventDefault();
    let userCreateForm = document.forms["userCreateForm"];
    if (userCreateForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                jsonObject = JSON.parse(xhr.responseText);

                if (jsonObject.userCreated) {
                    alert("Brugeren blev oprettet");
                    console.log("Midlertidig levering af kodeord, da vi ikke har webhotel på app'en:");
                    console.log(jsonObject.userPassword);
                    hideModal(null, "userCreateSchema");
                    populateUsers();
                }
                else if (jsonObject.error !== "") {
                    switch (jsonObject.error) {
                        case "FailSession":
                            alert("Dit login er udløbet, du logges ud. Log venligst på igen.");
                            logOut();
                            break;
                        case "FailPermission":
                            alert("Du kan ikke tildele de valgte rettigheder.");
                            break;
                        case "FailUserAddRights":
                            alert("Din konto har ikke tilladelse til at tilføje brugere.");
                            break;
                        case "FailUserExists":
                            alert("Brugernavnet er allerede reserveret til en bruger");
                            break;
                        default:
                    }
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





function userEdit(e, user) {
    e.preventDefault();

    let editForm = document.getElementById("userEditForm");
    for (let userParam in user) {

        if (userParam == "permissions") {
            showUserEditPermissions(user[userParam]);
            continue;
        }

        let newUserFormElm = editForm.elements[userParam];
        if (newUserFormElm != undefined) {
            newUserFormElm.value = user[userParam];
        }

        let oldUserElm = document.getElementById("userEdit_" + userParam);
        if (oldUserElm == undefined) continue;
        for (let child of oldUserElm.childNodes) {
            child.parentNode.removeChild(child);
        }
        oldUserElm.appendChild(document.createTextNode(user[userParam]));
    }

    // Assign perform edit function
    let editButton = document.getElementById("performEditButton");
    let clonedButton = editButton.cloneNode(true);
    clonedButton.addEventListener("click", function () { performUserEdit(event, user) });
    let editButtonParent = editButton.parentNode;
    editButtonParent.removeChild(editButton);
    editButtonParent.appendChild(clonedButton);

    showModal(event, 'userEditSchema');
}


function showUserEditPermissions(userPermissions) {
    let permissionTBody = document.getElementById("editUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownData = getCookieParam("user");
    if (ownData == "") logOut(null, true);
    let ownPermissions = JSON.parse(getCookieParam("user")).permissions;

    for (let permission in userPermissions) {
        let permissionRow = permissionTBody.getElementsByTagName("tr")[0].cloneNode(true);
        removeClass(permissionRow, "hideElm");

        let permissionColumns = permissionRow.getElementsByTagName("td");
        permissionColumns[0].appendChild(document.createTextNode(permission)); // Oversæt evt. senere
        permissionColumns[1].appendChild(newPermissionCheckBox("userEditOwn_" + permission, ownPermissions[permission], true, false, false))
        permissionColumns[2].appendChild(newPermissionCheckBox("userEditUser_" + permission, userPermissions[permission], true, false, !ownPermissions[permission]))
        permissionColumns[3].appendChild(newPermissionCheckBox(permission, userPermissions[permission], !ownPermissions[permission], true, false))
        permissionTBody.appendChild(permissionRow)
    }
}

function performUserEdit(e, oldUser) {
    e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/edit', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            if (xhr.responseText == "true") {
                alert("Bruger blev redigeret");
                hideModal(null, "userEditSchema");
                populateUsers();
            }
            else alert("Bruger blev ikke redigeret");
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 500) {
            alert("Redigering af brugeren blev ikke fuldført, en kritisk fejl opstod.")
        }
    }

    let formData = getFormJsonData("userEditForm");
    let userEditObject = { oldUser: oldUser, newUser: formData}
    xhr.send(JSON.stringify(userEditObject));
}




function userCreate(e) {
    e.preventDefault();
    showUserCreatePermissions();

    // Assign perform create function
    let createButton = document.getElementById("performCreateButton");
    let clonedButton = createButton.cloneNode(true);
    clonedButton.addEventListener("click", function () { performUserCreate(event) });
    let createButtonParent = createButton.parentNode;
    createButtonParent.removeChild(createButton);
    createButtonParent.appendChild(clonedButton);

    showModal(event, 'userCreateSchema');
}


function showUserCreatePermissions() {
    let permissionTBody = document.getElementById("createUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownData = getCookieParam("user");
    if (ownData == "") logOut(null, true);
    let ownPermissions = JSON.parse(getCookieParam("user")).permissions;

    for (let permission in ownPermissions) {
        let permissionRow = permissionTBody.getElementsByTagName("tr")[0].cloneNode(true);
        removeClass(permissionRow, "hideElm");

        let permissionColumns = permissionRow.getElementsByTagName("td");
        permissionColumns[0].appendChild(document.createTextNode(permission)); // Oversæt evt. senere
        permissionColumns[1].appendChild(newPermissionCheckBox("userCreateOwn_" + permission, ownPermissions[permission], true, false, false))
        permissionColumns[2].appendChild(newPermissionCheckBox(permission, false, !ownPermissions[permission], true, false))
        permissionTBody.appendChild(permissionRow)
    }
}







function newPermissionCheckBox(elmName, checked, disabled, toPermissionObj, hideDisabled) {
    if (hideDisabled && disabled) return document.createTextNode("Hemmelig");
    let checkbox = document.createElement("input");
    checkbox.setAttribute("type", "checkbox");
    checkbox.setAttribute("name", elmName);
    if (checked) checkbox.setAttribute("checked", "checked");
    if (disabled) checkbox.setAttribute("disabled", "disabled");
    if (toPermissionObj) addClass(checkbox, "ToJsonObjectPermissions");
    return checkbox;
}









// Member handling

function populateMembers() {
    let memberTableList = document.getElementById("memberTable");
    if (memberTableList == null) return;

    let xhr = new XMLHttpRequest();
    xhr.open("GET", '/member/overview', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            jsonObject = JSON.parse(xhr.responseText);
            addMembersToOverview(memberTableList, jsonObject);
        }
    }
    xhr.send();
}

function CreateMemberColumnElm(user, columnNumber) {
    switch (columnNumber) {
        case 0:
            return document.createTextNode(user["firstname"] + " " + user["lastname"]);
        case 1:
            return document.createTextNode(user["mail"]);
        case 2:
            return document.createTextNode(user["phoneNumber"]);
        case 3:
            return !getPermissions().editMember ? null : document.createTextNode("");
        case 4:
            return !getPermissions().deleteMember ? null : document.createTextNode("");
        default:
            return document.createTextNode("Undefined case");
    }
}

function addMembersToOverview(memberTableList, memberList) {
    let trMembers = memberTableList.getElementsByTagName("tr");
    while (trMembers.length > 2) trMembers[2].parentNode.removeChild(trMembers[2]);

    let memberCloneRow = memberTableList.getElementsByTagName("tr")[1].cloneNode(true);
    cleanMemberOverviewElements(memberCloneRow);
    removeClass(memberCloneRow, "hideElm");

    for (var user of memberList) {
        let rowCloned = memberCloneRow.cloneNode(true);
        let rowTds = rowCloned.getElementsByTagName("td");

        let removeTds = [];
        for (var i = 0; i < rowTds.length; i++) {
            let objectToAppend = CreateMemberColumnElm(user, i);
            if (objectToAppend != null) rowTds[i].appendChild(objectToAppend);
            else removeTds.push(rowTds[i]);
        }

        while (removeTds.length > 0) {
            let td = removeTds.pop();
            td.parentNode.removeChild(td);
        }
        memberTableList.appendChild(rowCloned);
    }
}


function cleanMemberOverviewElements(userCloneRow) {
    let editHeader = document.getElementById("editMemberHeader");
    if (!getPermissions().editMember) {
        editHeader.parentNode.removeChild(editHeader);
    }
    else editHeader.style.display = "table-cell";

    let deleteHeader = document.getElementById("deleteMemberHeader");
    if (!getPermissions().deleteMember) {
        deleteHeader.parentNode.removeChild(deleteHeader);
    }
    else deleteHeader.style.display = "table-cell";
}







function memberCreate(e) {
    e.preventDefault();
    console.log(getFormJsonData(memberCreateForm));
    alert("Not ready");
}

function memberEdit(e) {
    e.preventDefault();
    console.log(getFormJsonData(memberEditForm));
    alert("Not ready");
}


// Navigation
function showNavbar(show) {
    let navbar = document.getElementById("headerArea").getElementsByTagName("navbar")[0];
    if (navbar == null) return;
    if (show) removeClass(navbar, "hideElm");
    else addClass(navbar, "hideElm");
}








// Finances
function showFinances() {
    let financeOverview = document.getElementById("financeOverview");
    if (financeOverview == null) return;
    injectAccounts();
}



function changeAccount() {
    let accountSelected = document.getElementById("accountInjection").value;
    setCookieParam("selectedAcc", accountSelected);
    getPostings();
}


function addAccount(e) {
    e.preventDefault();
    let addAccountForm = document.forms["addAccountForm"];
    if (addAccountForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                var successMsg = xhr.responseText;
                alert(successMsg);
            }
            catch {
                alert("En fejl opstod under oprettelse af ny konti");
            }
        }
    }

    let formData = getFormJsonData("addAccountForm");
    xhr.send(JSON.stringify(formData));

}



function injectAccounts() {
    let accountSelect = document.getElementById("accountInjection");
    while (accountSelect.childNodes.length > 0) {
        accountSelect.removeChild(accountSelect.childNodes[0]);
    }

    let xhr = new XMLHttpRequest();
    xhr.open("GET", '/account/accounts', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            jsonObject = JSON.parse(xhr.responseText);
            insertAccounts(jsonObject, accountSelect);
            getPostings();
        }
    }
    xhr.send();
}





function insertAccounts(accounts, accountSelect) {
    let selectedAcc = getCookieParam("selectedAcc");
    for (let account of accounts) {
        let option = document.createElement("option")
        option.appendChild(document.createTextNode(account));
        if (selectedAcc == account) option.setAttribute("Selected", "Selected");
        accountSelect.appendChild(option);
    }
    if (selectedAcc == "") {
        setCookieParam("selectedAcc", accountSelect.value);
    }
}








function changeAccountName(e) {
    e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/change', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            setCookieParam("selectedAcc", getFormJsonData("changeAccountForm").NewAccountName);
            showFinances();
            alert("Ok");
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            alert("Error");
        }
    }

    let formData = getFormJsonData("changeAccountForm");
    formData.AccountName = getCookieParam("selectedAcc");
    xhr.send(JSON.stringify(formData));
}













function getPostings() {

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/overview', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            jsonObject = JSON.parse(xhr.responseText);
            showPostings(jsonObject);
        }
    }
    let accountSelect = {
        "AccountName": getCookieParam("selectedAcc")
    };
    xhr.send(JSON.stringify(accountSelect));
}




function showPostings(postings) {
    let postingHolder = document.getElementById("financeOverview");
    let trPostings = postingHolder.getElementsByTagName("tr");
    while (trPostings.length > 2) trPostings[2].parentNode.removeChild(trPostings[2]);

    let accountSelect = document.getElementById("accountInjection");
    let showSelectedAccount = accountSelect.value == "Main";
    let accountColumn = document.getElementById("accountColumn");

    let postingOrder = ["value", "comment", "konti", "payDate", "byWho", "id"];

    // Remove/add column as needed
    if (showSelectedAccount && accountColumn == null) {
        let insertAccountBefore = document.getElementById("dateColumn");
        let accountElm = document.createElement("td");
        addClass(accountElm, "tableHeaders");
        accountElm.setAttribute("id", "accountColumn");
        accountElm.appendChild(document.createTextNode("Konti"));
        trPostings[0].insertBefore(accountElm, insertAccountBefore);
    }
    else if (!showSelectedAccount && accountColumn != null) {
        accountColumn.parentNode.removeChild(accountColumn);
    }

    let financeSchema = document.getElementById("financeSchema");
    for (let i = 0; i < postings.length; i++) {

        let financeClone = financeSchema.cloneNode(true);
        removeClass(financeClone, "hideElm");

        let financeColumns = financeClone.getElementsByTagName("td");
        let column = 0;

        for (let columnId of postingOrder) {
            if (columnId == "konti" && !showSelectedAccount) {
                financeColumns[column].parentNode.removeChild(financeColumns[column]);
                continue;
            }
            let columnChild = getFinanceChild(postings[i], columnId);
            financeColumns[column++].appendChild(columnChild);
        }

        postingHolder.appendChild(financeClone);
    }
}

function getFinanceChild(posting, columnId) {
    let columnData = posting[columnId];
    if (columnId == "payDate") {
        let parsedDate = (new Date());
        parsedDate.setTime(Date.parse(posting[columnId]));
        let month = ((parsedDate.getMonth()+1).length > 1 ? "" : "0") + (parsedDate.getMonth()+1);
        columnData = parsedDate.getDate() + "/" + month + "-" + parsedDate.getFullYear();
    }

    if (columnId != "value") return document.createTextNode(columnData);

    // Dual data in value section, value and summary
    let valuesHolder = document.createElement("div");

    let valueHolder = document.createElement("div");
    valueHolder.appendChild(document.createTextNode(toFinanceNumber(columnData)));
    if (columnData < 0) valueHolder.style.color = "red";
    valuesHolder.appendChild(valueHolder);

    let sumHolder = document.createElement("div");
    sumHolder.appendChild(document.createTextNode(toFinanceNumber(posting.newSaldo)));
    if (posting.newSaldo < 0) sumHolder.style.color = "red";
    valuesHolder.appendChild(sumHolder);
    return valuesHolder;
}

function toFinanceNumber(value) {
    let toFixedDecimals = value.toFixed(2).replace(".", ",");
    let hasNoDecimals = (toFixedDecimals == value + ",00");
    return hasNoDecimals ? value + ",-" : toFixedDecimals;
}



function addFinance(e) {
    e.preventDefault();
    let addFinanceForm = document.forms["addFinanceForm"];
    if (addFinanceForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/finance', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                var successMsg = xhr.responseText;
                if (successMsg == "ok") {
                    getPostings();
                }
                else switch (successMsg) {
                    case "Wrong Konti name":
                        alert("Der skete en teknisk fejl i forhold til at oprette postering under kontien.");
                            logOut();
                            break;
                        case "not permitted":
                            alert("Du kan ikke tilføje en postering.");
                            break;
                        case "no session":
                            alert("Dit login er udløbet, du logges ud. Log venligst på igen.");
                            logOut();
                            break;
                        default:
                }
            }
            catch {
                alert("En fejl opstod under oprettelse af postering");
            }
        }
    }

    let formData = getFormJsonData("addFinanceForm");
    formData.value = formData.value.replace(",", ".");
    formData.konti = document.getElementById("accountInjection").value;
    xhr.send(JSON.stringify(formData));
}



/* Modals */
let activeModal = null;

window.onresize += resizeModal();

function resizeModal(e) {
    if (activeModal == null) return;
    showModal(null, elmId)
}

function showModal(e, elmId) {
    if (e != null) {
        e.preventDefault();
        activeModal = elmId;
    }
    let modal = document.getElementById(elmId);
    let modalSizeEvaluation = document.getElementById("sizeEvaluation");
    modalSizeEvaluation.appendChild(modal);
    removeClass(modal, "hideElm");

    let modalContent = modal.getElementsByTagName("div")[0];

    modalMarginH = ((100 - Math.round((modalContent.offsetHeight / window.innerHeight) * 100)) / 2);
    if (modalMarginH < 2) modalMarginH = 2;
    modalContent.style.margin = modalMarginH + "vh auto";

    document.body.appendChild(modal);
}


function hideModal(e, elmId) {
    if (e != null) {
        e.preventDefault();
    }
    let modal = document.getElementById(elmId);
    let forms = modal.getElementsByTagName("form");
    for (var form of forms) form.reset();
    addClass(modal, "hideElm");
}