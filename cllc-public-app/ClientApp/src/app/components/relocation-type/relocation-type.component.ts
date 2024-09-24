import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { faArrowLeft, faChevronRight, faTrash } from "@fortawesome/free-solid-svg-icons";
import { ApplicationLicenseSummary } from "@models/application-license-summary.model";
import { ApplicationStatuses } from "@models/application-type.model";
import { LicenseDataService } from "@services/license-data.service";
import { FormBase } from "@shared/form-base";
import { Subscription } from "rxjs";
import { takeWhile } from "rxjs/operators";

export enum TemporaryRelocationStatus {
    Yes = 845280000,
    No = 845280001,
    NotApplicable = 845280002
}
@Component({
    selector: "app-relocation-type",
    templateUrl: "./relocation-type.component.html",
    styleUrls: ["./relocation-type.component.scss"]
})
export class RelocationTypeComponent extends FormBase implements OnInit {
    // Properties
    licenceId: string;
    licence: ApplicationLicenseSummary;
    form: FormGroup;
    busy: Subscription;
    requestStarted: boolean = false;
    temporaryRelocation = this.ApplicationTypeNames.LRSTemporaryRelocation;
    permanentRelocation = this.ApplicationTypeNames.LRSTransferofLocation;
    isOperatingAtTemporaryLocation: boolean = false;
    temporaryRelocationApplicationExists: boolean = false;

    // Icons
    faTrash = faTrash;
    faChevronRight = faChevronRight;
    faArrowLeft = faArrowLeft;

    constructor(
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private router: Router,
        private snackBar: MatSnackBar,
        private licenceDataService: LicenseDataService,
    ) {
        super();
        // Fetch the licence ID from the route params
        this.route.paramMap.subscribe(pmap => {
            this.licenceId = pmap.get("licenceId");
        });

        // Fetch the license object from the route state
        const navigation = this.router.getCurrentNavigation();
        if (navigation?.extras?.state && navigation.extras.state['licence']) {
            this.licence = JSON.parse(navigation.extras.state['licence']);
            if (this.licence && this.licence.temporaryRelocationStatus === TemporaryRelocationStatus.Yes) {
                this.isOperatingAtTemporaryLocation = true;
            }
            this.licence.actionApplications.forEach(app => {
                if (app.applicationTypeName === this.ApplicationTypeNames.LRSTemporaryRelocation) {
                    if (
                        app.applicationStatus === ApplicationStatuses.Submitted ||
                        app.applicationStatus === ApplicationStatuses.Incomplete ||
                        app.applicationStatus === ApplicationStatuses.PendingApproval ||
                        app.applicationStatus === ApplicationStatuses.Review ||
                        app.applicationStatus === ApplicationStatuses.Assessment ||
                        app.applicationStatus === ApplicationStatuses.Issued ||
                        app.applicationStatus === ApplicationStatuses.PendingFinalInspection ||
                        app.applicationStatus === ApplicationStatuses.ReviewingResults ||
                        app.applicationStatus === ApplicationStatuses.PendingLicenseFee
                    ) {
                        this.temporaryRelocationApplicationExists = true;
                        return;
                    }
                }
            });
        } else {
            // Handle the case where the licence is not available in the router state (page refresh)
            console.error('Licence object not found in the router state.');
            // Redirect user to licenses page to get licence object again
            this.router.navigateByUrl(`/licences`);
        }
    }

    ngOnInit() {
        this.form = this.fb.group({
            relocationType: ["", Validators.required]
        });
    }

    processApplication() {
        if (this.form.value.relocationType === "") {
            this.snackBar.open("Please select a relocation type", "Warning", { duration: 3500, panelClass: ["red-snackbar"] });
            return;
        }
        if (!this.licence) {
            this.snackBar.open("An error occurred. Please return to Licences & Authorizations and try again.", "Warning", { duration: 3500, panelClass: ["red-snackbar"] });
        }
        this.requestStarted = true;
        const actionName = this.form.value.relocationType
        const isTemporaryApplication = this.form.value.relocationType === this.ApplicationTypeNames.LRSTemporaryRelocation;
        try {
            const actionApplication = this.licence.actionApplications.find(
                app => app.applicationTypeName === actionName
                    && !app.isStructuralChange
                    && app.applicationStatus !== ApplicationStatuses.Active
                    && app.applicationStatus !== ApplicationStatuses.Approved
            )
            if (typeof (actionApplication) !== "undefined" && !isTemporaryApplication) {
                // We found a permanent relocation application in progress
                if (actionApplication.isPaid === false) {
                    // Navigate to existing application to continue
                    this.router.navigateByUrl(`/account-profile/${actionApplication.applicationId}`);
                } else {
                    this.requestStarted = false;
                    // prevent a re-submission until the application status is no longer active
                    this.snackBar.open(`${actionName} has already been submitted and is under review`,
                        "Warning",
                        { duration: 3500, panelClass: ["red-snackbar"] });
                }
            } else {
                // We didn't find an application, or this is a temporary relocation application. Create a new application
                this.busy = this.licenceDataService.createApplicationForActionType(this.licence.licenseId, actionName)
                    .pipe(takeWhile(() => this.componentActive))
                    .subscribe(data => {
                        this.router.navigateByUrl(`/account-profile/${data.id}`);
                    },
                        () => {
                            this.requestStarted = false;
                            this.snackBar.open(`Error running licence action for ${actionName}`,
                                "Fail",
                                { duration: 3500, panelClass: ["red-snackbar"] });
                        }
                    );
            }
        }
        catch (err) {
            this.requestStarted = false;
            console.error(err);
        }
    }
}
