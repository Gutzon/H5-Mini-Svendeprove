import { helper } from './helper.js';
import { modal } from './modal.js';
import { inventory } from './inventory.js';
import { member } from './member.js';
import { dataPopulator } from './data-populator.js';
import { cookie } from './cookie.js';



document.documentElement.addEventListener("load", startFunctions());
function startFunctions() {
    let loggedIn = validateLogin();
    if (loggedIn) {
        populateUsers();
        highlightMenu();
        showNavbar(true);
        populateCorporationSelector();
        showFinances();

        attachElmEvents();

        /* Refactored */
        member.show();
        inventory.show();
    }
}

function attachElmEvents() {

    /* use add event listener for some these - move others to methods
     * window.login = login;
window.showEditAccount = showEditAccount;
window.showAddAccount = showAddAccount;
window.changeAccount = changeAccount;
window.showAddFinance = showAddFinance;
window.getRepFinance = getRepFinance;
window.addAccount = addAccount;
window.hide = modal.hide;
window.changeAccountName = changeAccountName;
window.createRepFinance = createRepFinance;
window.addFinance = addFinance;
window.helper.logOut = helper.logOut;
window.memberCreate = memberCreate;
window.userCreate = userCreate;*/

    /*
    let elm = document.getElementById("inventoryCreateInitButton");
    if (elm != null) {
        elm.addEventListener("click", function (e) { inventory.create(e) });
    }*/
}




// Navigation
function highlightMenu() {
    let menu = document.getElementsByTagName("navbar")[0];
    for (var navChild of menu.childNodes) {
        if (navChild.localName != "a") continue;
        let linkPath = new URL(navChild.href).pathname;
        if (document.location.pathname == linkPath) helper.addClass(navChild, "activeNav");
    }
}




// Access restrictions
function validateLogin() {
    let loggedIn = validateToken();
    let currentPage = document.location.pathname;
    if (currentPage === "/" && loggedIn) location.href = "/account";
    else if (currentPage === "/") return loggedIn;

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
    let isLoggedIn = (cookie.get("accessToken") !== "");
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
                helper.logOut();
            }
        }

        xhr.send();
    }
    return isLoggedIn;
}




function login(e) {
    if (e != undefined) e.preventDefault();
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
            helper.logOut();
        }
    }

    let formData = helper.getFormJsonData("loginForm");
    xhr.send(JSON.stringify(formData));
}

function handleLogin(responseText) {
    let jsonObject;
    try {
        jsonObject = JSON.parse(responseText);
        cookie.set("selectedCorp", jsonObject.corporations[0].id);
        cookie.set("corporations", escape(JSON.stringify(jsonObject.corporations)));
        cookie.set("accessToken", jsonObject.tokken);
        cookie.set("userName", loginForm.User.value);
        cookie.set("user", escape(JSON.stringify(jsonObject.user)));
        document.location.href = "/account";
    }
    catch {
        alert("En fejl skete under login, kontakt venligst en administrator.");
    }
}



// Corporation handling
function populateCorporationSelector() {
    let corporationSelector = document.getElementById("corporationInjection");
    while (corporationSelector.childNodes.length > 0) {
        corporationSelector.removeChild(corporationSelector.childNodes[0]);
    }
    let corporations = JSON.parse(cookie.get("corporations"));
    let selectedCorp = cookie.get("selectedCorp");
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
                cookie.set("selectedCorp", corporationSelector.value);
                setPermissions(corporationChanged.permissions);
                populateUsers();
                changeAccount();

                cookie.remove("selectedAcc");
                showFinances();

                member.show();
                inventory.show();
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
            let jsonObject = JSON.parse(xhr.responseText);
            addUsersToOverview(userTableList, jsonObject);
        }
    }
    xhr.send();
}

function setPermissions(permission) {
    let userData = cookie.get("user");
    if (userData == "") return null;
    let userObject = JSON.parse(userData);
    userObject.permissions = permission;
    cookie.set("user", JSON.stringify(userObject));
}

