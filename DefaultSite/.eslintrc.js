module.exports = {
    root: true,
    env: {
      node: true
    },
    'extends': [
      'plugin:vue/essential',
      'eslint:recommended'
    ],
    parserOptions: {
      ecmaVersion: 2020
    },
    rules: {
      'vue/no-multiple-template-root': 'off'
    }
  }