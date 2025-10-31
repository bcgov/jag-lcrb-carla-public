import { Component, OnInit, OnDestroy, Renderer2, ViewChild, ElementRef, AfterViewInit, AfterViewChecked } from "@angular/core";
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
export class AppComponent extends FormBase implements OnInit, OnDestroy, AfterViewInit, AfterViewChecked {
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
  @ViewChild('msgList') msgList!: ElementRef<HTMLDivElement>
  @ViewChild('bottomAnchor') bottomAnchor!: ElementRef<HTMLDivElement>;
  ngAfterViewInit(): void {
      this.scrollToBottom();
  }
  private lastMsgCount = 0;
  ngAfterViewChecked(){
    if (this.chatMessages.length !== this.lastMsgCount){
      this.lastMsgCount = this.chatMessages.length;
      this.scrollToBottom();
    }
  }
  showAISearch = false;
  chatCtas: Array<{ label: string; href?: string; routerLink?: string | any[]; params?: any }> = [];


  private orchBase = 'https://lcrb-ai-orch-cudne2ese0ghgtcx.canadacentral-01.azurewebsites.net';
  private SESSION_ID = (sessionStorage.getItem('aiSessionId')
    || (sessionStorage.setItem('aiSessionId', 'portal-' + Date.now()), sessionStorage.getItem('aiSessionId'))) as string;

  private introMessage: { role: 'assistant' | 'user'; content: string } = {
    role: 'assistant',
    content: `Hello!<br/>I am an AI assistant that can help you find information, navigate the portal, and complete applications. Just ask a question below or chose any of the available actions. `
  };

  newSession() {
    this.SESSION_ID = 'portal-' + Date.now();
    sessionStorage.setItem('aiSessionId', this.SESSION_ID);
    this.activeApplicationId = undefined;
    this.awaitingField = undefined;
    this.showMockApp = false;
    this.setMode('chat');
    this.chatMessages = [this.introMessage];
    this.chatCtas = [];
    this.scrollToBottom();
  }

  private scrollToBottom(){
    requestAnimationFrame(() => {
      const el = this.bottomAnchor?.nativeElement;
      if (el) el.scrollIntoView({ behavior: 'auto', block: 'end' });
    });
  }

  quickAction(kind: 'renewals' | 'startLP' | 'validateDocs') {
  if (kind === 'renewals') {
    const input = document.getElementById('assistantInput') as HTMLInputElement;
    if (input) input.value = 'check my upcoming licence renewals';
    this.sendChat();
    // this.chatMessages.push({ role: 'assistant', content: 'You currently do not have any upcoming licence renewals.' });
    // this.scrollToBottom();
  } else if (kind === 'startLP') {
    const input = document.getElementById('assistantInput') as HTMLInputElement;
    if (input) input.value = 'start a liquor primary application';
    this.sendChat();
  } else if (kind === 'validateDocs') {
    this.chatCtas = [{ label: 'Upload Floorplan' }];
  }
}

  
  chatMessages: Array<{ role: 'user' | 'assistant'; content: string }> = [];
  isBusy = false;
  lastError?: string;

  private portalApiToken?: string; // stores the short-lived Portal API delegation token

  awaitingField?: { id: string; label: string; type: string; required?: boolean; help?: string };
  activeApplicationId?: string;
  pendingHours?: { open: string; close: string; days: string };
  pendingMinorsPolicy?: string;

  // Demo iframe wiring
  showMockApp = false;
  private get demoFrame(): HTMLIFrameElement | null {
    return document.getElementById('demoApp') as HTMLIFrameElement | null;
  }
  private postToMock(msg: any) {
    try { this.demoFrame?.contentWindow?.postMessage(msg, '*'); } catch {}
  }
  private openMockAppOnce() {
    if (!this.activeApplicationId) { return; }
    if (!this.showMockApp) { this.showMockApp = true; }
  }

