import { Component, OnInit, Renderer2 } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { User } from '@models/user.model';
import { MatTableDataSource, MatDialog, MatSnackBar, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { isDevMode } from '@angular/core';
import { Store, resultMemoize } from '@ngrx/store';
import { AppState } from './app-state/models/app-state';
import { filter, takeWhile, map } from 'rxjs/operators';
import { FeatureFlagService } from '@services/feature-flag.service';
import { LegalEntity } from '@models/legal-entity.model';
import { AccountDataService } from '@services/account-data.service';
import { FormBase } from '@shared/form-base';
import { SetCurrentAccountAction } from '@app/app-state/actions/current-account.action';
import { SetOngoingLicenseeApplicationIdAction } from '@app/app-state/actions/ongoing-licensee-application-id.action';
import { Account } from '@models/account.model';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { VersionInfo } from '@models/version-info.model';
import { VersionInfoDialogComponent } from '@components/version-info/version-info-dialog.component';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { MonthlyReport, monthlyReportStatus } from '@models/monthly-report.model';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { EligibilityFormComponent } from '@components/eligibility-form/eligibility-form.component';
import { UserDataService } from '@services/user-data.service';

const Months = ['January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'];

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends FormBase implements OnInit {
  businessProfiles: LegalEntity[];
  title = '';
  previousUrl: string;
  public currentUser: User;
  public isNewUser: boolean;
  public isDevMode: boolean;
  public showMap: boolean;
  public versionInfo: VersionInfo;
  isAssociate = false;
  account: Account;
  showMessageCenterContent = false;
  linkedFederalReports: MonthlyReport[];
  Months = Months;  // make available in template
  parseInt = parseInt; // make available in template
  licenseeChangeFeatureOn: boolean;
  isEligibilityDialogOpen: boolean;
  showNavbar = true;

  constructor(
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private renderer: Renderer2,
    private router: Router,
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private applicationDataService: ApplicationDataService,
    public featureFlagService: FeatureFlagService,
    private monthlyReportDataService: MonthlyReportDataService,
    private versionInfoDataService: VersionInfoDataService,
    private userDataService: UserDataService,
  ) {
    super();
    featureFlagService.featureOn('Maps')
      .subscribe(x => this.showMap = x);

    featureFlagService.featureOn('LicenseeChanges')
      .subscribe(x => this.licenseeChangeFeatureOn = x);


    this.isDevMode = isDevMode();
    this.router.events
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((event) => {
        if (event instanceof NavigationEnd) {
          if (event.url.search('federal-reporting') >= 0) {
            this.showMessageCenterContent = false;
          } else if (event.url.search('application') >= 0) {
            this.reloadUser();
          } else if (event.url.search('personal-history-summary') >= 0 || event.url.search('cannabis-associate-screening') >= 0 || event.url.search('security-screening/confirmation') >= 0) {
            this.showNavbar = false;
          }
          const prevSlug = this.previousUrl;
          let nextSlug = event.url.slice(1);
          if (!nextSlug) { nextSlug = 'home'; }
          if (prevSlug) {
            this.renderer.removeClass(document.body, 'ctx-' + prevSlug);
          }
          if (nextSlug) {
            this.renderer.addClass(document.body, 'ctx-' + nextSlug);
          }
          this.previousUrl = nextSlug;
        }
      });
  }

  ngOnInit(): void {
    this.reloadUser();
    this.loadVersionInfo();

    this.store.select(state => state.legalEntitiesState)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(state => !!state))
      .subscribe(state => {
        this.businessProfiles = state.legalEntities;
      });

  }

  loadVersionInfo() {
    this.versionInfoDataService.getVersionInfo()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((versionInfo: VersionInfo) => {
        this.versionInfo = versionInfo;
      });
  }

  openVersionInfoDialog() {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '500px',
      data: this.versionInfo
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(VersionInfoDialogComponent, dialogConfig);
  }

  openEligibilityModal() {
    if (!this.isEligibilityDialogOpen) {
      const dialogRef = this.dialog.open(EligibilityFormComponent, {
        disableClose: true,
        autoFocus: false,
        maxHeight: '95vh'
      });
      this.isEligibilityDialogOpen = true;
      dialogRef.afterClosed().subscribe(() => this.isEligibilityDialogOpen = false);
    }
  }


  reloadUser() {
    this.userDataService.loadUserToStore().then(() => {
      this.store.select(state => state.currentUserState.currentUser)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((data: User) => {
          this.currentUser = data;
          this.isNewUser = this.currentUser && this.currentUser.isNewUser;
          if (this.currentUser && this.currentUser.accountid && this.currentUser.accountid !== '00000000-0000-0000-0000-000000000000') {
            this.accountDataService.loadCurrentAccountToStore(this.currentUser.accountid)
              .subscribe(() => {
                if (data.isEligibilityRequired) {
                  this.openEligibilityModal();
                }
              });

            // load federal reports after the user logs in
            this.monthlyReportDataService.getAllCurrentMonthlyReports(false)
              .subscribe(data => {
                this.linkedFederalReports = data.filter(report => report.statusCode === monthlyReportStatus.Draft);
              });
          } else {
            this.store.dispatch(new SetCurrentAccountAction(null));
          }
        });

      this.store.select(state => state.currentAccountState.currentAccount)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(account => {
          this.account = account;
        });
    });
  }


  showBceidTermsOfUse(): boolean {
    const result = (this.currentUser
      && this.currentUser.businessname
      && this.currentUser.isNewUser === true)
      || (this.account && !this.account.termsOfUseAccepted);
    return result;
  }

  isIE() {
    let result, jscriptVersion;
    result = false;

    jscriptVersion = new Function('/*@cc_on return @_jscript_version; @*/')();

    if (jscriptVersion !== undefined || !Array.prototype.includes) {
      result = true;
    }
    return result;
  }

}
