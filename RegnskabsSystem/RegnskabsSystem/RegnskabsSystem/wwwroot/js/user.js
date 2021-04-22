import { helper } from './helper.js';
import { modal } from './modal.js';
import { cookie } from './cookie.js';
import { dataPopulator } from './data-populator.js';



function show() {
    let rowSchema = document.getElementById("userSchema");
    if (rowSchema == null) return;

    helper.addInitButtonEvent("userCreate", createInit);
    helper.showButton("userCreateInitButton", ["addUser"]);

    helper.fetchData("GET", "/user/overview")
        .then((objData) => {
            dataPopulator.injectData("userSchema", objData, tranformUserData, editInit, performDelete, ["editUser"], ["deleteUser"]);
        })
        .catch((error) => {
            helper.errorNotify("hentning af bruger.", error);
        });
}




function tranformUserData(param, data) {
    let dataText = data[param];
    if (param.toUpperCase() == "PASSWORD") return null;
    if (param.toUpperCase() == "HASHPASSWORD") return null;
    if (param.toUpperCase() == "LASTNAME") return null;

    if (param.toUpperCase() == "LASTSEEN") {
        if (dataText == "0001-01-01T00:00:00") return document.createTextNode("Ikke logget på endnu");
        let dateParsed = Date.parse(dataText)
        let parsedDate = (new Date());
        parsedDate.setTime(dateParsed);
        return document.createTextNode(parsedDate.toLocaleString());
    }

    if (param.toUpperCase() == "FIRSTNAME") {
        dataText = data[param] + " " + data["lastname"];
    }

    if (param.toUpperCase() == "PERMISSIONS") {
        return null;
    }
    return document.createTextNode(dataText);
}



function createInit(e) {
    if (e != undefined) e.preventDefault();
    modal.showCreateModal("userCreate", performCreate);
    showUserCreatePermissions();
}



function performCreate(e) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("userCreateForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved tilføjelse af bruger.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }

    helper.fetchDataTxt("POST", "/user", JSON.stringify(formData))
        .then((objData) => {
            let jsonObject = JSON.parse(objData);

            if (jsonObject.userCreated) {
                alert("Brugeren blev oprettet");
                modal.hide(null, "userCreateModal");
                console.log("Midlertidig levering af kodeord, da vi ikke har webhotel på app'en:");
                console.log(jsonObject.userPassword);
                show();
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
        })
        .catch((error) => {
            if ("Name empty") alert("Navn skal angives ved tilføjelse af bruger");
            else if ("Name permited") alert("Du har ikke tilladelse til at tilføje til brugere");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("tilføjelse af bruger.", error);
        });
}

function validateCreate(formData) {

    let errors = [];
    // TODO
    return errors;
}



function editInit(e, data) {
    if (e != undefined) e.preventDefault();
    dataPopulator.populateEditModal("userEdit", data);

    // Reset eventlistener to carry new version of old data
    let performEditButton = document.getElementById("userEditButton");
    let editClone = performEditButton.cloneNode(true);
    let editButtonParent = performEditButton.parentNode;
    editButtonParent.removeChild(performEditButton);
    editButtonParent.appendChild(editClone);
    showUserEditPermissions(data["username"], data["permissions"]);

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownDataObj = JSON.parse(ownData);
    let ownPermissions = helper.getPermissions();

    let hasPasswordPermissions = helper.hasPermission([]) || data["username"] == ownDataObj["username"];
    let passwordEditBox = document.getElementById("editUserPassword");
    if (hasPasswordPermissions) helper.removeClass(passwordEditBox, "hideElm");
    else helper.addClass(passwordEditBox, "hideElm");

    editClone.addEventListener("click", function (e) { performEdit(e, data) });
}



function performEdit(e, data) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("userEditForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved rettelse af bruger.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }
    let transferObject = { oldUser: data, newUser: formData };

    helper.fetchDataTxt("POST", "/user/edit", JSON.stringify(transferObject))
        .then((objData) => {
            if (objData == "true") {
                alert("Brugeren er rettet");
                modal.hide(null, "userEditModal");
                show();
            }
            else if ("Name empty") alert("Navn skal angives ved rettelse af bruger");
            else if ("Not permited") alert("Du har ikke tilladelse til at rette bruger");
            else if ("Not found") alert("Eksisterende bruger kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("rettelse af bruger.", error);

        })
        .catch((error) => {

            if (error == "Name empty") alert("Navn skal angives ved rettelse af bruger");
            else if (error == "Not permited") alert("Du har ikke tilladelse til at rette bruger");
            else if (error == "Not found") alert("Eksisterende bruger kunne ikke findes");
            else if (error == "No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("rettelse af bruger.", error);
        });
}



function performDelete(e, data) {
    if (e != undefined) e.preventDefault();

    helper.fetchDataTxt("POST", "/user/delete", JSON.stringify(data))
        .then((objData) => {
            if (objData == "OK") alert("Bruger er slettet");
            show();
        })
        .catch((error) => {

            if ("Name empty") alert("Navn skal angives ved sletning af bruger");
            else if ("Not permited") alert("Du har ikke tilladelse til at slette brugere");
            else if ("Not found") alert("Eksisterende bruger kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("sletning af bruger.", error);
        });
}



function showUserEditPermissions(username, userPermissions) {
    let permissionTBody = document.getElementById("editUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownData = cookie.get("user");
    if (ownData == "") helper.logOut(null, true);
    let ownDataObj = JSON.parse(ownData);
    let ownPermissions = helper.getPermissions();

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



function showUserCreatePermissions() {
    let permissionTBody = document.getElementById("createUserRights").getElementsByTagName("tbody")[0];
    let permissionRows = permissionTBody.getElementsByTagName("tr");
    while (permissionRows.length > 1) permissionRows[1].parentNode.removeChild(permissionRows[1]);

    let ownPermissions = helper.getPermissions();

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



export let user = {
    show: show
};