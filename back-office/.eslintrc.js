module.exports = {
  root: true,
  parser: '@typescript-eslint/parser',
  plugins: [
    '@typescript-eslint',
  ],
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
  ],
  overrides: [
    {
      'files': ['*.tsx'],
      'rules': {
        '@typescript-eslint/explicit-module-boundary-types': 'off'
      }
    }
  ]
};