const { parse, convert } = require('odata2openapi');

// Get the OData metadata as a string.


var fs = require('fs');
var path = require('path');
  
var xml = fs.readFileSync(path.join(__dirname, "dynamics-metadata.xml"))

const options = {
  host: 'localhost',
  nopath: '/api/data/v8.2/'
};

parse(xml)
//  .then(service => convert(service.entitySets, options, service.version))
  .then(swagger => console.log(JSON.stringify(swagger, null, 2)))
  .catch(error => console.error(error))