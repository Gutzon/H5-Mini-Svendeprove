import { helper } from './helper.js';
import { modal } from './modal.js';
import { dataPopulator } from './data-populator.js';



function show() {
    let rowSchema = document.getElementById("memberSchema");
    if (rowSchema == null) return;

    helper.addInitButtonEvent("memberCreate", createInit);
    helper.showButton("memberCreateInitButton", ["addMember"]);

    helper.fetchData("GET", "/member/overview")
        .then((objData) => {
            dataPopulator.injectData("memberSchema", objData, tranformMemberData, editInit, performDelete, ["editMember"], ["deleteMember"]);
        })
        .catch((error) => {
            helper.errorNotify("hentning af medlem.", error);
        });
}



function tranformMemberData(param, data) {
    let dataText = data[param];
    if (param.toUpperCase() == "CORPORATIONID") return null;
    if (param.toUpperCase() == "LASTNAME") return null;
    if (param.toUpperCase() == "FIRSTNAME") {
        dataText = data[param] + " " + data["lastname"];
    }
    return document.createTextNode(dataText);
}



function createInit(e) {
    if (e != undefined) e.preventDefault();
    modal.showCreateModal("memberCreate", performCreate);
}



function performCreate(e) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("memberCreateForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved tilføjelse af medlem.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }

    helper.fetchDataTxt("POST", "/member", JSON.stringify(formData))
        .then((objData) => {
            if (objData == "OK") alert("Det nye medlem er blevet tilføjet");
            modal.hide(null, "memberCreateModal");
            show();
        })
        .catch((error) => {
            if ("Name empty") alert("Navn skal angives ved tilføjelse af medlem");
            else if ("Name permited") alert("Du har ikke tilladelse til at tilføje til medlemslisten");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("tilføjelse af medlem.", error);
        });
}

function validateCreate(formData) {
    
    let errors = [];
    if (formData.firstname == "") errors.push("Fornavn skal angives.");
    if (formData.lastname == "") errors.push("Efternavn skal angives.");

    if (formData.mail == "") errors.push("E-mail skal angives.");
    if (formData.phoneNumber == "") errors.push("Telefon nr. skal angives.");

    return errors;
}



function editInit(e, data) {
    if (e != undefined) e.preventDefault();
    dataPopulator.populateEditModal("memberEdit", data);

    // Reset eventlistener to carry new version of old data
    let performEditButton = document.getElementById("memberEditButton");
    let editClone = performEditButton.cloneNode(true);
    let editButtonParent = performEditButton.parentNode;
    editButtonParent.removeChild(performEditButton);
    editButtonParent.appendChild(editClone);

    editClone.addEventListener("click", function (e) { performEdit(e, data) });
}



function performEdit(e, data) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("memberEditForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved rettelse af medlem.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }
    let transferObject = { oldMember: data, newMember: formData };

    helper.fetchDataTxt("POST", "/member/edit", JSON.stringify(transferObject))
        .then((objData) => {
            if (objData == "OK") alert("Inventaret er rettet");
            modal.hide(null, "memberEditModal");
            show();
        })
        .catch((error) => {

            if ("Name empty") alert("Navn skal angives ved rettelse af medlem");
            else if ("Not permited") alert("Du har ikke tilladelse til at rette medlem");
            else if ("Not found") alert("Eksisterende medlem kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("rettelse af medlem.", error);
        });
}



function performDelete(e, data) {
    if (e != undefined) e.preventDefault();

    helper.fetchDataTxt("POST", "/member/delete", JSON.stringify(data))
        .then((objData) => {
            if (objData == "OK") alert("Medlem er slettet");
            show();
        })
        .catch((error) => {

            if ("Name empty") alert("Navn skal angives ved sletning af medlem");
            else if ("Not permited") alert("Du har ikke tilladelse til at slette medlemmer");
            else if ("Not found") alert("Eksisterende medlem kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("sletning af medlem.", error);
        });
}



export let member = {
    show: show
};