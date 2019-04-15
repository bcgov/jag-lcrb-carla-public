// // Protractor configuration file, see link for more information
// // https://github.com/angular/protractor/blob/master/lib/config.ts

// const { SpecReporter } = require('jasmine-spec-reporter');

// exports.config = {
//   SELENIUM_PROMISE_MANAGER: false,
//   allScriptsTimeout: 30000,
//   seleniumAddress: 'http://localhost:4444/wd/hub',
//   specs: [
//     './e2e/**/*.e2e-spec.ts'
//   ],
//   suites: {
//     smokeTest: "./e2e/app.e2e-spec.ts",
//   },
//   capabilities: {
//     'browserName': 'chrome',
// 	 chromeOptions: {
//        args: ["--headless", "--disable-gpu", "--no-zygote", "--no-sandbox", "--window-size=1024x768"]
//        //args: ["--disable-gpu", "--no-zygote", "--no-sandbox", "--window-size=1024x768"]
// 	 },
//     'loggingPrefs': {
//       'driver': 'INFO',
//       'server': 'INFO',
//       'browser': 'INFO'
//     }
//   },
//   directConnect: true,
//   baseUrl: 'http://localhost:5000/cannabislicensing/',
//   framework: 'jasmine',
//   jasmineNodeOpts: {
//     showColors: true,
//     defaultTimeoutInterval: 3000000,
//     print: function() {}
//   },
//   onPrepare() {
//     require('ts-node').register({
//       project: 'e2e/tsconfig.e2e.json'
//     });
//     jasmine.getEnv().addReporter(new SpecReporter({ spec: { displayStacktrace: true } }));

//     var fs = require('fs-extra');

//     fs.emptyDir('e2e-reports/screenshots/', function (err) {
//       console.log(err);
//     });

//     var reporters = require('jasmine-reporters');
//     var junitReporter = new reporters.JUnitXmlReporter({
//       savePath: __dirname,
//       consolidateAll: true
//     });
//     jasmine.getEnv().addReporter(junitReporter);

//     jasmine.getEnv().addReporter({
//       specDone: function (result) {
//         if (result.status == 'failed') {
//           browser.getCapabilities().then(function (caps) {
//             var browserName = caps.get('browserName');

//             browser.takeScreenshot().then(function (png) {
//               var stream = fs.createWriteStream('e2e-reports/screenshots/' + browserName + '-' + result.fullName + '.png');
//               stream.write(new Buffer(png, 'base64'));
//               stream.end();
//             });
//           });
//         }
//       }
//     });
//   },
//   onComplete: function () {
//     var browserName, browserVersion;
//     var capsPromise = browser.getCapabilities();

//     capsPromise.then(function (caps) {
//       browserName = caps.get('browserName');
//       browserVersion = caps.get('version');

//       var HTMLReport = require('protractor-html-reporter');

//       testConfig = {
//         reportTitle: 'Test Execution Report',
//         outputPath: './e2e-reports',
//         screenshotPath: './e2e-reports/screenshots',
//         testBrowser: browserName,
//         browserVersion: browserVersion,
//         modifiedSuiteName: false,
//         screenshotsOnlyOnFailure: true
//       };
//       new HTMLReport().from('junitresults.xml', testConfig);
//     });
//   }

// };
// Protractor configuration file, see link for more information
// https://github.com/angular/protractor/blob/master/lib/config.ts

const { SpecReporter } = require('jasmine-spec-reporter');

exports.config = {
  allScriptsTimeout: 11000,
  specs: [
    './src/**/*.e2e-spec.ts'
  ],
  suites: {
    smokeTest: './e2e/app.e2e-spec.ts',
    loginTest: './e2e/login.e2e-spec.ts',
  },
  capabilities: {
    'browserName': 'chrome',
    chromeOptions: {
      // args: ["--headless", "--disable-gpu", "--no-zygote", "--no-sandbox", "--window-size=1024x768"]
      args: ["--disable-gpu", "--no-zygote", "--no-sandbox", "--window-size=1024x768"]
    }
  },
  directConnect: true,
  baseUrl: process.env.BASE_URL ? `${process.env.BASE_URL}` : 'http://localhost:4200/',
  framework: 'jasmine',
  jasmineNodeOpts: {
    showColors: true,
    defaultTimeoutInterval: 30000,
    print: function () { }
  },
  onPrepare() {
    require('ts-node').register({
      project: require('path').join(__dirname, './e2e/tsconfig.e2e.json')
    });
    jasmine.getEnv().addReporter(new SpecReporter({ spec: { displayStacktrace: true } }));
  }
};