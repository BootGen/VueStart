'use strict';

function isObject(item) {
  return (item && typeof item === 'object' && !Array.isArray(item));
}

function merge(current, updates) {
  for (key of Object.keys(updates)) {
    if (!current.hasOwnProperty(key) || typeof updates[key] !== 'object') current[key] = updates[key];
    else merge(current[key], updates[key]);
  }
  return current;
}
function mergeDeep(target, source) {
  const isObject = (obj) => obj && typeof obj === 'object';

  if (!isObject(target) || !isObject(source)) {
    return source;
  }

  Object.keys(source).forEach(key => {
    const targetValue = target[key];
    const sourceValue = source[key];
    if (Array.isArray(targetValue) && Array.isArray(sourceValue)) {
      target[key] = targetValue.concat(sourceValue);
    } else if (isObject(targetValue) && isObject(sourceValue)) {
      target[key] = mergeDeep(Object.assign({}, targetValue), sourceValue);
    } else {
      target[key] = sourceValue;
    }
  });

  return target;
}

function typeArray(val) {
  let r = {'type': 'object', 'properties': {}}
  val.forEach( function (v) {
    r = mergeDeep(r, typeValue(v).properties)
    
  })
  return r;
}


function typeValue(val) {
  let type = [];
  if (Array.isArray(val)) {
    type.push({array: true});
  }
  
  if (typeof val == 'object') {
    type.push({object: true});
  }
  
  if (typeof val == 'number' && Number.isInteger(val)) {
    type.push({integer: true});
  }
  type.push({type: typeof val})

  if (Array.isArray(val)) {
    return {'type': type, 'items': typeArray(val)};
  }
  
  if (typeof val == 'object') {
    let properties = getProperties(val);
    return {'type': type, 'properties': properties};
  }
  
  if (typeof val == 'number' && Number.isInteger(val)) {
    return {'type': type}
  }

  return { 'type': type };
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