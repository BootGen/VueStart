'use strict';

function mergeDeep(obj1, obj2) {
  const isObject = (obj) => obj && typeof obj === 'object';

  if (!isObject(obj1) || !isObject(obj2)) {
    return obj1 || obj2;
  }

  let result = {};

  mergeArrays(Object.keys(obj2), Object.keys(obj1)).forEach(key => {
    const value1 = obj1[key];
    const value2 = obj2[key];
    if (Array.isArray(value1) && Array.isArray(value2)) {
      let mergedArray = mergeArrays(value1, value2);
      result[key] = mergedArray;
    } else if (isObject(value1) && isObject(value2)) {
      result[key] = mergeDeep(value1, value2);
    } else if (value1) {
      result[key] = value1;
    } else {
      result[key] = value2;
    }
  });

  return result;
}

function mergeArrays(array1, array2) {
  let mergedArray = [...array1];
  array2.forEach(val => {
    if (!mergedArray.includes(val)) {
      mergedArray.push(val);
    }
  });
  return mergedArray;
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
  if (!j) {
    throw { schemaError: "Null values are not supported."}
  }
  let r = {};
  let k = Object.keys(j);
  k.forEach(function(name) {
    r[name] = getSchema(j[name]);
  })
  return r;
}

function getType(val) {
  
  if (Array.isArray(val)) {
    return 'array';
  }
  
  if (typeof val == 'object') {
    return 'object';
  }
  
  if (typeof val === 'number') {
    if (Number.isInteger(val)) {
      return 'integer';
    }
    return 'float';
  }

  if (typeof val === 'string') {
    let regexExp = /^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(:\d{2})?)$/gi;
    if (regexExp.test(val))
      return 'datetime'
    regexExp = /^(http|https):\/\/.*/gi;
    if (regexExp.test(val))
      return 'link'
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

