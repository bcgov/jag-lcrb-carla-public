import { browser, by, element } from 'protractor';
import { AppSurveyPage } from './survey.po';
import { SurveyConfig } from '../src/app/models/survey-config.model';

describe('App Survey Page', () => {
    let page: AppSurveyPage;
    let surveyConfig: SurveyConfig;

    function httpGet(theUrl)
    {
        var XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open( "GET", theUrl, false ); // false for synchronous request
        xmlHttp.send( null );
        return xmlHttp.responseText;
    }

    beforeAll(async () => {
      // load survey json from resources      
      var resp_json = httpGet(browser.baseUrl + '/assets/survey-primary.json');
      surveyConfig = JSON.parse(resp_json);      
    });

    beforeEach(() => {
        page = new AppSurveyPage();
    });

    afterEach(function() {
        browser.manage().logs().get('browser').then(function(browserLog) {
          if(browserLog.length > 0)
            console.log('log: ' + JSON.parse(JSON.stringify(browserLog))[0].message);
        });
    });

    it('should display a title', async () => {
      await page.navigateTo();
      let heading = await page.getMainHeading();
      console.log("Survey title is: " + heading);
      expect(heading).toEqual('What does an applicant need to apply for a cannabis licence?');
    });

    it('should load the survey configuration file', async () => {
        console.log('surveyConfig title is ' + surveyConfig.title);
        expect(surveyConfig.title).toEqual('What does an applicant need to apply for a cannabis licence?');
        expect(page.getMainHeading()).toEqual(surveyConfig.title);
        expect(surveyConfig.pages[0].name).toEqual('p1');
        expect(surveyConfig.pages[0].elements[0].type).toEqual('radiogroup');
        expect(surveyConfig.pages[0].elements[0].name).toEqual('age19');
        expect(surveyConfig.pages[0].elements[0].title).toEqual('Is the applicant 19 years old or older?');

        // TODO verify survey results text has loaded
    });

    it('should not allow under 19 years old to apply', async () => {
      var navPath = [{ 'q': 'p1', 'r': 'No', 'button': 'complete' }];

        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        
        // TODO expect we are on result page
    });

    it('should allow over 19 years old to apply and capture all information', async () => {
        var navPath = [{'q':'p1', 'r':'Yes', 'button':'next'},
                       {'q':'p2', 'r':'Yes', 'button':'next'},
                       {'q':'p3', 'r':'Corporation', 'button':'next'},
                       {'q':'p4', 'r':'Yes', 'button':'next'},
                       {'q':'p5', 'r':'Richmond', 'button':'next'},
                       {'q':'p6', 'r':'Yes', 'button':'next'},
                       {'q':'p7', 'r':'Yes', 'button':'next'},
                       {'q':'p8', 'r':'Yes', 'button':'next'},
                       {'q':'p9', 'r':'Yes', 'button':'complete'}];
        
        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        // TODO expect we are on result page
    });

    it('should allow backtrack to update previously entered information', async () => {
        var navPath = [{'q':'p1', 'r':'Yes', 'button':'next'},
                       {'q':'p2', 'r':'Yes', 'button':'next'},
                       {'q':'p3', 'r':'Corporation', 'button':'next'},
                       {'q':'p4', 'r':'Yes', 'button':'next'},
                       {'q':'p5', 'r':'Richmond', 'button':'next'},
                       {'q':'p6', 'r':'Yes', 'button':'prev'},
                       {'q':'p5', 'r':'Saanich', 'button':'next'},
                       {'q':'p6', 'r':'Yes', 'button':'next'},
                       {'q':'p7', 'r':'Yes', 'button':'next'},
                       {'q':'p8', 'r':'Yes', 'button':'next'},
                       {'q':'p9', 'r':'Yes', 'button':'complete'}];
        
        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        // TODO expect we are on result page
    });
});
