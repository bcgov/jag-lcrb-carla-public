import { By } from "@angular/platform-browser";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { ActivatedRoute } from "@angular/router";
import { ApplicationComponent } from "./application.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { StoreModule } from "@ngrx/store";
import { reducers, metaReducers } from "@app/app-state/reducers/reducers";
import { PaymentDataService } from "@services/payment-data.service";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { RouterTestingModule } from "@angular/router/testing";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ApplicationDataService } from "@services/application-data.service";
import { AccountDataService } from "@services/account-data.service";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { FormBuilder } from "@angular/forms";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { of } from "rxjs";
import { Application } from "@models/application.model";
import { provideMockStore } from "@ngrx/store/testing";
import { AppState } from "@app/app-state/models/app-state";
import { FileSystemItem } from "@models/file-system-item.model";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { Account } from "@models/account.model";
import { FileUploaderComponent } from "@shared/components/file-uploader/file-uploader.component";
import { FieldComponent } from "@shared/components/field/field.component";

let paymentDataServiceStub: Partial<PaymentDataService>;
let applicationDataServiceStub: Partial<ApplicationDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let tiedHouseConnectionsDataServiceStub: Partial<TiedHouseConnectionsDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe("ApplicationComponent",
  () => {
    let component: ApplicationComponent;
    let fixture: ComponentFixture<ApplicationComponent>;
    let applicationService: ApplicationDataService;

    const account = new Account();
    account.businessType = "PublicCorporation";
    const initialState = {
      currentAccountState: { currentAccount: account },
      currentUserState: { currentUser: {} }
    } as AppState;

    beforeEach(waitForAsync(() => {
      paymentDataServiceStub = {};
      applicationDataServiceStub = {
        getSubmittedApplicationCount: () => of(0),
        cancelApplication: () => of(null),
        updateApplication: () => of(null),
        getApplicationById: () => of({
          applicationType: {
            contentTypes: []
          } as any
        } as Application),

      };
      dynamicsDataServiceStub = { getRecord: () => of([]) };
      tiedHouseConnectionsDataServiceStub = {
        updateTiedHouse: () => of(null)
      };
      matDialogStub = {};
      matSnackBarStub = {};
      activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });
      TestBed.configureTestingModule({
          declarations: [ApplicationComponent, FileUploaderComponent, FieldComponent],
          imports: [
            RouterTestingModule,
            HttpClientTestingModule,
            StoreModule.forRoot(reducers, { metaReducers }),
          ],
          providers: [
            provideMockStore({ initialState }),
            FormBuilder,
            { provide: PaymentDataService, useValue: paymentDataServiceStub },
            { provide: AccountDataService, useValue: {} },
            { provide: ApplicationDataService, useValue: applicationDataServiceStub },
            { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
            { provide: TiedHouseConnectionsDataService, useValue: tiedHouseConnectionsDataServiceStub },
            { provide: MatDialog, useValue: matDialogStub },
            { provide: ActivatedRoute, useValue: activatedRouteStub },
            { provide: MatSnackBar, useValue: matSnackBarStub },
          ],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();

      applicationService = TestBed.get(ApplicationDataService);
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ApplicationComponent);
      component = fixture.debugElement.componentInstance;
      fixture.detectChanges();
    });

    afterEach(() => { fixture.destroy(); });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });

    it("should have the title specified in the application type",
      () => {
        applicationService.getApplicationById = () => of({
          applicationType: {
            title: "Submit the Cannabis Retail Store Application",
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        expect(component.htmlContent.title).toBe("Submit the Cannabis Retail Store Application");
      });

    it("should show preamble content if enabled",
      () => {


        applicationService.getApplicationById = () => of({
          applicantType: "PublicCorporation",
          applicationType: {
            contentTypes: [
              {
                'body': "body1",
                'category': "Preamble",
                'businessTypes': ["PublicCorporation"]
              },
            ]
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        // ensure the indigenous nation box is not clicked
        // component.application.indigenousNationId = null; //("applyAsIndigenousNation").setValue(false);

        fixture.detectChanges();
        expect(component.htmlContent.preamble).toBe("body1");
      });

    it("should hide preamble content if disabled",
      () => {
        expect(component.htmlContent.preamble).toBe("");
      });

    it("should show before starting content if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicantType: "PublicCorporation",
          applicationType: {
            contentTypes: [
              {
                'id': "2",
                'body': "body2",
                'category': "BeforeStarting",
                'businessTypes': ["PublicCorporation"]
              }
            ]
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        expect(component.htmlContent.beforeStarting).toBe("body2");
      });

    it("should hide before starting content if disabled",
      () => {
        expect(component.htmlContent.beforeStarting).toBe("");
      });

    it("should show next steps content if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicantType: "PublicCorporation",
          applicationType: {
            contentTypes: [
              {
                'id': "3",
                'body': "body3",
                'category': "NextSteps",
                'businessTypes': ["PublicCorporation"]
              }
            ]
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        expect(component.htmlContent.nextSteps).toBe("body3");
      });

    it("should hide before starting content if disabled",
      () => {
        expect(component.htmlContent.nextSteps).toBe("");
      });

    it("should show declarations content if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicationType: {
            'showDeclarations': true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();

        const elem = fixture.debugElement.query(By.css(".ngtest-declarations")).nativeElement;
        expect(elem).not.toBeFalsy();
      });

    it("should hide declarations content if disabled",
      () => {
        const elem = fixture.debugElement.query(By.css(".ngtest-declarations"));
        expect(elem).toBeFalsy();
      });
      
    it("should show property details if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicationType: {
            'showPropertyDetails': true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        const elem = fixture.debugElement.query(By.css(".ngtest-property-details")).nativeElement;
        expect(elem).not.toBeFalsy();
      });

    it("should hide property details if disabled",
      () => {
        const elem = fixture.debugElement.query(By.css(".ngtest-property-details"));
        expect(elem).toBeFalsy();
      });

    it("should show current property if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicationType: {
            newEstablishmentAddress: "Yes",
            showPropertyDetails: true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        const elem = fixture.debugElement.query(By.css(".ngtest-new-address")).nativeElement;
        expect(elem).not.toBeFalsy();
      });

    it("should hide current property if disabled",
      () => {
        const elem = fixture.debugElement.query(By.css(".ngtest-new-address"));
        expect(elem).toBeFalsy();
      });

    it("should show hours of sale if enabled",
      () => {
        applicationService.getApplicationById = () => of({
          applicationType: {
            'showHoursOfSale': true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();
        const elem = fixture.debugElement.query(By.css(".ngtest-hours-of-sale")).nativeElement;
        expect(elem).not.toBeFalsy();
      });

    it("should hide hours of sale if disabled",
      () => {
        const elem = fixture.debugElement.query(By.css(".ngtest-hours-of-sale"));
        expect(elem).toBeFalsy();
      });

    it("should be invalid if no supporting documents are uploaded",
      () => {
        applicationService.getApplicationById = () => of({
          id: "1",
          applicationType: {
            'showSupportingDocuments': true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();

        component.isValid();

        expect(component.validationMessages).toContain("At least one supporting document is required.");
      });

    it("should be valid if supporting documents are uploaded",
      () => {
        applicationService.getApplicationById = () => of({
          id: "1",
          applicationType: {
            'showSupportingDocuments': true,
            contentTypes: []
          } as any
        } as Application);
        fixture = TestBed.createComponent(ApplicationComponent);
        component = fixture.debugElement.componentInstance;
        fixture.detectChanges();

        // fake file upload
        const file = new FileSystemItem();
        component.supportingDocuments.files.push(file);

        component.uploadedSupportingDocuments = 1;

        component.isValid();
        expect(component.validationMessages).not.toContain("At least one supporting document is required.");
      });
  });