function getPermissions() {
    let userData = unescape(cookie.get("user"));
    if (userData == "") return null;
    return JSON.parse(userData).permissions;
}

function CreateUserColumnElm(user, columnNumber, hasEditRights, hasDeleteRightsOfUser, hasDeleteRightsGeneral) {
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
            let lastSeen = parsedDate.toLocaleString() != "1.1.1 00.00.00" ? parsedDate.toLocaleString() : "Ikke logget på endnu";
            return document.createTextNode(lastSeen);
        case 4:
            return !hasEditRights ? document.createTextNode("") : getEditUserElm(user);
        case 5:
            return !hasDeleteRightsOfUser ? (hasDeleteRightsGeneral ? document.createTextNode("") : null) : getDeleteUserElm(user);
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
    helper.addClass(elmImage, "tableImgEdit");

    elmHref.appendChild(elmImage);
    return elmHref;
}

function getDeleteUserElm(user) {
    let elmHref = document.createElement("a");
    elmHref.setAttribute("href", "/user/delete");
    elmHref.addEventListener("click", function () { deleteUser(event, user) });
    let elmImage = document.createElement("img");
    elmImage.setAttribute("src", "/Media/DeleteIcon.png");
    helper.addClass(elmImage, "tableImgDelete");

    elmHref.appendChild(elmImage);
    return elmHref;
}



