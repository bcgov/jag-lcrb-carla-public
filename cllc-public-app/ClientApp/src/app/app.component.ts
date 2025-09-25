import { Component, OnInit, Renderer2, ViewChild } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { User } from "@models/user.model";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatSidenav, MatSidenavContainer } from "@angular/material/sidenav";
import { isDevMode } from "@angular/core";
import { Store } from "@ngrx/store";
import { AppState } from "./app-state/models/app-state";
import { filter, map, takeWhile } from "rxjs/operators";
import { FeatureFlagService } from "@services/feature-flag.service";
import { LegalEntity } from "@models/legal-entity.model";
import { AccountDataService } from "@services/account-data.service";
import { FormBase } from "@shared/form-base";
import { SetCurrentAccountAction } from "@app/app-state/actions/current-account.action";
import { Account } from "@models/account.model";
import { VersionInfoDataService } from "@services/version-info-data.service";
import { VersionInfo } from "@models/version-info.model";
import { VersionInfoDialogComponent } from "@components/version-info/version-info-dialog.component";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { MonthlyReport, monthlyReportStatus } from "@models/monthly-report.model";
import { ApplicationDataService } from "@services/application-data.service";
import { EligibilityFormComponent } from "@components/eligibility-form/eligibility-form.component";
import { UserDataService } from "@services/user-data.service";
import { HttpClient } from "@angular/common/http";
import { HttpHeaders } from "@angular/common/http";
import { faInternetExplorer } from "@fortawesome/free-brands-svg-icons";
import { faBell, faBusinessTime } from "@fortawesome/free-solid-svg-icons";
import { Observable, of } from "rxjs";
import { FeedbackComponent } from "@components/feedback/feedback.component";