  private handleMockMessage = (evt: MessageEvent) => {
    const msg = evt?.data || {};
    if (!msg || typeof msg !== 'object') return;
    switch (msg.type) {
      case 'mock:init':
      case 'mock:requestState':
        this.refreshMockState();
        break;
      case 'mock:setField': {
        const { field_id, value } = msg;
        if (!field_id) return;
        this.upsertField(field_id, value).subscribe({
          next: () => this.refreshMockState(),
          error: () => this.postToMock({ type: 'toast', level: 'error', text: 'Could not save field.' })
        });
        break;
      }
      case 'mock:submit':
        this.submitApplication(true).subscribe({
          next: (res) => this.postToMock({ type: 'submitted', ok: !!res?.ok, receipt_id: res?.receipt_id }),
          error: () => this.postToMock({ type: 'toast', level: 'error', text: 'Submit failed.' })
        });
        break;
      default:
        break;
    }
  };

  private refreshMockState() {
    // Guard for no active application
    if (!this.activeApplicationId){
      this.postToMock({ type: 'state', application_id: null, next_field: null, missing: [], warnings: [], fields: [] });
      return;
    }
    // Gather latest application state for the iframe
    const afterFields = (fields: any[]) => {
      this.getReview().subscribe({
        next: (rev) => {
          this.postToMock({
            type: 'state',
            application_id: this.activeApplicationId || null,
            next_field: this.awaitingField || null,
            missing: rev?.missing || [],
            warnings: rev?.warnings || [],
            fields: fields || []
          });
        },
        error: () => this.postToMock({ type: 'toast', level: 'error', text: 'Could not load review.' })
      });
    };
    this.fetchFields().subscribe({
      next: (ff) => afterFields(ff?.fields || []),
      error: () => afterFields([])
    });
  }

  private computeNextFieldPrompt() {
    const body = { session_id: this.SESSION_ID, message: 'next field', selected_index: 'portal-index' };
    this.httpClient.post<any>(`${this.orchBase}/chat`, body, this.authHeaders()).subscribe({
      next: (r) => {
        const nf = r?.next_field;
        if (nf) {
          this.awaitingField = nf;
          this.setMode('app');
          if (this.chatMessages[this.chatMessages.length - 1]?.content?.includes(nf.label)) return;
          this.chatMessages.push({
            role: 'assistant',
            content: `Next field: <strong>${nf.label}</strong>${nf.required ? ' <span class="required-text">*Required</span>' : ''}`
          });
          this.scrollToBottom();
          // this.fieldSummary(nf); // optional
          if (this.showMockApp) { this.refreshMockState(); }
        }
      }
    });
  }


  private fieldSummary(field: { id: string; label: string }) {
    const prompt = `You are a form assistant gathering input on the next form field. Briefly explain the "${field.label}" field, then ask the user for the value for their application. Keep it brief and simple using natural language`;
    const body = { session_id: this.SESSION_ID, message: prompt, selected_index: 'portal-index' };
    this.ensureDelegationToken().catch(() => {});
    this.httpClient.post<any>(`${this.orchBase}/chat`, body, this.authHeaders()).subscribe({
      next: (res) => {
        const text = res?.rag?.answer || res?.rag?.summary || res?.message;
        if (text) {
          this.chatMessages.push({ role: 'assistant', content: text });
          this.scrollToBottom();
        } 
      }
    });
  }

  // Chat modes (Chat or Application Assistant)
  mode: 'chat' | 'app' = 'chat';
  private defaultAppCtas () {
    return [{ label: this.showMockApp ? 'Close Application' : 'Open Application' }, { label: 'Upload Floorplan' }];
  }

  private setMode(m: 'chat' | 'app'){
    this.mode = m;
    if (m === 'chat'){
      this.awaitingField = undefined;
      this.chatCtas = [];
      this.showMockApp = false;
    } else {
      if (!this.chatCtas?.length) {
        this.chatCtas = this.defaultAppCtas();
      }
    }
  }
  enterApplicationMode() {
    this.setMode('app');
    this.computeNextFieldPrompt?.()
  }

  exitApplicationMode() {
    this.setMode('chat');
  }

  skipField() {
    this.awaitingField = undefined;
    this.computeNextFieldPrompt?.();
  }

  explainField() {
    if (this.awaitingField) {
      this.fieldSummary?.(this.awaitingField);
    }
  }

  resumeApplication(){
    if (this.activeApplicationId) {
      this.setMode('app');
      this.computeNextFieldPrompt?.();
    }
  }
  // This is Observable will be set to true when there are e-notices attached to the current account.
  // The value determines whether or not to display a warning badge for the "Notices" link in the NavBar.
  showNoticesBadge$ = of(false);

