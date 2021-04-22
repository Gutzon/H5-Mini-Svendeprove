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



export let dataPopulator = {
    populateEditModal: populateEditModal,   
    getImgTriggerElm: getImgTriggerElm
};