const Months = [
  "January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
];

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent extends FormBase implements OnInit {
  faInternetExplorer = faInternetExplorer;
  faBell = faBell;
  faBusinessTime = faBusinessTime;
  businessProfiles: LegalEntity[];
  title = "";
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
  testAPIRestul = "";
  @ViewChild('aiSidenav') aiSidenav: MatSidenav;
  showAISearch = false;
  chatCtas: Array<{ label: string; href?: string; routerLink?: string | any[]; params?: any }> = [];


  private orchBase = 'https://lcrb-ai-orch-cudne2ese0ghgtcx.canadacentral-01.azurewebsites.net';
  private readonly SESSION_ID = 'portal-s1';
  chatMessages: Array<{ role: 'user' | 'assistant'; content: string }> = [];
  isBusy = false;
  lastError?: string;

  awaitingField?: { id: string; label: string; type: string; required?: boolean; help?: string };
  activeApplicationId?: string;
  pendingHours?: { open: string; close: string; days: string };
  pendingMinorsPolicy?: string;


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
    private userDataService: UserDataService,
  ) {
    super();

    featureFlagService.featureOn("LicenseeChanges")
      .subscribe(x => this.licenseeChangeFeatureOn = x);

    featureFlagService.featureOn("Eligibility")
      .subscribe(x => this.eligibilityFeatureOn = x);

    this.isDevMode = isDevMode();
    this.router.events
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((event) => {
        if (event instanceof NavigationEnd) {
          if (event.url.search("federal-reporting") >= 0) {
            this.showMessageCenterContent = false;
          } else if (event.url.search("application") >= 0 || event.url.search("event") >= 0) {
            this.reloadUser();
          } else if (event.url.search("personal-history-summary") >= 0 ||
            event.url.search("cannabis-associate-screening") >= 0 ||
            event.url.search("security-screening/confirmation") >= 0) {
            this.showNavbar = false;
          }
          const prevSlug = this.previousUrl;
          let nextSlug = event.url.slice(1);
          if (!nextSlug) {
            nextSlug = "home";
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

  makeAPICall(url: string) {
    this.testAPIRestul = "";
    const headers = new HttpHeaders({
      'Content-Type': "text/html; charset=UTF-8"
    });
    this.httpClient.get<string>(decodeURI(url), { headers })
      .subscribe(
        res => {
          this.testAPIRestul = res.toString();
        },
        err => {
          console.log(err);
        });
  }

  openVersionInfoDialog() {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      data: this.versionInfo
    };

    // open dialog, get reference and process returned data from dialog
    this.dialog.open(VersionInfoDialogComponent, dialogConfig);
  }

  openEligibilityModal() {
    if (!this.isEligibilityDialogOpen) {
      const dialogRef = this.dialog.open(EligibilityFormComponent,
        {
          disableClose: true,
          autoFocus: false,
          maxHeight: "95vh"
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
          if (this.currentUser &&
            this.currentUser.accountid &&
            this.currentUser.accountid !== "00000000-0000-0000-0000-000000000000") {
            this.accountDataService.loadCurrentAccountToStore(this.currentUser.accountid)
              .subscribe(() => {
                if (data.isEligibilityRequired && this.eligibilityFeatureOn) {
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
        .pipe(filter(account => !!account))
        .subscribe(account => {
          this.account = account;
          this.showNoticesBadge$ = this.accountHasNotices(account);
        });
    });
  }

  accountHasNotices(account: Account): Observable<boolean> {
    return this.accountDataService.getFilesAttachedToAccount(account.id, "Notice")
      .pipe(map(files => files?.length > 0));
  }

  showBceidTermsOfUse(): boolean {
    const result = (this.currentUser && this.currentUser.businessname && this.currentUser.isNewUser === true) ||
      (this.account && !this.account.termsOfUseAccepted);
    return result;
  }

  isIE() {
    let result: boolean, jscriptVersion;
    result = false;

    jscriptVersion = new Function("/*@cc_on return @_jscript_version; @*/")();

    if (jscriptVersion !== undefined || !Array.prototype.includes) {
      result = true;
    }
    return result;
  }

  openChat(lang, destinationId) {
    // console.log("Button clicked");
    // [alternative] var baseSite = "https://bclcbr.icescape.com/iceMessaging/";
    var baseSite = "https://16012.icescape.com/iceMessaging/";
    var imHref = baseSite + "Login.html?dId=" + encodeURIComponent(destinationId) + "&lang=" + encodeURIComponent(lang);
    var imWindow = window.open("", "iceIM", "width=490, height=760, resizable=yes, scrollbars=yes");
    if (imWindow && imWindow.location.href === "about:blank") {
      imWindow.location.href = imHref;
    }
    imWindow.focus();
  }

  toggleAISearch() {
    this.aiSidenav?.toggle();
  }

  handleKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.handleSearch();
    }
  }

  handleSearch() {
    const inputEl = document.getElementById('searchInput') as HTMLInputElement;
    const query = inputEl.value.trim();
    if (!query) { return; }
    this.httpClient.post<any>(
      'https://chatmvp-bydygzcccxb5cwa8.canadacentral-01.azurewebsites.net/api/search',
      { query, index: 'portal-index', top: '5' }
    ).subscribe(data => {
      const container = document.getElementById('search-results');
      container.innerHTML = data.summary ? `<div class="search-summary">${data.summary}</div>` : '';
      data.results.forEach(r => {
        container.innerHTML += `
          <div class="search-result-item">
            <a href="${r.url}" target="_blank">${r.title}</a>
          </div>`;
      });
    });
  }

  handleChatKeypress(e: KeyboardEvent) {
  if (e.key === 'Enter') { this.sendChat(); }
}

  sendChat() {
    const input = document.getElementById('assistantInput') as HTMLInputElement;
    const text = input?.value?.trim();
    if (!text) { return; }

    // show user bubble immediately
    this.chatMessages.push({ role: 'user', content: text });
    input.value = '';

    // If we're awaiting a field value, upsert instead of chatting
    if (this.awaitingField && this.activeApplicationId) {
      this.isBusy = true; this.lastError = undefined;

      const field = this.awaitingField;
      let value: any = text;
      if (field.type === 'boolean') {
        value = /^true|yes|y|1$/i.test(text);
      }
      // NOTE: for "compound" hours, you'll supply a small UI later; for now we keep it simple.

      this.upsertField(field.id, value).subscribe({
        next: (r) => {
          // Confirmation
          if (r?.decision === 'warn' && r?.warnings?.length) {
            this.chatMessages.push({ role: 'assistant', content: `Recorded "${field.label}". Warnings:\n- ${r.warnings.join('\n- ')}` });
          } else {
            this.chatMessages.push({ role: 'assistant', content: `Recorded "${field.label}".` });
          }

          // Recompute next field
          this.getReview().subscribe({
            next: (rev) => {
              // find next missing field
              this.fetchFields().subscribe({
                next: (ff) => {
                  const fields = ff?.fields || [];
                  const missing: string[] = rev?.missing || [];
                  const fid = missing[0];
                  const nf = fields.find((f: any) => f.id === fid);
                  if (nf) {
                    this.awaitingField = nf;
                    this.chatMessages.push({
                      role: 'assistant',
                      content: `Next field: ${nf.label} (${nf.id})${nf.required ? ' [required]' : ''}`
                    });
                  } else {
                    this.awaitingField = undefined;
                    // Optional: show warnings summary if any
                    if (rev?.warnings?.length) {
                      this.chatMessages.push({ role: 'assistant', content: `Review warnings:\n- ${rev.warnings.join('\n- ')}` });
                    }
                    this.chatMessages.push({ role: 'assistant', content: 'All required fields are complete. You can upload a floorplan, compute fees, and submit.' });
                  }
                },
                error: () => { this.chatMessages.push({ role: 'assistant', content: 'Could not load fields.' }); }
              });
            },
            error: () => { this.chatMessages.push({ role: 'assistant', content: 'Could not review application.' }); }
          });
        },
        error: () => {
          this.lastError = 'Assistant failed to save your answer.';
          this.chatMessages.push({ role: 'assistant', content: 'Sorry—could not save that. Please try again.' });
        },
        complete: () => { this.isBusy = false; }
      });

      return; // IMPORTANT: do not fall through to /chat
    }

    // Normal chat flow
    this.isBusy = true; this.lastError = undefined;
    const body = { session_id: 'portal-s1', message: text, selected_index: 'portal-index' };
    this.httpClient.post<any>(`${this.orchBase}/chat`, body).subscribe({
      next: (res) => {
        this.chatCtas = res?.ctas || [];
        const a = res?.rag?.answer || res?.rag?.summary || res?.message || 'OK';
        this.chatMessages.push({ role: 'assistant', content: a });

        if (res?.application_id || res?.state?.active_application_id) {
          this.activeApplicationId = res.application_id || res.state.active_application_id;
        }
        if (res?.next_field) {
          this.awaitingField = res.next_field;
          this.chatMessages.push({
            role: 'assistant',
            content: `Next field: ${res.next_field.label} (${res.next_field.id})${res.next_field.required ? ' [required]' : ''}`
          });
        }
      },
      error: () => {
        this.lastError = 'Assistant failed to respond.';
        this.chatMessages.push({ role: 'assistant', content: 'Sorry—something went wrong.' });
      },
      complete: () => { this.isBusy = false; }
    });
  }


  navigateTo(path: string | any[], params?: any) {
    if (Array.isArray(path)) {
      this.router.navigate(path as any, params ? { queryParams: params } : undefined);
    } else {
      this.router.navigate([path], params ? { queryParams: params } : undefined);
    }
  }

  openHref(url?: string) {
    if (url) { window.open(url, '_blank'); }
  }


  openFeedbackDialog() {
     const dialogRef = this.dialog.open(FeedbackComponent, {
        disableClose: true,
        autoFocus: false,
        maxHeight: "95vh"
      });
  }

  fetchFields() {
    return this.httpClient.get<any>(`${this.orchBase}/application/fields`, {
      params: { session_id: this.SESSION_ID }
    });
  }

  upsertField(field_id: string, value: any) {
    const fd = new FormData();
    fd.append('application_id', this.activeApplicationId || '');
    fd.append('session_id', 'portal-s1');
    fd.append('field_id', field_id);
    // hours must be JSON string; booleans as strings
    if (field_id === 'hours') {
      fd.append('value', JSON.stringify(value));
    } else if (typeof value === 'boolean') {
      fd.append('value', String(value));
    } else {
      fd.append('value', String(value));
    }
    return this.httpClient.post<any>(`${this.orchBase}/application/upsert`, fd);
  }

  getReview() {
    return this.httpClient.get<any>(`${this.orchBase}/application/review`, {
      params: { session_id: this.SESSION_ID }
    });
  }

  getFees() {
    return this.httpClient.get<any>(`${this.orchBase}/application/fees`, {
      params: { session_id: this.SESSION_ID }
    });
  }

  submitApplication(attestation = true) {
    const fd = new FormData();
    fd.append('attestation', String(attestation));
    return this.httpClient.post<any>(`${this.orchBase}/application/submit`, fd, {
      params: { session_id: this.SESSION_ID }
    });
  }

  triggerFilePicker() {
    (document.getElementById('floorplanInput') as HTMLInputElement)?.click();
  }

  async handleFloorplanSelected(evt: Event) {
    const input = evt.target as HTMLInputElement;
    const file = input?.files?.[0];
    if (!file) return;

    const fd = new FormData();
    fd.append('file', file);
    this.isBusy = true; this.lastError = undefined;
    this.httpClient.post<any>(`${this.orchBase}/upload/floorplan`, fd, {
      params: { session_id: this.SESSION_ID }
    }).subscribe({
      next: (res) => {
        if (res?.passed) {
          this.chatMessages.push({ role: 'assistant', content: 'Floorplan passed screening and was recorded.' });
        } else {
          this.chatMessages.push({ role: 'assistant', content: `Screening issues:\n- ${res?.reasons?.join('\n- ') || 'Unknown issue'}` });
        }
      },
      error: () => {
        this.lastError = 'Upload failed.';
        this.chatMessages.push({ role: 'assistant', content: 'Sorry—floorplan upload failed.' });
      },
      complete: () => { this.isBusy = false; input.value = ''; }
    });
  }

  handleCta(c: any) {
    if (c.routerLink) {
      this.navigateTo(c.routerLink, c.params);
      return;
    }
    // Label-based fallbacks for now (you can switch to explicit 'action' later)
    const label = (c.label || '').toLowerCase();
    if (label.includes('upload floorplan')) {
      this.triggerFilePicker();
    } else if (label.includes('open application') && this.activeApplicationId) {
      this.navigateTo(['/applications', this.activeApplicationId], c.params);
    } else if (label.includes('compute fees') || label.includes('fees')) {
      this.getFees().subscribe(res => {
        this.chatMessages.push({ role: 'assistant', content: `Estimated fees: $${(res?.total ?? 0).toFixed(2)}` });
      });
    } else if (label.includes('submit')) {
      this.submitApplication(true).subscribe(res => {
        if (res?.ok) {
          this.chatMessages.push({ role: 'assistant', content: `Submitted. Receipt: ${res.receipt_id}` });
        } else {
          this.chatMessages.push({ role: 'assistant', content: `Cannot submit: ${res?.error || 'Unknown error'}` });
        }
      });
    }
  }


}