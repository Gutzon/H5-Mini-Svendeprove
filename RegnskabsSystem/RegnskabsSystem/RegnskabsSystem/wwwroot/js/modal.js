import { helper } from './helper.js';

let activeModal = "";

/**
 * Resizes the current modal position on window resize
 * @param {any} e
 */
function resize(e) {
    if (activeModal == "") return;
    modal.show(null, activeModal);
}

/**
 * Shows a modal view
 * @param {any} e Event that is prevented - prevents following ex a href
 * @param {any} elmId Id of modal to show
 */
function show(e, elmId) {
    if (e != undefined) {
        e.preventDefault();
    }

    if (elmId != undefined) {
        activeModal = elmId;
    }

    let modal = document.getElementById(elmId);
    let modalSizeEvaluation = document.getElementById("sizeEvaluation");
    modalSizeEvaluation.appendChild(modal);
    helper.removeClass(modal, "hideElm");

    let modalContent = modal.getElementsByTagName("div")[0];

    let modalMarginH = ((100 - Math.round((modalContent.offsetHeight / window.innerHeight) * 100)) / 2);
    if (modalMarginH < 2) modalMarginH = 2;
    modalContent.style.margin = modalMarginH + "vh auto";

    document.body.appendChild(modal);
}


/**
 * Hides a modal view
 * @param {any} e Event that is prevented - prevents following ex a href
 * @param {any} elmId Id of modal to hide
 */
function hide(e, elmId) {
    if (e != undefined) {
        e.preventDefault();
    }
    let modal = document.getElementById(elmId);
    let forms = modal.getElementsByTagName("form");
    for (var form of forms) form.reset();
    helper.addClass(modal, "hideElm");
}

/**
 * Shows a create modal
 * @param {any} modalId The id for modal/form
 * @param {any} method The method for createButton
 */
function showCreateModal(modalId, method) {
    insertCloseModalElm(modalId);
    show(null, modalId + "Modal");
    let createButton = document.getElementById(modalId + "Button");
    if (createButton.getAttribute("class").indexOf("eventAdded") == -1) {
        createButton.addEventListener("click", function (e) { method(e) });
        helper.addClass(createButton, "eventAdded");
    }
}


function insertCloseModalElm(modalId) {
    let modalHead = document.getElementById(modalId + "ModalHead");
    let modalAElms = modalHead.getElementsByTagName("a");
    if (modalAElms.length == 0) {
        let modalClose = document.createElement("a");
        modalClose.addEventListener("click", function (e) { hide(e, modalId + "Modal") });
        modalClose.setAttribute("href", "/closeModal");

        let modalCloseImg = document.createElement("img");
        modalCloseImg.setAttribute("src", "/Media/DeleteIcon.png");
        modalClose.appendChild(modalCloseImg);

        modalHead.appendChild(modalClose);
    }
}


export let modal = {
    hide: hide,
    show: show,
    resize: resize,
    showCreateModal: showCreateModal,
    insertCloseModalElm: insertCloseModalElm
};