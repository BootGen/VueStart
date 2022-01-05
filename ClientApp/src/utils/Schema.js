'use strict';

function mergeDeep(target, source) {
  const isObject = (obj) => obj && typeof obj === 'object';

  if (!isObject(target) || !isObject(source)) {
    return source;
  }

  let result = {};

  Object.keys(source).forEach(key => {
    const targetValue = target[key];
    const sourceValue = source[key];
    if (Array.isArray(targetValue) && Array.isArray(sourceValue)) {
      let mergedArray = [ ...targetValue];
      sourceValue.forEach(val => {
        if (!mergedArray.includes(val)) {
          mergedArray.push(val)
        }
      });
      result[key] = mergedArray;
    } else if (isObject(targetValue) && isObject(sourceValue)) {
      result[key] = mergeDeep(targetValue, sourceValue);
    } else {
      result[key] = sourceValue;
    }
  });

  return result;
}

function getArraySchema(val) {
  let r = {type: [], properties: {}}
  val.forEach( function (v) {
    const type = getType(v);
    if (!r.type.includes(type)) {
      r.type.push(type);
    }
    r.properties = mergeDeep(r.properties, getSchema(v).properties)
  })
  return r;
}


function getProperties(j) {
  let k = Object.keys(j);
  k.forEach(function(name) {
    j[name] = getSchema(j[name]);
  })
  return j;
}

function getType(val) {
  
  if (Array.isArray(val)) {
    return 'array';
  }
  
  if (typeof val == 'object') {
    return 'object';
  }
  
  if (typeof val == 'number') {
    if (Number.isInteger(val)) {
      return 'integer';
    }
    return 'float';
  }
  
  return typeof val;
}

export function getSchema(val) {
  let type = getType(val);

  if (type === 'array') {
    return {type: [type], items: getArraySchema(val)};
  }
  
  if (type === 'object') {
    return {type: [type], properties: getProperties(val)};
  }
  
  return { type: [type] };
}