function addUsersToOverview(userTableList, userList) {
    let trUsers = userTableList.getElementsByTagName("tr");
    while (trUsers.length > 2) trUsers[2].parentNode.removeChild(trUsers[2]);

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownDataObj = JSON.parse(ownData);
    let ownPermissions = ownDataObj.permissions;
    let hasDeleteRightsGeneral = (ownPermissions.addCorporation || ownPermissions.admin || ownPermissions.deleteUser);

    let userCloneRow = userTableList.getElementsByTagName("tr")[1].cloneNode(true);
    cleanUserOverviewElements(userCloneRow, hasDeleteRightsGeneral);
    helper.removeClass(userCloneRow, "hideElm");

    let hasAddRightsGeneral = (ownPermissions.addCorporation || ownPermissions.admin || ownPermissions.addUser);
    let addUserButton = document.getElementById("addUserButton");
    if (hasAddRightsGeneral) {
        helper.removeClass(addUserButton, "hideElm");
    }
    else helper.addClass(addUserButton, "hideElm");

    for (var user of userList) {
        // Rights evaluation on user protection to avoid showing edit/delete where not applicable
        let hasEditRights = (ownPermissions.addCorporation
            || ownPermissions.admin && !user["permissions"]["addCorporation"]
            || ownPermissions.editUser && !user["permissions"]["admin"]
            || user["username"] == ownDataObj["username"]);

        let hasDeleteRightsOfUser = user["username"] != ownDataObj["username"] && (ownPermissions.addCorporation
            || ownPermissions.admin && !user["permissions"]["addCorporation"]
            || ownPermissions.deleteUser && !user["permissions"]["admin"]);

        let rowCloned = userCloneRow.cloneNode(true);
        let rowTds = rowCloned.getElementsByTagName("td");

        let removeTds = [];
        for (var i = 0; i < rowTds.length; i++) {
            let objectToAppend = CreateUserColumnElm(user, i, hasEditRights, hasDeleteRightsOfUser, hasDeleteRightsGeneral);
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


function cleanUserOverviewElements(userCloneRow, hasDeleteRightsGeneral) {
    let deleteHeader = document.getElementById("deleteUserHeader");
    if (deleteHeader == undefined) return;
    if (!hasDeleteRightsGeneral) {
        deleteHeader.parentNode.removeChild(deleteHeader);
    }
    else deleteHeader.style.display = "table-cell";
}


function performUserCreate(e) {
    if (e != undefined) e.preventDefault();
    let userCreateForm = document.forms["userCreateForm"];
    if (userCreateForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                let jsonObject = JSON.parse(xhr.responseText);

                if (jsonObject.userCreated) {
                    alert("Brugeren blev oprettet");
                    console.log("Midlertidig levering af kodeord, da vi ikke har webhotel på app'en:");
                    console.log(jsonObject.userPassword);
                    modal.hide(null, "userCreateSchema");
                    populateUsers();
                }
                else if (jsonObject.error !== "") {
                    switch (jsonObject.error) {
                        case "FailSession":
                            alert("Dit login er udløbet, du logges ud. Log venligst på igen.");
                            helper.logOut();
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

    let formData = helper.getFormJsonData("userCreateForm");
    xhr.send(JSON.stringify(formData));
}





function userEdit(e, user) {
    if (e != undefined) e.preventDefault();

    let editForm = document.getElementById("userEditForm");

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownDataObj = JSON.parse(ownData);
    let ownPermissions = ownDataObj.permissions;
    let hasPasswordEditPermission = (ownPermissions.addCorporation || ownPermissions.admin ||
        user["username"] == ownDataObj["username"]
    );

    let passwordEditBox = document.getElementById("editUserPassword");
    if (hasPasswordEditPermission) helper.removeClass(passwordEditBox, "hideElm");
    else helper.addClass(passwordEditBox, "hideElm");

    for (let userParam in user) {


        if (userParam == "permissions") {
            showUserEditPermissions(user["username"], user[userParam]);
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

    modal.show(event, 'userEditSchema');
}


function showUserEditPermissions(username, userPermissions) {
    let permissionTBody = document.getElementById("editUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownDataObj = JSON.parse(ownData);
    let ownPermissions = ownDataObj.permissions;

    for (let permission in userPermissions) {
        let permissionRow = permissionTBody.getElementsByTagName("tr")[0].cloneNode(true);
        helper.removeClass(permissionRow, "hideElm");

        let hasRightToEditPermission = (ownPermissions.addCorporation
            || ownPermissions.admin && permission != "addCorporation"
            || ownPermissions[permission]
            || username == ownDataObj["username"]);

        let permissionColumns = permissionRow.getElementsByTagName("td");
        permissionColumns[0].appendChild(document.createTextNode(permission)); // Oversæt evt. senere
        permissionColumns[1].appendChild(newPermissionCheckBox("userEditOwn_" + permission, hasRightToEditPermission, true, false, false))
        permissionColumns[2].appendChild(newPermissionCheckBox("userEditUser_" + permission, userPermissions[permission], true, false, !hasRightToEditPermission))
        permissionColumns[3].appendChild(newPermissionCheckBox(permission, userPermissions[permission], !hasRightToEditPermission, true, false))
        permissionTBody.appendChild(permissionRow)
    }
}

function performUserEdit(e, oldUser) {
    if (e != undefined) e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/edit', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {

            let ownData = cookie.get("user");
            if (ownData == "") helper.logOut(null, true);
            let ownDataObj = JSON.parse(ownData);
            let editedSelf = oldUser["username"] == ownDataObj["username"];

            if (xhr.responseText == "true") {
                if (editedSelf) {
                    alert("Din egen bruger blev redigeret, du logges nu af - log venligst på igen");
                    helper.logOut(null, false);
                }
                alert("Bruger blev redigeret");
                modal.hide(null, "userEditSchema");
                populateUsers();
            }
            else alert("Bruger blev ikke redigeret");
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 500) {
            alert("Redigering af brugeren blev ikke fuldført, en kritisk fejl opstod.")
        }
    }

    let formData = helper.getFormJsonData("userEditForm");
    let userEditObject = { oldUser: oldUser, newUser: formData }
    xhr.send(JSON.stringify(userEditObject));
}




function userCreate(e) {
    if (e != undefined) e.preventDefault();
    showUserCreatePermissions();

    // Assign perform create function
    let createButton = document.getElementById("performCreateButton");
    let clonedButton = createButton.cloneNode(true);
    clonedButton.addEventListener("click", function () { performUserCreate(event) });
    let createButtonParent = createButton.parentNode;
    createButtonParent.removeChild(createButton);
    createButtonParent.appendChild(clonedButton);

    modal.show(event, 'userCreateSchema');
}


function showUserCreatePermissions() {
    let permissionTBody = document.getElementById("createUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownPermissions = JSON.parse(cookie.get("user")).permissions;

    for (let permission in ownPermissions) {
        let permissionRow = permissionTBody.getElementsByTagName("tr")[0].cloneNode(true);
        helper.removeClass(permissionRow, "hideElm");

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
    if (toPermissionObj) helper.addClass(checkbox, "ToJsonObjectPermissions");
    return checkbox;
}










// Navigation
function showNavbar(show) {
    let navbar = document.getElementById("headerArea").getElementsByTagName("navbar")[0];
    if (navbar == null) return;
    if (show) helper.removeClass(navbar, "hideElm");
    else helper.addClass(navbar, "hideElm");
}








// Finances
function showFinances() {
    let financeOverview = document.getElementById("financeOverview");
    if (financeOverview == null) return;
    injectAccounts();
}



function changeAccount() {
    let accountElm = document.getElementById("accountInjection");
    if (accountElm == undefined) {
        cookie.remove("selectedAcc");
        return;
    }
    let accountSelected = accountElm.value;
    cookie.set("selectedAcc", accountSelected);
    getPostings();
}


function addAccount(e) {
    if (e != undefined) e.preventDefault();
    let addAccountForm = document.forms["addAccountForm"];
    if (addAccountForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                var successMsg = xhr.responseText;
                if (successMsg == "OK") {
                    alert("Den nye konti er nu oprettet");
                    modal.hide(null, "createAccountSchema");
                }
                switch (successMsg) {
                    case "Alredy exist":
                        alert("Kontoen eksisterer i forvejen");
                        break;
                    case "Not permitted":
                        alert("Du har ikke tilladelse til at oprette konti");
                        break;
                    case "No session":
                        alert("Din session er udløbet, du logges af - log på igen");
                        helper.logOut(null, false);
                        break;
                    default:
                }
                showFinances();
            }
            catch {
                alert("En fejl opstod under oprettelse af ny konti");
            }
        }
    }

    let formData = helper.getFormJsonData("addAccountForm");
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
            let jsonObject = JSON.parse(xhr.responseText);
            insertAccounts(jsonObject, accountSelect);
            getPostings();
        }
    }
    xhr.send();
}





function insertAccounts(accounts, accountSelect) {
    let selectedAcc = cookie.get("selectedAcc");
    for (let account of accounts) {
        let option = document.createElement("option")
        option.appendChild(document.createTextNode(account));
        if (selectedAcc == account) option.setAttribute("Selected", "Selected");
        accountSelect.appendChild(option);
    }
    if (selectedAcc == "") {
        cookie.set("selectedAcc", accountSelect.value);
    }
}



function showAddAccount(e) {
    if (e != undefined) e.preventDefault();
    modal.show(null, "createAccountSchema");
}


function showEditAccount(e) {
    if (e != undefined) e.preventDefault();
    let editForm = document.getElementById("accountEditForm");
    let accountChosen = cookie.get("selectedAcc");
    editForm.elements["AccountName"].value = accountChosen;
    editForm.elements["NewAccountName"].value = accountChosen;
    modal.show(null, "editAccountSchema");
}

function changeAccountName(e) {
    if (e != undefined) e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/change', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            alert("Konti navnet blev rettet");
            cookie.set("selectedAcc", helper.getFormJsonData("accountEditForm").NewAccountName);
            modal.hide(null, "editAccountSchema");
            showFinances();
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 400) {
            switch (xhr.responseText) {
                case "ErrorMainAccount":
                    alert("Du kan ikke rette navnet på hoved kontien");
                    break;
                case "Name problem":
                    alert("Navnet på kontien der skulle rettes kunne ikke matches");
                    break;
                case "Not permitted":
                    alert("Du har ikke tilladelsen til at rette kontien");
                    break;
                case "Name session":
                    alert("Dit login er udløbet, log venligst på igen");
                    helper.logOut(null, false);
                    break;
                default:
            }
        }
    }

    let formData = helper.getFormJsonData("accountEditForm");
    xhr.send(JSON.stringify(formData));
}













function getPostings() {

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/overview', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            let jsonObject = JSON.parse(xhr.responseText);
            showPostings(jsonObject);
        }
    }
    let accountSelect = {
        "AccountName": cookie.get("selectedAcc")
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
        helper.addClass(accountElm, "tableHeaders");
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
        helper.removeClass(financeClone, "hideElm");

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

function to2digit(val) {
    if (val.toString().length < 2) val = "0" + val;
    return val;
}

function getFinanceChild(posting, columnId) {
    let columnData = posting[columnId];
    if (columnId == "payDate") {
        let parsedDate = (new Date());
        parsedDate.setTime(Date.parse(posting[columnId]));
        let month = to2digit(parsedDate.getMonth() + 1);
        columnData = parsedDate.getDate() + "/" + month + "-" + parsedDate.getFullYear()
            + " " + to2digit(parsedDate.getHours()) + ":" + to2digit(parsedDate.getMinutes());
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




function showAddFinance(e) {
    if (e != undefined) e.preventDefault();
    modal.show(null, "createFinanceSchema");
}


function addFinance(e) {
    if (e != undefined) e.preventDefault();
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
                    modal.hide(null, "createFinanceSchema");
                    getPostings();
                }
                else switch (successMsg) {
                    case "Wrong Konti name":
                        alert("Der skete en teknisk fejl i forhold til at oprette postering under kontien.");
                        helper.logOut();
                        break;
                    case "not permitted":
                        alert("Du kan ikke tilføje en postering.");
                        break;
                    case "no session":
                        alert("Dit login er udløbet, du logges ud. Log venligst på igen.");
                        helper.logOut();
                        break;
                    default:
                }
            }
            catch {
                alert("En fejl opstod under oprettelse af postering");
            }
        }
    }

    let formData = helper.getFormJsonData("addFinanceForm");
    formData.value = formData.value.replace(",", ".");
    formData.konti = document.getElementById("accountInjection").value;
    xhr.send(JSON.stringify(formData));
}


function deleteUser(e, user) {
    if (e != undefined) e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/user/delete', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                if (xhr.responseText != "OK") {
                    alert("Brugeren blev slettet");
                }
                else alert("Brugeren kunne ikke slettes");
                populateUsers();
            }
            catch {
                alert("En fejl opstod under håndtering af sletning");
            }
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 500) {
            alert("En kritisk fejl opstod da brugeren blev forsøgt slettet");
        }
    }

    xhr.send(JSON.stringify(user));
}

