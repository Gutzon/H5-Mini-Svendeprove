/**
 * Fetches data using ajax
 * @param {any} method - Define POST/GET
 * @param {any} path - Define path for the request
 * @param {any} bodyData - Define body if applicable
 */
function fetchData(method, path, bodyData) {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
        xhr.open(method, path, true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
                try {
                    let objData = JSON.parse(xhr.responseText);
                    resolve(objData);
                }
                catch {
                    reject("InterpretationError");
                }
            }
            else if (this.readyState === XMLHttpRequest.DONE && this.status !== 200) reject(xhr.responseText);
        }

        if (bodyData === undefined) xhr.send();
        else xhr.send(bodyData);
    });
}


/**
 * Provides a basic error message usable for ex promise rejection.
 * @param {any} activity - Alerted with base message to user
 * @param {any} error - Printed to console for debugging
 */
function errorNotify(activity, error) {
    alert("Der opstod en fejl på serveren under " + activity);
    console.log(error);
}

export let helper = {
    errorNotify: errorNotify,
    fetchData: fetchData
};