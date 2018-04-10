import { browser, by, element } from 'protractor';
import { AppSurveyPage } from './survey.po';

describe('App Survey Page', () => {
    let page: AppSurveyPage;
    let surveyConfig: {};

    function httpGet(theUrl)
    {
        var XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
        var xhr = new XMLHttpRequest();
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open( "GET", theUrl, false ); // false for synchronous request
        xmlHttp.send( null );
        return xmlHttp.responseText;
    }

    beforeAll(() => {
        // load survey json from resources
        var resp_json = httpGet("http://localhost:5000/cannabislicensing/assets/survey-primary.json");
        expect(resp_json).toContain('"title": "Find out what you need to apply for a licence",');
        surveyConfig = JSON.parse(resp_json);

        // TODO load survey results text ?
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
        expect(page.getMainHeading()).toEqual('Find out what you need to apply for a licence');
    });

    it('should load the survey configuration file', async () => {
        await page.navigateTo();
        expect(surveyConfig.title).toEqual('Find out what you need to apply for a licence');
        expect(page.getMainHeading()).toEqual(surveyConfig.title);
        expect(surveyConfig.pages[0].name).toEqual('q1');
        expect(surveyConfig.pages[0].elements[0].type).toEqual('radiogroup');
        expect(surveyConfig.pages[0].elements[0].name).toEqual('age19');
        expect(surveyConfig.pages[0].elements[0].title).toEqual('Are you 19 years old or over?');

        // TODO verify survey results text has loaded
    });

    it('should not allow under 19 years old to apply', async () => {
        var navPath = [{'q':'q1', 'r':'No', 'button':'complete'}];

        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        // TODO expect we are on result page
    });

    it('should allow over 19 years old to apply and capture all information', async () => {
        var navPath = [{'q':'q1', 'r':'Yes', 'button':'next'},
                       {'q':'q2', 'r':'Yes', 'button':'next'},
                       {'q':'q3', 'r':'Corporation', 'button':'next'},
                       {'q':'q4', 'r':'Yes', 'button':'next'},
                       {'q':'q5', 'r':'Richmond', 'button':'next'},
                       {'q':'q6', 'r':'Yes', 'button':'next'},
                       {'q':'q7', 'r':'Yes', 'button':'next'},
                       {'q':'q8', 'r':'Yes', 'button':'next'},
                       {'q':'q9', 'r':'Yes', 'button':'complete'}];
        
        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        // TODO expect we are on result page
    });

    it('should allow backtrack to update previously entered information', async () => {
        var navPath = [{'q':'q1', 'r':'Yes', 'button':'next'},
                       {'q':'q2', 'r':'Yes', 'button':'next'},
                       {'q':'q3', 'r':'Corporation', 'button':'next'},
                       {'q':'q4', 'r':'Yes', 'button':'next'},
                       {'q':'q5', 'r':'Richmond', 'button':'next'},
                       {'q':'q6', 'r':'Yes', 'button':'prev'},
                       {'q':'q5', 'r':'Saanich', 'button':'next'},
                       {'q':'q6', 'r':'Yes', 'button':'next'},
                       {'q':'q7', 'r':'Yes', 'button':'next'},
                       {'q':'q8', 'r':'Yes', 'button':'next'},
                       {'q':'q9', 'r':'Yes', 'button':'complete'}];
        
        await page.navigateTo();
        await page.executeSurvey(navPath, surveyConfig);

        // TODO expect we are on result page
    });
});