/* Modals */
window.onresize += modal.resize();

/* Repeating finance entries */
function getRepFinance(e) {
    if (e != undefined) e.preventDefault();

    let xhr = new XMLHttpRequest();
    xhr.open("GET", '/account/finance/repeated', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                let repFinances = JSON.parse(xhr.responseText);
                populateRepFinances(repFinances)
            }
            catch {
                alert("En fejl opstod under håndtering af gentagende betalinger");
            }
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 500) {
            alert("En kritisk fejl opstod da gentagende betalinger blev hentet");
        }
    }

    xhr.send();
}


function populateRepFinances(repFinances) {
    let repFinanceHolder = document.getElementById("repFinanceSchema");
    let repFinanceTBody = repFinanceHolder.getElementsByTagName("tbody")[0];
    let repFinanceEntries = repFinanceTBody.getElementsByTagName("tr");
    while (repFinanceEntries.length > 1) repFinanceTBody.removeChild(repFinanceEntries[1]);

    for (let repFinance of repFinances) {
        let clonedRepFinanceRow = repFinanceEntries[0].cloneNode(true);
        helper.removeClass(clonedRepFinanceRow, "hideElm");
        let clonedTds = clonedRepFinanceRow.getElementsByTagName("td");

        for (let i = 0; i < clonedTds.length; i++) {
            let childTextElm = getRepFinanceColText(repFinance, i)
            if (childTextElm == null) continue;
            clonedTds[i].appendChild(childTextElm);
        }

        repFinanceTBody.appendChild(clonedRepFinanceRow);
    }

    modal.show(null, "repFinanceSchema");
}

