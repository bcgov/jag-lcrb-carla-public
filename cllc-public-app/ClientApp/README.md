# lclb_public_app

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 1.7.4.

- Npm version - 5.6.0
- Node version - 8.1.1.3
- Angular version - 5.2.11

## Installing prerequisite

Run `npm install` in `jag-lcrb-carla-public\cllc-public-app\ClientApp`. To reinstall, delete the `node_modules` directory and re-run `npm install`.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `-prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Running end-to-end tests against an arbitrary deployment

Run `ng e2e --project=lclb-public-app-e2e --base-url=[URL]` where `[URL]` is the URL to your deployed instance of the app.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

## Ngrx Store (Client side data cache/sharing)

**@ngrx/store** is a controlled state container designed to help write performant, consistent applications on top of Angular. Core tenets:

- State is a single immutable data structure
- Actions describe state changes
- Pure functions called reducers take the previous state and the next action to compute the new state
- State accessed with the Store, an observable of state and an observer of actions

For more information see the [@ngrx/store git page](https://github.com/ngrx/store).
  
ngrx code is located in the **ClientApp/scr/app/app-state** directory

**Actions**: found in **ClientApp/scr/app/app-state/actions** 

An action has a type: This is a string constant that describes/identifies the action.

An action optionally has a payload which is passed through a constructor.
The payload is a parameter usually used to pass in the new value to update the state to. 

### Example 1

```ts
import { Action } from '@ngrx/store';
import { AdoxioApplication } from '../../models/adoxio-application.model';

export const APPLICATION = 'APPLICATION';
export const SET_APPLICATION = 'SET_APPLICATION';

export class ApplicationsAction implements Action {
    readonly type = APPLICATION;
}

export class SetApplicationsAction implements Action {
    readonly type = SET_APPLICATION;

  constructor(public payload: AdoxioApplication[]) { }
}

export type Actions =
  ApplicationsAction
  | SetApplicationsAction;
```

**Models**: found in **ClientApp/scr/app/app-state/models**

In the models directory are files that describe the shape of the state object.
Notice that the AppState is comprised of smaller states. This is to allow 'sub-states' to manipulated indepentely.
It also allows components and service to only subscribe to a portion of the AppState.

### Example 2

```ts
import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { AdoxioApplication } from '../../models/adoxio-application.model';

export interface AppState {
    legalEntitiesState: LegalEntitiesState;
    applicationsState: ApplicationsState;
    currentAccountState: CurrentAccountState;
    currentApplicaitonState: CurrentApplicationState;
    currentLegalEntityState: CurrentLegalEntityState;
}

export interface LegalEntitiesState {
    legalEntities: AdoxioLegalEntity[];
}

export interface ApplicationsState {
    applications: AdoxioApplication[];
}

export interface CurrentAccountState {
    currentAccount: DynamicsAccount;
}
export interface CurrentApplicationState {
    currentApplication: AdoxioApplication;
}

export interface CurrentLegalEntityState {
    currentLegalEntity: AdoxioLegalEntity;
}
```

**Reducers**: found in **ClientApp/scr/app/app-state/reducers**
Each reducer file typically contains the action handler function (called a reducer) for a portion of state.

## Routing

Passing parameters on the URL(route)

In app-routing.module.ts

### Example 3

```js
const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'application-lite/:applicationId', //applicationId is a route parameter
    component: ApplicationComponent,
    canDeactivate: [CanDeactivateGuard]
  }
];
```

To read parameters from the route:

### Example 4

```ts
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.scss']
})
export class ApplicationComponent implements OnInit, OnDestroy {
    applicationId: string;

    constructor(private route: ActivatedRoute) {
        //Use this form if the route does not change (same route with different parameters) once the component is loaded
    this.applicationId = this.route.snapshot.params.applicationId;
    }

    ngOnInit(): void {
    //Use this form if the route changes while the component is loaded
    this.route.params.subscribe(p => {
        this.applicationId = p.applicationId;
    });
    }

    //this get called because the route configuation has a CanDeactivate router guard
    canDeactivate(): Observable<boolean> | boolean {
        if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
            return true; //return true if form value has not changed
        } else {
            return this.save(); //otherwise wait for the form to load the navigate away 
        }
    }

    save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>(); //used to exposed the result of the save operation
    const saveData = this.form.value;
    this.accountModel = this.toAccountModel(this.form.value);
    const sub = this.applicationDataService.updateAccount(this.accountModel).subscribe(
      res => {
        saveResult.next(true);
        this.savedFormData = saveData;
      },
      err => {
        saveResult.next(false); // this will stop router navigation
      });

    if (showProgress === true) {
      this.busy = sub;
    }
    return saveResult;
  }
}
```

## Reactive forms

When creating angular forms, it is important to have the 'shape' of the form-group match that of the data-model
that the form deals with. This makes it easier to get and set the form values.

See the [angular reactive forms page](https://angular.io/guide/reactive-forms#creating-nested-form-groups) for reference.

## Mobile styling

For information on the Bootstrap grid system [see](https://getbootstrap.com/docs/4.0/layout/grid/).

## Debugging in Chrome

For information about debugging in chrome vist the [chrome-devtools documentation](https://developers.google.com/web/tools/chrome-devtools/javascript/).
