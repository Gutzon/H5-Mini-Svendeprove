import { modal } from './modal.js';

/**
 * Return an edit/delte img with onclick event
 * @param {any} method The method to run onclick
 * @param {any} dataObj Data for the method
 * @param {any} isDelete Return a delete/edit img
 */
function getImgTriggerElm(method, dataObj, isDelete) {
    let elmHref = document.createElement("a");
    elmHref.setAttribute("href", "#");
    elmHref.addEventListener("click", function () { method(event, dataObj) });

    let imgType = (isDelete ? "Delete" : "Edit");
    let imgSrc = ("/Media/" + imgType + "Icon.png");
    let imgClass = ("tableImg" + imgType);

    let elmImage = document.createElement("img");
    elmImage.setAttribute("src", imgSrc);
    addClass(elmImage, imgClass);

    elmHref.appendChild(elmImage);
    return elmHref;
}


/**
 * Inserts data into modals
 * @param {any} modalId The id for modal/form
 * @param {any} objData Data to be parsed in
 * @param {any} method Method to run on confirm
 */
function populateModal(modalId, objData, method, dataTransformer) {
    let modalMain = document.getElementById(modalId + "Modal");
    let modalHead = document.getElementById(modalId + "ModalHead");
    let modalForm = document.getElementById(modalId + "Form");
    insertCloseModalElm(modalHead);

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
}

function insertCloseModalElm(modalHead) {
    let modalAElms = modalHead.getElementsByTagName("a");
    if (modalAElms.length == 0) {
        let modalClose = document.createElement("a");
        modalClose.addEventListener("click", modal.hide(event, modalId + "Modal"));
        modalClose.setAttribute("href", "/closeModal");
        let modalCloseImg = document.createElement("img");
        modalCloseImg.setAttribute("src", "/Media/DeleteIcon.png");
    }
}



export let dataPopulator = {
    getImgTriggerElm: getImgTriggerElm,
    populateModal: populateModal
};