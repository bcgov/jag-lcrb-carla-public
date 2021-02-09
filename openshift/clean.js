'use strict';

const fs = require('fs');

var args = process.argv.slice(2);

let rawdata = fs.readFileSync(args[0]);
let template = JSON.parse(rawdata);

