// https://material-ui.com/guides/minimizing-bundle-size/#main-content

const plugins = [
  [
    'babel-plugin-import',
    {
      'libraryName': '@material-ui/core',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'core'
  ],
  
  [
    'babel-plugin-import',
    {
      'libraryName': '@angular/forms',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'forms'
  ], 
  [
    'babel-plugin-import',
    {
      'libraryName': '@angular/common',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'common'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@material-ui/icons',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'icons'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@fortawesome/free-brands-svg-icons',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'free-brands-svg-icons'
  ]  ,
  [
    'babel-plugin-import',
    {
      'libraryName': '@fortawesome/free-regular-svg-icons',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'free-regular-svg-icons'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@fortawesome/angular-fontawesome',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'angular-fontawesome'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@fortawesome/fontawesome-svg-core',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'fontawesome-svg-core'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@fortawesome/free-solid-svg-icons',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'free-solid-svg-icons'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@angular/compiler',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'compiler'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@angular/router',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'router'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@angular/localize',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'localize'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@types/knockout',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'knockout'
  ]
  ,
  [
    'babel-plugin-import',
    {
      'libraryName': '@ngrx/store',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'store'
  ],
  [
    'babel-plugin-import',
    {
      'libraryName': '@ng-bootstrap/ng-bootstrap',
      // Use "'libraryDirectory': ''," if your bundler does not support ES modules
      'libraryDirectory': 'esm',
      'camel2DashComponentName': false
    },
    'ng-bootstrap'
  ]
  
  
  
  
  
  


 
];

module.exports = {plugins};