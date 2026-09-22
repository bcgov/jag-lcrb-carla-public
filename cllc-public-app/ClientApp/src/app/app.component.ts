import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, isDevMode, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationEnd, Router } from '@angular/router';
import { SetCurrentAccountAction } from '@app/app-state/actions/current-account.action';
import { EligibilityFormComponent } from '@components/eligibility-form/eligibility-form.component';
import { FeedbackComponent } from '@components/feedback/feedback.component';
import { VersionInfoDialogComponent } from '@components/version-info/version-info-dialog.component';
import { faInternetExplorer } from '@fortawesome/free-brands-svg-icons';
import { faBell, faBusinessTime } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { LegalEntity } from '@models/legal-entity.model';
import { MonthlyReport, monthlyReportStatus } from '@models/monthly-report.model';
import { User } from '@models/user.model';
import { VersionInfo } from '@models/version-info.model';
import { Store } from '@ngrx/store';
import { AccountDataService } from '@services/account-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { UserDataService } from '@services/user-data.service';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { FormBase } from '@shared/form-base';
import { Observable, of } from 'rxjs';
import { filter, map, takeWhile } from 'rxjs/operators';
import { AppState } from './app-state/models/app-state';

const Months = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December'
];

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends FormBase implements OnInit, OnDestroy {
  faInternetExplorer = faInternetExplorer;
  faBell = faBell;
  faBusinessTime = faBusinessTime;
  businessProfiles: LegalEntity[];
  title = '';
  previousUrl: string;
  currentUser: User;
  isNewUser: boolean;
  isDevMode: boolean;
  versionInfo: VersionInfo;
  account: Account;
  showMessageCenterContent = false;
  linkedFederalReports: MonthlyReport[];
  Months = Months; // make available in template
  parseInt = parseInt; // make available in template
  licenseeChangeFeatureOn: boolean;
  eligibilityFeatureOn: boolean;
  isEligibilityDialogOpen: boolean;
  showNavbar = true;
  isOnSepDashboard = false;
  testAPIResult = '';
  mockSchema: any;
  mockValues: Record<string, any> = {};
  uploadStatuses: Record<string, 'ok' | 'missing'> = {};
  // This is Observable will be set to true when there are e-notices attached to the current account.
  // The value determines whether or not to display a warning badge for the "Notices" link in the NavBar.
  showNoticesBadge$ = of(false);

  constructor(
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private renderer: Renderer2,
    private httpClient: HttpClient,
    private router: Router,
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private applicationDataService: ApplicationDataService,
    public featureFlagService: FeatureFlagService,
    private monthlyReportDataService: MonthlyReportDataService,
    private versionInfoDataService: VersionInfoDataService,
    private userDataService: UserDataService
  ) {
    super();

    featureFlagService.featureOn('LicenseeChanges').subscribe((x) => (this.licenseeChangeFeatureOn = x));

    featureFlagService.featureOn('Eligibility').subscribe((x) => (this.eligibilityFeatureOn = x));


    this.isDevMode = isDevMode();
    this.router.events.pipe(takeWhile(() => this.componentActive)).subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.isOnSepDashboard = event.url.includes('sep/dashboard');
        if (event.url.search('federal-reporting') >= 0) {
          this.showMessageCenterContent = false;
        } else if (event.url.search('application') >= 0 || event.url.search('event') >= 0) {
          this.reloadUser();
        } else if (
          event.url.search('personal-history-summary') >= 0 ||
          event.url.search('cannabis-associate-screening') >= 0 ||
          event.url.search('security-screening/confirmation') >= 0
        ) {
          this.showNavbar = false;
        }
        const prevSlug = this.previousUrl;
        let nextSlug = event.url.slice(1);
        if (!nextSlug) {
          nextSlug = 'home';
        }
        if (prevSlug) {
          this.renderer.removeClass(document.body, `ctx-${prevSlug}`);
        }
        if (nextSlug) {
          this.renderer.addClass(document.body, `ctx-${nextSlug}`);
        }
        this.previousUrl = nextSlug;
      }
    });
  }

  ngOnInit(): void {
    this.reloadUser();
    this.loadVersionInfo();
    // window.addEventListener('message', this.handleMockMessage);
    this.httpClient.get('/lcrb/assets/mock-app.schema.json').subscribe((schema) => {
      this.mockSchema = schema;
    });
    this.store
      .select((state) => state.legalEntitiesState)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter((state) => !!state))
      .subscribe((state) => {
        this.businessProfiles = state.legalEntities;
      });
  }

  ngOnDestroy(): void {
    // window.removeEventListener('message', this.handleMockMessage);
  }

  loadVersionInfo() {
    this.versionInfoDataService
      .getVersionInfo()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((versionInfo: VersionInfo) => {
        this.versionInfo = versionInfo;
      });
  }

  makeAPICall(url: string) {
    this.testAPIResult = '';
    const headers = new HttpHeaders({
      'Content-Type': 'text/html; charset=UTF-8'
    });
    this.httpClient.get<string>(decodeURI(url), { headers }).subscribe(
      (res) => {
        this.testAPIResult = res.toString();
      },
      (err) => {
        console.log(err);
      }
    );
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
    this.dialog.open(VersionInfoDialogComponent, dialogConfig);
  }

  openEligibilityModal() {
    if (!this.isEligibilityDialogOpen) {
      const dialogRef = this.dialog.open(EligibilityFormComponent, {
        disableClose: true,
        autoFocus: false,
        maxHeight: '95vh'
      });
      this.isEligibilityDialogOpen = true;
      dialogRef.afterClosed().subscribe(() => (this.isEligibilityDialogOpen = false));
    }
  }

  reloadUser() {
    this.userDataService.loadUserToStore().then(() => {
      this.store
        .select((state) => state.currentUserState.currentUser)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((data: User) => {
          this.currentUser = data;
          this.isNewUser = this.currentUser && this.currentUser.isNewUser;
          if (
            this.currentUser &&
            this.currentUser.accountid &&
            this.currentUser.accountid !== '00000000-0000-0000-0000-000000000000'
          ) {
            this.accountDataService.loadCurrentAccountToStore(this.currentUser.accountid).subscribe(() => {
              if (data.isEligibilityRequired && this.eligibilityFeatureOn) {
                this.openEligibilityModal();
              }
            });

            // load federal reports after the user logs in
            this.monthlyReportDataService.getAllCurrentMonthlyReports(false).subscribe((data) => {
              this.linkedFederalReports = data.filter((report) => report.statusCode === monthlyReportStatus.Draft);
            });
          } else {
            this.store.dispatch(new SetCurrentAccountAction(null));
          }
        });

      this.store
        .select((state) => state.currentAccountState.currentAccount)
        .pipe(takeWhile(() => this.componentActive))
        .pipe(filter((account) => !!account))
        .subscribe((account) => {
          this.account = account;
          this.showNoticesBadge$ = this.accountHasNotices(account);
        });
    });
  }

  accountHasNotices(account: Account): Observable<boolean> {
    return this.accountDataService
      .getFilesAttachedToAccount(account.id, 'Notice')
      .pipe(map((files) => files?.length > 0));
  }

  showBceidTermsOfUse(): boolean {
    const result =
      (this.currentUser && this.currentUser.businessname && this.currentUser.isNewUser === true) ||
      (this.account && !this.account.termsOfUseAccepted);
    return result;
  }

  isIE() {
    let result: boolean, jscriptVersion;
    result = false;

    jscriptVersion = new Function('/*@cc_on return @_jscript_version; @*/')();

    if (jscriptVersion !== undefined || !Array.prototype.includes) {
      result = true;
    }
    return result;
  }

  openChat(lang, destinationId) {
    // console.log("Button clicked");
    // [alternative] var baseSite = "https://bclcbr.icescape.com/iceMessaging/";
    var baseSite = 'https://16012.icescape.com/iceMessaging/';
    var imHref = baseSite + 'Login.html?dId=' + encodeURIComponent(destinationId) + '&lang=' + encodeURIComponent(lang);
    var imWindow = window.open('', 'iceIM', 'width=490, height=760, resizable=yes, scrollbars=yes');
    if (imWindow && imWindow.location.href === 'about:blank') {
      imWindow.location.href = imHref;
    }
    imWindow.focus();
  }

  navigateTo(path: string | any[], params?: any) {
    if (Array.isArray(path)) {
      this.router.navigate(path as any, params ? { queryParams: params } : undefined);
    } else {
      this.router.navigate([path], params ? { queryParams: params } : undefined);
    }
  }

  openHref(url?: string) {
    if (url) {
      window.open(url, '_blank');
    }
  }

  openFeedbackDialog() {
    const dialogRef = this.dialog.open(FeedbackComponent, {
      disableClose: true,
      autoFocus: false,
      maxHeight: '95vh'
    });
  }

}
