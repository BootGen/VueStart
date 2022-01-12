import { toCamelCase } from './Helper';

function getFrom(str, position, line){
  if(!line){
    return parseInt(position);
  } else {
    let charCount = 0;
    const strArray = str.split('\n');
    for(let i = 0; i < line-1; i++){
      charCount += strArray[i].length+1;
    }
    return parseInt(charCount) + parseInt(position) - 1;
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
      return {error: true, message: 'The root element must be an object!'};
    return {error: false, message: ''};
  } catch (err) {
    const positions = getErrorPosition(text, err);
    return {error: true, from: positions.from, to: positions.to, message: err.message};
  }
}
export function formatJson(json){
  json = json.replaceAll(/\s/g,'');
  json = json.replaceAll('{','{\n');
  json = json.replaceAll('}','\n}');
  json = json.replaceAll('[','[\n');
  json = json.replaceAll(']','\n]');
  json = json.replaceAll(',',',\n');
  json = json.replaceAll(':',': ');
  return json;
}
function indentLines(lines){
  let tabCount = 0;
  for(let i = 0; i < lines.length; i++){
    if(lines[i].includes('{') || lines[i].includes('[')){
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
      tabCount++;
    }else if(lines[i].includes('}') || lines[i].includes(']')){
      --tabCount;
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
    }else{
      lines[i] = '\t'.repeat(tabCount).concat(lines[i].trim());
    }
  }
  return lines;
}
function replaceToKey(strings, lines){
  strings.forEach(key => {
    const name = key.replace(/"([^"]*)":/g, '$1')
    for(let i = 0; i < lines.length; i++){
      if(lines[i].includes('key')){
        lines[i] = lines[i].replace('key', `"${toCamelCase(name)}": `);
        break;
      }
    }
  });
  return lines;
}
function replaceToString(strings, lines){
  strings.forEach(string => {
    for(let i = 0; i < lines.length; i++){
      if(lines[i].includes('""')){
        lines[i] = lines[i].replace('""', string);
        break;
      }
    }
  });
  return lines;
}
export function prettyPrint(json) {
  json = json.replaceAll('\r','');
  json = json.replaceAll('\'','"');
  json = json.replaceAll('""','"&zwnj;"');
  const keys = json.match(/"([^"]*)":/g);
  json = json.replace(/"([^"]*)":/g, 'key');
  const strings = json.match(/"[^"]*"/g);
  json = json.replace(/"[^"]*"/g, '""');
  json = formatJson(json);
  let lines = indentLines(json.split('\n'));
  if (keys) {
    lines = replaceToKey(keys, lines);
  }  
  if (strings) {
    lines = replaceToString(strings, lines);
  }
  lines.forEach((line, idx) =>{
    if((/^(\t)+$/g).test(line) || line === ''){
      lines.splice(idx, 1);
    } 
  })
  lines = lines.join('\n');
  lines = lines.replaceAll('"&zwnj;"','""');
  return lines;
}
