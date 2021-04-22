// TODO: Make to array of active modals
// ...though only one use case currently...
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
    if (e != null) {
        e.preventDefault();
        activeModal = elmId;
    }
    let modal = document.getElementById(elmId);
    let modalSizeEvaluation = document.getElementById("sizeEvaluation");
    modalSizeEvaluation.appendChild(modal);
    removeClass(modal, "hideElm");

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
    if (e != null) {
        e.preventDefault();
    }
    let modal = document.getElementById(elmId);
    let forms = modal.getElementsByTagName("form");
    for (var form of forms) form.reset();
    addClass(modal, "hideElm");
}


export let modal = {
    hide: hide,
    show: show,
    resize: resize
};