import { modal } from './modal.js';
import { helper } from './helper.js';

/**
 * Inserts data into modals
 * @param {any} modalId The id for modal/form
 * @param {any} objData Data to be parsed in
 * @param {any} dataTransformer Opt. method defining how to handle data
 */
function populateEditModal(modalId, objData, dataTransformer) {
    // TODO: Use dataTransformer when refactoring to use this genericly
    let modalForm = document.getElementById(modalId + "Form");
    modal.insertCloseModalElm(modalId);

    for (let param in objData) {
        let newParamFormElm = modalForm.elements[param];
        if (newParamFormElm != undefined) {
            newParamFormElm.value = objData[param];
        }

        let oldParamElm = document.getElementById(modalId + "_" + param);
        if (oldParamElm == undefined) continue;
        for (let child of oldParamElm.childNodes) {
            child.parentNode.removeChild(child);
        }
        oldParamElm.appendChild(document.createTextNode(objData[param]));
    }

    modal.show(null, modalId + "Modal");
}


/**
 * Return an edit/delte img with onclick event
 * @param {any} method The method to run onclick
 * @param {any} dataObj Data for the method
 * @param {any} isDelete Return a delete/edit img
 */
function getImgTriggerElm(method, dataObj, isDelete) {
    let elmHref = document.createElement("a");
    elmHref.setAttribute("href", "#");
    elmHref.addEventListener("click", function (e) { method(e, dataObj) });

    let imgType = (isDelete ? "Delete" : "Edit");
    let imgSrc = ("/Media/" + imgType + "Icon.png");
    let imgClass = ("tableImg" + imgType);

    let elmImage = document.createElement("img");
    elmImage.setAttribute("src", imgSrc);
    helper.addClass(elmImage, imgClass);

    elmHref.appendChild(elmImage);
    return elmHref;
}



/**
 * Dynamically injects data into a table
 * @param {any} rowSchemaId The rowSchame identifier, ex memberSchema
 * @param {any} objData Data to parse in
 * @param {any} dataTransformer A method returning elements depending on param
 * @param {any} editInitMethod The edit method to trigger
 * @param {any} performDeleteMethod The delete method to trigger
 * @param {any} editPermissions What permissions gives edit right (addCorporation and admin is included)
 * @param {any} deletePermissions What permissions gives delete right (addCorporation and admin is included)
 */
function injectData(rowSchemaId, objData, dataTransformer, editInitMethod, performDeleteMethod, editPermissions, deletePermissions) {
    let rowSchema = document.getElementById(rowSchemaId);
    let parentElm = rowSchema.parentNode;


    // Empty form for fresh injection
    let schemaRows = parentElm.getElementsByTagName("tr");
    while (schemaRows.length > 1) parentElm.removeChild(schemaRows[1]);

    // Hide headers for edit delete as needed
    let hasEditPermission = helper.hasPermission(editPermissions);
    let editTh = document.getElementById(rowSchemaId + "Edit");
    let hasDeletePermission = helper.hasPermission(deletePermissions);
    let deleteTh = document.getElementById(rowSchemaId + "Delete");

    let thElements = document.getElementById(rowSchemaId + "Header").getElementsByTagName("th");
    let lastThElm = thElements.length - 1;


    // Ensure only one end class when modal reused
    helper.removeClass(thElements[lastThElm - 1], "tableDataEnd");
    helper.removeClass(thElements[lastThElm - 2], "tableDataEnd");

    if (!hasEditPermission) {
        helper.addClass(editTh, "hideElm");
        lastThElm--;
    }
    else helper.removeClass(editTh, "hideElm");

    if (!hasDeletePermission) helper.addClass(deleteTh, "hideElm");
    else {
        helper.removeClass(deleteTh, "hideElm");
        lastThElm--;
    }
    helper.addClass(thElements[lastThElm], "tableDataEnd",);

    for (let data in objData) {
        let column = 0;
        let clonedRow = rowSchema.cloneNode(true);
        let tdElements = clonedRow.getElementsByTagName("td");

        for (let param in objData[data]) {
            let tdChildElm = dataTransformer(param, objData[data]);
            if (tdChildElm == null) continue;
            tdElements[column++].appendChild(tdChildElm);
        }

        // Append edit button if allowed
        if (column < tdElements.length && editInitMethod !== undefined) {
            if (hasEditPermission) {
                let editElm = dataPopulator.getImgTriggerElm(editInitMethod, objData[data], false);
                tdElements[column++].appendChild(editElm);
            }
            else tdElements[0].parentNode.removeChild(tdElements[column]);
        }

        // Append delete button if allowed
        if (column < tdElements.length && performDeleteMethod !== undefined) {
            if (hasDeletePermission) {
                let deleteElm = dataPopulator.getImgTriggerElm(performDeleteMethod, objData[data], true);
                tdElements[column].appendChild(deleteElm);
            }
            else {
                tdElements.parentNode.removeChild(tdElements[column]);
                helper.addClass(tdElements[tdElements.length - 1], "tableDataEnd");
            }
        }

        helper.removeClass(clonedRow, "hideElm");
        parentElm.appendChild(clonedRow);
    }
}



export let dataPopulator = {
    populateEditModal: populateEditModal,   
    getImgTriggerElm: getImgTriggerElm,
    injectData: injectData
};