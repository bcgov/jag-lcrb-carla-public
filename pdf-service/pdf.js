module.exports = function (callback, templateName, viewData, pdfOptions) {

	// https://www.npmjs.com/package/mustache
	var mustache = require('mustache');
	// https://www.npmjs.com/package/html-pdf
	var pdf = require('html-pdf');
	// setup mustache template	
	var fs = require('fs');

	const DEFAULT_PDF_OPTIONS = {
		// Papersize Options: http://phantomjs.org/api/webpage/property/paper-size.html
		// format: "Letter",        // allowed units: A3, A4, A5, Legal, Letter, Tabloid
		// orientation: "portrait", // portrait or landscape
	  
		// Page options
		height: "11in",
		width: "8in",            // default is 0, units: mm, cm, in, px

		// File options
		type: "pdf"             // allowed file types: png, jpeg, pdf
	}

	fs.readFile('Templates/' + templateName + '.mustache', 'utf8', function (err, template) {
		if (err) {
			callback(err, null);
		}
		else {
			// render
			var html = mustache.render(template, viewData)

			// PDF options
			var options = Object.assign({}, DEFAULT_PDF_OPTIONS, pdfOptions);

			// export as PDF
			pdf.create(html, options).toBuffer(function (err, buffer) {
				if (err) {
					callback(err, null);
				}
				else {
					callback(null, buffer.toJSON());
				}
			});
		}
	});
};
