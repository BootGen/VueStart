'use strict';

function typeArray(val) {
  let r = {'type': 'object', 'properties': {}}
  val.forEach( function (v) {
    Object.assign(r.properties, typeValue(v).properties)
  })
  return r;
}


function typeValue(val) {
  if (Array.isArray(val)) {
    return {'type': 'array', 'items': typeArray(val)};
  }
  
  if (typeof val == 'object') {
    let properties = getProperties(val);
    return {'type': 'object', 'properties': properties};
  }
  
  if (typeof val == 'number' && Number.isInteger(val)) {
    return {'type': 'integer'}
  }

  return { 'type': typeof val };
}

function getProperties(j) {
  let k = Object.keys(j);
  
  k.forEach(function(name) {
    j[name] = typeValue(j[name]);
  })
  
  return j;
}

export function getSchema(json_object) {
  let schema = {};
  
  schema['properties'] = getProperties(json_object);

  return schema;
}