  // helper to call the Portal API to mint a token (uses existing SiteMinder session)
  private async ensureDelegationToken(): Promise<void> {
    if (this.portalApiToken) return;
    const res = await this.httpClient.post<any>('/lcrb/api/auth/delegation-token', {}).toPromise();
    this.portalApiToken = res?.access_token;
    console.log('Delegation token acquired', res?.access_token);
  }

  // helper to include Authorization header when talking to orchestrator
  private authHeaders(): { headers?: HttpHeaders } {
    return this.portalApiToken
      ? { headers: new HttpHeaders({ Authorization: `Bearer ${this.portalApiToken}` }) }
      : {};
  }

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
    window.addEventListener('message', this.handleMockMessage);

    this.store.select(state => state.legalEntitiesState)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(state => !!state))
      .subscribe(state => {
        this.businessProfiles = state.legalEntities;
      });

    if (!this.chatMessages.length) {
      this.chatMessages.push(this.introMessage);
      this.scrollToBottom();
    }
  }

  ngOnDestroy(): void {
    window.removeEventListener('message', this.handleMockMessage);
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

  async toggleAISearch() {
    this.aiSidenav?.toggle();

    // on open, mint a short-lived token once per session
    if (this.showAISearch && !this.portalApiToken) {
      try {
        await this.ensureDelegationToken();
        console.log('Obtained delegation token for AI services.');
      } catch {
        this.lastError = 'Could not authenticate to AI services.';
        console.log('Could not obtain delegation token for AI services.');
      }
    }
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

  async sendChat() {
    const input = document.getElementById('assistantInput') as HTMLInputElement;
    const text = input?.value?.trim();
    if (!text) { return; }

    // show user bubble immediately
    this.chatMessages.push({ role: 'user', content: text });
    this.scrollToBottom();
    input.value = '';

    // If awaiting a field value, upsert instead of chatting
    if (this.awaitingField && this.activeApplicationId) {
      this.isBusy = true; this.lastError = undefined;

      const field = this.awaitingField;
      let value: any = text;
      if (field.type === 'boolean') {
        value = /^true|yes|y|1$/i.test(text);
      }
      // TODO: for "compound" hours, supply a small UI later

      // No token needed here when using orchestrator for upsert.
      // If this is call is switched to Portal API, use this.authHeaders().

      this.upsertField(field.id, value).subscribe({
        next: (r) => {
          if (r?.decision === 'warn' && r?.warnings?.length) {
            this.chatMessages.push({ role: 'assistant', content: `Saved "${field.label}". Warnings:\n- ${r.warnings.join('\n- ')}` });
            this.scrollToBottom();
          } else {
            this.chatMessages.push({ role: 'assistant', content: `Saved "${field.label}".` });
            this.scrollToBottom();
          }
          if (this.showMockApp) {
            this.postToMock({ type: 'setField', id: field.id, value });
          }
          this.getReview().subscribe({
            next: (rev) => {
              this.fetchFields().subscribe({
                next: (ff) => {
                  const fields = ff?.fields || [];
                  const missing: string[] = rev?.missing || [];
                  const fid = missing[0];
                  const nf = fields.find((f: any) => f.id === fid);
                  if (nf) {
                    this.awaitingField = nf;
                    this.mode = 'app';
                    this.chatMessages.push({
                      role: 'assistant',
                      content: `Next field: <strong>${nf.label}</strong>${nf.required ? ' <span class="required-text">*Required</span>' : ''}`
                    });
                    this.scrollToBottom();
                    if (this.showMockApp) { this.refreshMockState(); }
                  } else {
                    this.awaitingField = undefined;
                    if (rev?.warnings?.length) {
                      this.chatMessages.push({ role: 'assistant', content: `Review warnings:\n- ${rev.warnings.join('\n- ')}` });
                      this.scrollToBottom();
                    }
                    this.chatMessages.push({ role: 'assistant', content: 'All required fields are complete. You can upload a floorplan, compute fees, and submit.' });
                    this.scrollToBottom();
                    if (this.showMockApp) { this.refreshMockState(); }
                  }
                },
                error: () => { this.chatMessages.push({ role: 'assistant', content: 'Could not load fields.' }); this.scrollToBottom(); }
              });
            },
            error: () => { this.chatMessages.push({ role: 'assistant', content: 'Could not review application.' }); this.scrollToBottom(); }
          });
        },
        error: () => {
          this.lastError = 'Assistant failed to save your answer.';
          this.chatMessages.push({ role: 'assistant', content: 'Sorry—could not save that. Please try again.' });
          this.scrollToBottom();
        },
        complete: () => { this.isBusy = false; }
      });

      return; // IMPORTANT: do not fall through to /chat
    }

    // Normal chat flow
    this.isBusy = true; this.lastError = undefined;

    // make sure we have a token before the first orchestrator call
    try {
      await this.ensureDelegationToken();
    } catch {
      // non-fatal: orchestrator may still accept the call without auth if left open
    }

    const body = { session_id: this.SESSION_ID, message: text, selected_index: 'portal-index' };

    const makeCall = () => this.httpClient.post<any>(
      `${this.orchBase}/chat`,
      body,
      this.authHeaders() // include Authorization header
    );

    const doRequest = () => makeCall().subscribe({
      next: (res) => {
        this.chatCtas = this.mode === 'app'
          ? ((res?.ctas && res.ctas.length) ? res.ctas : this.defaultAppCtas())
          : (res?.ctas || []).filter(c =>
              (c?.href || c?.routerLink) &&
              !/(^|\b)(open|close)\s+application\b|upload\s+floorplan|fees|submit/i.test(c?.label || '')
            );

        // clear default app CTAs if we’re in chat mode
        if (this.mode === 'chat' && !res?.ctas?.length) {
          this.chatCtas = [];
        }

        const a = res?.rag?.answer || res?.rag?.summary || res?.message || 'OK';
        if (res?.intent === 'START_APPLICATION'){
          this.setMode('app');
          this.chatMessages.push({
            role: 'assistant',
            content: 'Started a Liquor Primary application draft. Your responses will be saved as field inputs for the application. Click the "More" button from the application toolbar to get more information on a field or click "Exit" at any time continue chatting.'
          })
          this.scrollToBottom();
        } else {
            this.chatMessages.push({ role: 'assistant', content: a });
            this.scrollToBottom();
        }

        if (res?.application_id || res?.state?.active_application_id) {
          this.activeApplicationId = res.application_id || res.state.active_application_id;
          if (this.mode === 'app') {
              this.chatCtas = (res?.ctas && res.ctas.length) ? res.ctas : [
                {label: this.showMockApp? 'Close Application' : 'Open Application'},
                {label: 'Upload Floorplan'}
              ]
          }
        }
        if (res?.intent === 'START_APPLICATION' && (res?.application_id || res?.state?.active_application_id)) {
          this.computeNextFieldPrompt();
        }
        if (res?.next_field && this.mode === 'app') {
          this.awaitingField = res.next_field;
          this.chatMessages.push({
            role: 'assistant',
            content: `Next field: <strong>${res.next_field.label}</strong>${res.next_field.required ? ' <span class="required-text">*Required</span>' : ''}`
          });
          this.scrollToBottom();
          // this.fieldSummary(res.next_field);
        }
        if (!res?.next_field && this.activeApplicationId && this.mode === 'app') { this.computeNextFieldPrompt(); }

      },
      // error: async (err) => {
      //   // auto-refresh token on 401 and retry once
      //   if (err?.status === 401) {
      //     try {
      //       this.portalApiToken = undefined;
      //       await this.ensureDelegationToken();
      //       makeCall().subscribe({
      //         next: (res2) => {
      //           this.chatCtas = res2?.ctas || [];
      //           const a2 = res2?.rag?.answer || res2?.rag?.summary || res2?.message || 'OK';
      //           this.chatMessages.push({ role: 'assistant', content: a2 });
      //           if (res2?.application_id || res2?.state?.active_application_id) {
      //             this.activeApplicationId = res2.application_id || res2.state.active_application_id;
      //             // keep the mock page in sync if it is visible
      //             if (this.showMockApp) { this.refreshMockState(); }
      //           }
      //           if (res2?.intent === 'START_APPLICATION' && !res2?.next_field) {
      //             this.mode = 'app';
      //             this.computeNextFieldPrompt();
      //           }
      //           if (res2?.next_field) {
      //             this.awaitingField = res2.next_field;
      //             this.mode = 'app';
      //             this.chatMessages.push({
      //               role: 'assistant',
      //               content: `Next field: ${res2.next_field.label} (${res2.next_field.id})${res2.next_field.required ? ' [required]' : ''}`
      //             });
      //             // this.fieldSummary(res2.next_field);
      //             if (this.showMockApp) { this.refreshMockState(); }
      //           }
      //           if (!res2?.next_field && this.activeApplicationId) { this.computeNextFieldPrompt(); }
      //         },
      //         error: () => {
      //           this.lastError = 'Assistant failed to respond.';
      //           this.chatMessages.push({ role: 'assistant', content: 'Sorry—something went wrong.' });
      //         },
      //         complete: () => { this.isBusy = false; }
      //       });
      //       return;
      //     } catch {
      //       // fall through to default error handling
      //     }
      //   }
      //   this.lastError = 'Assistant failed to respond.';
      //   this.chatMessages.push({ role: 'assistant', content: 'Sorry—something went wrong.' });
      // },
      complete: () => { this.isBusy = false; }
    });

    doRequest();
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
    fd.append('session_id', this.SESSION_ID);
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
    const url = this.activeApplicationId
      ? `${this.orchBase}/upload/floorplan`
      : `${this.orchBase}/validate/floorplan`;

    const params = this.activeApplicationId ? { session_id: this.SESSION_ID } : undefined;

    this.httpClient.post<any>(url, fd, { params }).subscribe({
      next: (res) => {
        const passed = res?.passed ?? res?.screening?.passed;
        const reasons: string[] = res?.reasons ?? (res?.screening?.issues || []).map((i: any) => i?.message).filter(Boolean);
        this.chatMessages.push({
          role: 'assistant',
          content: passed
            ? (this.activeApplicationId
                ? 'Floorplan passed screening and was recorded.'
                : 'Floorplan passed screening.')
            : `Screening issues:\n- ${reasons.length ? reasons.join('\n- ') : 'Unknown issue'}`
        });
        this.scrollToBottom();
      },
      error: () => {
        this.lastError = 'Upload failed.';
        this.chatMessages.push({ role: 'assistant', content: 'Sorry—floorplan upload failed.' });
        this.scrollToBottom();
      },
      complete: () => { this.isBusy = false; input.value = ''; }
    });

  }

  handleCta(c: any) {
    const label = (c.label || '').toLowerCase();

    // Detect and handle mock app CTAs
    if (label.includes('open application') || (c.href && c.href.includes('/assets/mock'))) {
      if (!this.activeApplicationId) {
        this.chatMessages.push({ role: 'assistant', content: 'No active application to open.' });
        this.scrollToBottom();
        return;
      }
      this.openMockAppOnce();
      setTimeout(() => this.refreshMockState(), 300);
      this.chatCtas = this.defaultAppCtas();
      return;
    }
    if (label.includes('close application')) {
      this.showMockApp = false;
      this.chatCtas = this.defaultAppCtas();
      return;
    }

    // Fallbacks
    if (c.href) {
      this.openHref(c.href);
      return;
    }
    if (c.routerLink) {
      this.navigateTo(c.routerLink, c.params);
      return;
    }

    // Label-based fallbacks
    if (label.includes('upload floorplan')) {
      this.triggerFilePicker();
    } else if (label.includes('compute fees') || label.includes('fees')) {
      this.getFees().subscribe(res => {
        this.chatMessages.push({ role: 'assistant', content: `Estimated fees: $${(res?.total ?? 0).toFixed(2)}` });
        this.scrollToBottom();
      });
    } else if (label.includes('submit')) {
      this.submitApplication(true).subscribe(res => {
        if (res?.ok) {
          this.chatMessages.push({ role: 'assistant', content: `Submitted. Receipt: ${res.receipt_id}` });
          this.scrollToBottom();
        } else {
          this.chatMessages.push({ role: 'assistant', content: `Cannot submit: ${res?.error || 'Unknown error'}` });
          this.scrollToBottom();
        }
      });
    }
  }



}