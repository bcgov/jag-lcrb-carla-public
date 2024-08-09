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

@Component({
    selector: "app-relocation-type",
    templateUrl: "./relocation-type.component.html",
    styleUrls: ["./relocation-type.component.scss"]
})
export class RelocationTypeComponent extends FormBase implements OnInit {
    // Properties
    applicationId: string;
    licence: ApplicationLicenseSummary;
    form: FormGroup;
    busy: Subscription;
    temporaryRelocation = this.ApplicationTypeNames.LRSTemporaryRelocation;
    permanentRelocation = this.ApplicationTypeNames.LRSTransferofLocation;

    // Icons
    faTrash = faTrash;
    faChevronRight = faChevronRight;
    faArrowLeft = faArrowLeft;

    constructor(
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private router: Router,
        private snackBar: MatSnackBar,
        private licenceDataService: LicenseDataService
    ) {
        super();
        // Fetch the application ID from the route params
        this.route.paramMap.subscribe(pmap => {
            this.applicationId = pmap.get("applicationId");
        });
        // Fetch the license object from the route state
        const navigation = this.router.getCurrentNavigation();
        if (navigation?.extras.state && navigation.extras.state['licence']) {
            this.licence = JSON.parse(navigation.extras.state['licence']);
        } else {
            // Handle the case where the licence is not available
            console.error('Licence object not found in the router state.');
        }
    }

    ngOnInit() {
        if (!this.licence) {
            console.log("No licence found.")
        } else {
            console.log("Licence found: ", this.licence);
        }
        this.form = this.fb.group({
            relocationType: ["", Validators.required]
        });
        this.form.valueChanges.subscribe(value => console.log("Form value changed: ", value));
    }

    processApplication() {
        const actionName = this.form.value.relocationType 
        const isTemporaryApplication = this.form.value.relocationType === this.ApplicationTypeNames.LRSTemporaryRelocation;
        // We need to figure out what the action name for temp relocations is
        // We need to get the license (ApplicationLicenseSummary) before we can process this
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
                    this.router.navigateByUrl(`/account-profile/${actionApplication.applicationId}`);
                } else {
                    // prevent a re-submission until the application status is no longer active
                    this.snackBar.open(`${actionName} has already been submitted and is under review`,
                        "Warning",
                        { duration: 3500, panelClass: ["red-snackbar"] });
                }
            } else {
                // We didn't find an application, or this is a temporary relocation application
                this.busy = this.licenceDataService.createApplicationForActionType(this.licence.licenseId, actionName)
                    .pipe(takeWhile(() => this.componentActive))
                    .subscribe(data => {
                        this.router.navigateByUrl(`/account-profile/${data.id}`);
                    },
                        () => {
                            this.snackBar.open(`Error running licence action for ${actionName}`,
                                "Fail",
                                { duration: 3500, panelClass: ["red-snackbar"] });
                        }
                    );
            }
        }
        catch (err) {
            console.error(err);
        }
    }
}
