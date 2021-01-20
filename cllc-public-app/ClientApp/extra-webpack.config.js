/**
 * ## Angular custom webpack config for compatibility with IE11
 * ------------------------------------------------------------
 * 1. Add this file to your project root. Add the modules list that needs transpiling (see below code).
 * 2. Install dev dependencies:
 *
 *     `$ npm i -D @angular-builders/custom-webpack:browser babel-loader @babel/core @babel/preset-env browserlist`
 *
 * 3. Add this to your `angular.json`:
 *
 *     ```json
 *     "projects": {
 *       "app": {
 *         "architect": {
 *           "build": {
 *             "builder": "@angular-builders/custom-webpack:browser",
 *             "options": {
 *               "customWebpackConfig": {
 *                 "path": "./webpack.config.js",
 *                 "replaceDuplicatePlugins": true
 *               }
 *             }
 *           }
 *         }
 *       }
 *     }
 *     ```
 */

module.exports = {
  externals: {
	moment: 'moment'
  },
  module: {
    rules: [
      {
        test: /\.m?js$/,
        /**
         * Exclude `node_modules` except the ones that need transpiling for IE11 compatibility.
         * Run `$ npx are-you-es5 check . -r` to get a list of those modules.
         */
        //exclude: /[\\/]node_modules[\\/](?!(incompatible-module1|incompatible_module_2|some_other_nested_module)[\\/])/,
        use: {
          loader: 'babel-loader',
		  options: {
		  presets: ['@babel/preset-env'],
          plugins: ['@babel/plugin-transform-runtime'],
		  cacheDirectory: true,
		  cacheCompression: true
		  
		  }
          
        }
		
      }
    ]
  }
};