# 🚀 mygovbc-bootstrap-theme
A B.C. Gov. Look-and-Feel for MyGovBC Service Providers using a bootstrap 3.x theme.

[Demostration of all bootstrap styles implemented and enhancements using this theme](https://bcgov.github.io/mygovbc-bootstrap-theme/docs/index.html)

# Installation

Pick you favourite package manager method:

## NPM

`npm install mygovbc-bootstrap-theme --save-dev`

## Yarn

`yarn add mygovbc-bootstrap-theme`

## Manual

Download the contents of the `dist` folder and include in your project.

# Usage in Web App

## CSS Includes

Depending on your web build system, i.e., webpack, you may or may not have to explicitly add the CSS link.  

This code snippet shows the net result of the build system or if you're doing it manually:

```
<link rel="stylesheet"
      href="assets/css/mygovbc-bootstrap-theme.min.css"
      media="screen">
```

## Javascript Includes (Optional)

Bootstrap has nice interactive components, this requires Bootstrap's Javascript and a JQuery dependency.  
However, if you're using an **AngularJS v2+ app**, we recommend using  [ngx-bootstrap](https://github.com/valor-software/ngx-bootstrap)
 
```
npm install ngx-bootstrap --save-dev
```

That way, you don't have to include JQuery directly and you get nice Typescript interfaces to work with.

**ReactJS** and other frameworks have similar libraries to work with.  
 
For a vanilla application you must include the Bootstrap JS and JQuery, they've been repacked for linking to in your app:

```
dist/js/bootstrap.js       <--- for development
dist/js/bootstrap.min.js   <--- for production
dist/js/npm.js             <--- for linking to with a node based build system
```

## Fonts and Other Assets

The MyGovBC Bootstrap imports within the CSS to the following assets:

```
dist/fonts/fontawesome-webfont.ttf    <--- the font awesome icon pack, more choices than glyphicons lib
dist/fonts/MyriadWebPro.tff           <--- the default font for the theme
dist/images/gov3_bc_logo.png          <--- B.C. Government logo that fits nicely in the header
dist/images/favicon.ico               <--- Favicon of the B.C. Government logo
```

# Enhancements from Vanilla Bootstrap

This bootstrap uses 99% bootstrap, to use it read the Bootstrap docs.  
There are some minor additions to help with the B.C. look-and-feel
 
1. Myrid WebPro font included and is used by default
1. [Font Awesome](http://fontawesome.io/icons/) replaces Glyphicons
1. `visuallyhidden` class can be used to accessibility purposes
1. Sticky headers and footers if you use a `#wrap` and `#pad` divs, see the demo page