function getRepFinanceColText(repFinance, column) {
    let text = "";
    switch (column) {
        case 0:
            text = repFinance["value"];
            break;
        case 1:
            text = repFinance["comment"];
            break;
        case 2:
            text = repFinance["konti"];
            break;
        case 3:
            text = repFinance["byWho"];
            break;
        case 4:
            text = repFinance["intervalType"];
            break;
        case 5:
            text = repFinance["intervalValue"];
            break;
        case 6:
            text = repFinance["nextExecDate"];
            break;
        case 7:
            return getDeleteRepFinanceElm(repFinance);
            break;
        default:
            return null;
    }
    return document.createTextNode(text);
}


function getDeleteRepFinanceElm(repFinance) {
    let elmHref = document.createElement("a");
    elmHref.setAttribute("href", "/account/finance/repeated/delete");
    elmHref.addEventListener("click", function () { deleteRepFinance(event, repFinance) });
    let elmImage = document.createElement("img");
    elmImage.setAttribute("src", "/Media/DeleteIcon.png");
    helper.addClass(elmImage, "tableImgDelete");

    elmHref.appendChild(elmImage);
    return elmHref;
}




function deleteRepFinance(e, repFinance) {
    if (e != undefined) e.preventDefault();
    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/finance/repeated/delete', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                if (xhr.responseText == "OK") {
                    alert("Gentagende betaling blev slettet");
                }
                else alert("Gentagende betaling kunne ikke slettes");
                getRepFinance(null);
            }
            catch {
                alert("En fejl opstod under håndtering af gentagende betaling");
            }
        }
        else if (this.readyState === XMLHttpRequest.DONE && this.status === 500) {
            alert("En kritisk fejl opstod da gentagende betaling blev forsøgt slettet");
        }
    }

    xhr.send(JSON.stringify(repFinance));
}




