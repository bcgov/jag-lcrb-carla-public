

var exec = require('child_process').exec;
var opn = require('opn');
var child;


var command = "gradlew.bat chromeTest";

// Run the test.
child = exec(command, function (error, stdout, stderr) {
  console.log('stdout: ' + stdout);
  console.log('stderr: ' + stderr);
  if (error !== null) {
	console.log("ERROR - the following problem occured during execution of the test \n" + error);
  }
  else
  {
	 console.log ("Test successful\n");
  }
  
  // This may not work on a PC if your default browser is Chrome, if Chrome is already running.
  // A workaround is to add --process-per-site to your launch Chrome shortcut (or close Chrome before running this script)
  opn('build\\reports\\chromeTest\\tests\\index.html');
	

  
});
