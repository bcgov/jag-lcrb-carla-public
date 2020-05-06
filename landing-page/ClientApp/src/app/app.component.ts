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


  showBceidTermsOfUse(): boolean {
    const result = (this.currentUser
      && this.currentUser.businessname
      && this.currentUser.isNewUser === true)
      || (this.account && !this.account.termsOfUseAccepted);
    return result;
  }

  isIE10orLower() {
    let result, jscriptVersion;
    result = false;

    jscriptVersion = new Function('/*@cc_on return @_jscript_version; @*/')();

    if (jscriptVersion !== undefined) {
      result = true;
    }
    return result;
  }

}