function createRepFinance(e) {
    if (e != undefined) e.preventDefault();

    // Assign perform create function
    let editButton = document.getElementById("performCreateRepFinanceButton");
    let clonedButton = editButton.cloneNode(true);
    clonedButton.addEventListener("click", function () { performCreateRepFinance(event) });
    let editButtonParent = editButton.parentNode;
    editButtonParent.removeChild(editButton);
    editButtonParent.appendChild(clonedButton);

    modal.show(event, 'createRepFinanceSchema');
}

function performCreateRepFinance(e) {
    if (e != undefined) e.preventDefault();

    let createRepFinanceForm = document.forms["createRepFinanceForm"];
    if (createRepFinanceForm == undefined) return;

    let xhr = new XMLHttpRequest();
    xhr.open("POST", '/account/finance/repeated', true);
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
            try {
                let repPaymentCreatedSuccess = xhr.responseText;

                if (repPaymentCreatedSuccess == "OK") {
                    alert("Gentagende betaling blev oprettet");
                    modal.hide(null, "createRepFinanceSchema");
                    getRepFinance(event);
                }
                else switch (repPaymentCreatedSuccess) {
                    default:
                        alert("Error");
                }
            }
            catch {
                alert("En fejl opstod under oprettelse af gentagende betaling");
            }
        }
    }

    let formData = helper.getFormJsonData("createRepFinanceForm");
    xhr.send(JSON.stringify(formData));
}













/* Temp exposes to global after refactor to module usage begin
 * ... these must be set using add eventlisteners onload instead ... */
window.login = login;
window.showEditAccount = showEditAccount;
window.showAddAccount = showAddAccount;
window.changeAccount = changeAccount;
window.addAccount = addAccount;

window.showAddFinance = showAddFinance;
window.getRepFinance = getRepFinance;

window.changeAccountName = changeAccountName;
window.createRepFinance = createRepFinance;
window.addFinance = addFinance;

window.userCreate = userCreate;

window.hide = modal.hide;
window.logOut = helper.logOut;