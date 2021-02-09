'use strict';

const fs = require('fs');

let rawdata = fs.readFileSync('student.json');
let student = JSON.parse(rawdata);

var args = process.argv.slice(2);