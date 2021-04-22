import { helper } from './helper.js';
import { modal } from './modal.js';
import { dataPopulator } from './data-populator.js';



function show() {
    let inventoryRowSchema = document.getElementById("inventorySchema");
    if (inventoryRowSchema == null) return;

    helper.addInitButtonEvent("inventoryCreate", createInit);

    helper.fetchData("GET", "/inventory/overview", "a")
        .then((objData) => {
            injectInventoryData(inventoryRowSchema, objData, tranformInventoryData, editInit, performDelete);
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



function editInit(e, data) {
    if (e != undefined) e.preventDefault();
    dataPopulator.populateEditModal("inventoryEdit", objData);

    /*
    helper.fetchData("GET", "/inventory/overview", "a")
        .then((objData) => {
            
        })
        .catch((error) => {
            helper.errorNotify("redigering af inventar.", error);
        });*/
}



function deleteInit(e) {
    if (e != undefined) e.preventDefault();
    alert("Delete inventory");
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
            else if ("Name empty") alert("Du har ikke tilladelse til at tilføje til inventar");
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

function performEdit(e) {
    if (e != undefined) e.preventDefault();
    alert("Edit");
    modal.hide(null, "inventoryEditModal");
}

function performDelete(e) {
    if (e != undefined) e.preventDefault();
    alert("Delete");
    modal.hide(null, "inventoryDeleteModal");
}










/* Possible candidates for generic - START */
function injectInventoryData(rowSchema, objData, dataTransformer, editInitMethod, performDeleteMethod) {
    let parentElm = rowSchema.parentNode;

    // Empty form for fresh injection
    let schemaRows = parentElm.getElementsByTagName("tr");
    while (schemaRows.length > 1) parentElm.removeChild(schemaRows[1]);

    for (let data of objData) {
        let clonedRow = rowSchema.cloneNode(true);
        let tdElements = clonedRow.getElementsByTagName("td");

        let column = 0;
        for (let param in data) {
            let tdChildElm = dataTransformer(param, data);
            if (tdChildElm == null) continue;
            tdElements[column++].appendChild(tdChildElm);
        }

        if (column < tdElements.length && editInitMethod !== undefined) {
            let editElm = dataPopulator.getImgTriggerElm(editInitMethod, data, false);
            tdElements[column++].appendChild(editElm);
        }

        if (column < tdElements.length && performDeleteMethod !== undefined) {
            let editElm = dataPopulator.getImgTriggerElm(performDeleteMethod, data, true);
            tdElements[column++].appendChild(editElm);
        }

        helper.removeClass(clonedRow, "hideElm");
        parentElm.appendChild(clonedRow);
    }
}



/* Possible candidates for generic - END */



export let inventory = {
    show: show
};