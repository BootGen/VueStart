
function getFrom(str, position, line){
    if(!line){
        return parseInt(position);
    } else {
        let charCount = 0;
        const strArray = str.split('\n');
        for(let i = 0; i < line-1; i++){
            charCount += strArray[i].length+1;
        }
        return charCount + parseInt(position) - 1;
    }
}

function getErrorPosition(str, err) {
    let userAgent = navigator.userAgent;
    let from = -1;
    let to = -1;
    let match = err.message.match(/\d+/g);
    if (match === null || match.length === 0)
        return { from: -1, to: -1 };
    if(userAgent.match(/firefox|fxios/i)){
        if (match.length === 1)
            return { from: -1, to: -1 };
        from = getFrom(str, match[1], match[0]);
    }else if(userAgent.match(/opr\//i)){
        from = getFrom(str, match[0]);
    } else if(userAgent.match(/edg/i)){
        from = getFrom(str, match[0]);
    }else{
        from = getFrom(str, match[0]); //chrome
    }
    if (from > 0) {
        let charCount = 0;
        const strArray = str.split('\n');
        for(let i = 0; i < strArray.length; i++){
            charCount += strArray[i].length+1;
            if(charCount >= from){
                to = charCount-1;
                break;
            }
        }
    }
    return { from, to };
}

export function validateJson(text) {
    try {
        const obj = JSON.parse(text);
        if (typeof obj !== 'object' || Array.isArray(obj))
            return {error: true, message: 'The root elements must be an object!'};
        let idx = 0;
        for (const prop in obj) {
            if (typeof obj[prop] !== 'object')
                return {error: true, message: 'The root elements must only have a single property with object or array type!'};
            idx += 1;
        }
        return {error: false, message: ''};
    } catch (err) {
        const positions = getErrorPosition(text, err);
        return {error: true, from: positions.from, to: positions.to, message: err.message};
    }
}