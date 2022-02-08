import { toCamelCase } from './Helper';

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
