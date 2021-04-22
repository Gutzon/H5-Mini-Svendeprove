/**
 * Set a cookieparameter to a value
 * @param {any} param The parameter
 * @param {any} value The parameter value
 */
function set(param, value) {
    // Currently no expiration implemented for cookie params set
    document.cookie = param + "=" + value;
}



/**
 * Gets a cookieparameter
 * @param {any} param The parameter
 */
function get(param) {
    let paramIdentifier = param + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let cookieParts = decodedCookie.split(';');
    for (let i = 0; i < cookieParts.length; i++) {
        let cookieParamText = cookieParts[i];
        while (cookieParamText.charAt(0) == ' ') {
            cookieParamText = cookieParamText.substring(1);
        }
        if (cookieParamText.indexOf(paramIdentifier) == 0) {
            return cookieParamText.substring(paramIdentifier.length, cookieParamText.length);
        }
    }
    return "";
}



/**
 * Removes a cookieparameter
 * @param {any} param The parameter
 */
function remove(param) {
    document.cookie = param + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}



export let cookie = {
    set: set,
    get: get,
    remove: remove
};