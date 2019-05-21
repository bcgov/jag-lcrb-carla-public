import { Component, OnInit, Renderer2 } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { InsertService } from './insert/insert.service';
import { UserDataService } from './services/user-data.service';
import { User } from './models/user.model';
import { isDevMode } from '@angular/core';
import { LegalEntityDataService } from './services/legal-entity-data.service';
import { Store } from '@ngrx/store';
import { AppState } from './app-state/models/app-state';
import { Observable } from '../../node_modules/rxjs';
import * as CurrentUserActions from './app-state/actions/current-user.action';
import * as CurrentAccountActions from './app-state/actions/current-account.action';
import { filter } from 'rxjs/operators';
import { FeatureFlagService } from '@services/feature-flag.service';
import { LegalEntity } from '@models/legal-entity.model';
import { CurrentAccountAction } from './app-state/actions/current-account.action';
import { AccountDataService } from '@services/account-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { BCeidAuthGuard } from '@services/bceid-auth-guard.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  businessProfiles: LegalEntity[];
  title = '';
  previousUrl: string;
  public currentUser: User;
  public isNewUser: boolean;
  public isDevMode: boolean;
  isAssociate = false;


  constructor(
    private renderer: Renderer2,
    private router: Router,
    private userDataService: UserDataService,
    private store: Store<AppState>,
    private adoxioLegalEntityDataService: LegalEntityDataService,
    private bCeidAuthGuard: BCeidAuthGuard,
    private accountDataService: AccountDataService,
    public featureFlagService: FeatureFlagService,
    private tiedHouseService: TiedHouseConnectionsDataService
  ) {
    this.isDevMode = isDevMode();
    if (!featureFlagService.initialized) {
      featureFlagService.initialize();
    }
    this.router.events.subscribe((event) => {
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
    this.reloadUser();

    this.store.select(state => state.legalEntitiesState)
      .pipe(filter(state => !!state))
      .subscribe(state => {
        this.businessProfiles = state.legalEntities;
      });

  }

  reloadUser() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(filter(u => !!u))
      .subscribe((data: User) => {
        this.currentUser = data;
        this.isNewUser = this.currentUser.isNewUser;
        if (this.currentUser && this.currentUser.accountid && this.currentUser.accountid !== '00000000-0000-0000-0000-000000000000') {
          this.accountDataService.loadCurrentAccountToStore(this.currentUser.accountid)
          .subscribe(() => {});
        }
      });
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
