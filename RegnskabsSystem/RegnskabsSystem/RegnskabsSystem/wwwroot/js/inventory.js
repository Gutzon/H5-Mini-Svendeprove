import { helper } from './helper.js';
import { modal } from './modal.js';
import { dataPopulator } from './data-populator.js';



function show() {
    let rowSchema = document.getElementById("inventorySchema");
    if (rowSchema == null) return;

    helper.addInitButtonEvent("inventoryCreate", createInit);
    helper.showButton("inventoryCreateInitButton", ["addInventory"]);

    helper.showButton("inventorySchemaEdit", ["editInventory"]);
    helper.showButton("inventorySchemaDelete", ["deleteInventory"]);


    helper.fetchData("GET", "/inventory/overview")
        .then((objData) => {
            dataPopulator.injectData("inventorySchema", objData, tranformInventoryData, editInit, performDelete, ["editInventory"], ["deleteInventory"]);
        })
        .catch((error) => {
            helper.errorNotify("hentning af inventar.", error);
        });
}



function tranformInventoryData(param, data) {
    if (param.toUpperCase() == "CORPORATIONID") return null;
    else return document.createTextNode(data[param]);
}



function createInit(e) {
    if (e != undefined) e.preventDefault();
    modal.showCreateModal("inventoryCreate", performCreate);
}



function performCreate(e) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("inventoryCreateForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved tilføjelse af inventar.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }

    helper.fetchDataTxt("POST", "/inventory", JSON.stringify(formData))
        .then((objData) => {
            if (objData == "OK") alert("Det nye inventar er blevet tilføjet");
            modal.hide(null, "inventoryCreateModal");
            show();
        })
        .catch((error) => {
            if ("Name empty") alert("Navn skal angives ved tilføjelse af inventar");
            else if ("Name permited") alert("Du har ikke tilladelse til at tilføje til inventar");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("tilføjelse af inventar.", error);
        });
}

function validateCreate(formData) {
    let errors = [];
    if (formData.itemName == "") errors.push("En beskrivelse er påkrævet.");
    if (formData.value == "") errors.push("Varen skal have en pris.");
    else {
        formData.value = formData.value.replace(",", ".");
        if (parseFloat(formData.value) == NaN) errors.push("Prisen skal være i formatet 12.34 eller 12,34.");
    }
    return errors;
}



function editInit(e, data) {
    if (e != undefined) e.preventDefault();
    dataPopulator.populateEditModal("inventoryEdit", data);

    // Reset eventlistener to carry new version of old data
    let performEditButton = document.getElementById("inventoryEditButton");
    let editClone = performEditButton.cloneNode(true);
    let editButtonParent = performEditButton.parentNode;
    editButtonParent.removeChild(performEditButton);
    editButtonParent.appendChild(editClone);

    editClone.addEventListener("click", function (e) { performEdit(e, data) });
}



function performEdit(e, data) {
    if (e != undefined) e.preventDefault();

    let formData = helper.getFormJsonData("inventoryEditForm");
    let validationErrors = validateCreate(formData);
    if (validationErrors.length > 0) {
        alert("Der opstod en fejl ved rettelse af inventar.\n\nFejl:\n" + validationErrors.join("\n"));
        return;
    }
    let transferObject = { oldInventory: data, newInventory: formData };

    helper.fetchDataTxt("POST", "/inventory/edit", JSON.stringify(transferObject))
        .then((objData) => {
            if (objData == "OK") alert("Inventaret er rettet");
            modal.hide(null, "inventoryEditModal");
            show();
        })
        .catch((error) => {
            
            if ("Name empty") alert("Navn skal angives ved rettelse af inventar");
            else if ("Not permited") alert("Du har ikke tilladelse til at rette inventar");
            else if ("Not found") alert("Eksisterende inventar kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("rettelse af inventar.", error);
        });
}



function performDelete(e, data) {
    if (e != undefined) e.preventDefault();

    helper.fetchDataTxt("POST", "/inventory/delete", JSON.stringify(data))
        .then((objData) => {
            if (objData == "OK") alert("Inventaret er slettet");
            show();
        })
        .catch((error) => {

            if ("Name empty") alert("Navn skal angives ved sletning af inventar");
            else if ("Not permited") alert("Du har ikke tilladelse til at slette inventar");
            else if ("Not found") alert("Eksisterende inventar kunne ikke findes");
            else if ("No session") {
                alert("Dit login var udløbet, log venligst på igen.");
                helper.logOut();
            }
            else helper.errorNotify("sletning af inventar.", error);
        });
}



export let inventory = {
    show: show
};