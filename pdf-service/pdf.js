module.exports = function (callback, html, viewData, pdfOptions) {

	// https://www.npmjs.com/package/html-pdf
	var pdf = require('html-pdf');
	// setup mustache template	
	var fs = require('fs');

	const DEFAULT_PDF_OPTIONS = {
		// Papersize Options: http://phantomjs.org/api/webpage/property/paper-size.html
		format: "Letter",        // allowed units: A3, A4, A5, Legal, Letter, Tabloid
		orientation: "portrait", // portrait or landscape

		// Page options
		border: "20px",            // default is 0, units: mm, cm, in, px

		// File options
		type: "pdf"             // allowed file types: png, jpeg, pdf
	};

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
		
};
