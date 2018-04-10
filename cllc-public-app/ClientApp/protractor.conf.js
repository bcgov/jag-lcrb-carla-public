// Protractor configuration file, see link for more information
// https://github.com/angular/protractor/blob/master/lib/config.ts

const { SpecReporter } = require('jasmine-spec-reporter');

exports.config = {
  SELENIUM_PROMISE_MANAGER: false,
  allScriptsTimeout: 30000,
  seleniumAddress: 'http://localhost:4444/wd/hub',
  specs: [
    './e2e/**/*.e2e-spec.ts'
  ],
  capabilities: {
    'browserName': 'chrome',
    'loggingPrefs': {
      'driver': 'INFO',
      'server': 'INFO',
      'browser': 'INFO'
    }
  },
  directConnect: true,
  baseUrl: 'http://localhost:5000/cannabislicensing/',
  framework: 'jasmine',
  jasmineNodeOpts: {
    showColors: true,
    defaultTimeoutInterval: 30000,
    print: function() {}
  },
  onPrepare() {
    require('ts-node').register({
      project: 'e2e/tsconfig.e2e.json'
    });
    jasmine.getEnv().addReporter(new SpecReporter({ spec: { displayStacktrace: true } }));
  }
};
