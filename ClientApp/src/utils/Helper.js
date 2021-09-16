export function toPascalCase(str) {
    return str
        .replace(/\s(.)/g, function($1) { return $1.toUpperCase(); })
        .replace(/\s/g, '')
        .replace(/^(.)/, function($1) { return $1.toUpperCase(); });
}

export function toCamelCase(str) {
    return str
        .replace(/\s(.)/g, function($1) { return $1.toUpperCase(); })
        .replace(/\s/g, '')
        .replace(/^(.)/, function($1) { return $1.toLowerCase(); });
}

export function getJsonLength (json){
    json = json.replace(/ {2}/g, '');
    json = json.replace(/": /g, '":');
    json = json.replace(/[\n\t\r]/g, '');
    return json.length;
}

export function getJsonLineNumber(text){
    return text.split('\n').length;
}