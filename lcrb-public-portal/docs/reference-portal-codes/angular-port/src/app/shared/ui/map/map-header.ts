/** Top toolbar with buttons that open map panels and mock login/settings dialogs. Holds no map data; only raises *Toggle outputs. Gated by injectAuth() mock. */
import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { LoginDialog } from './login-dialog';
import { injectAuth } from './map-auth';
import { SettingsPanel } from './settings-panel';
import { Button } from '../primitives/button';
import { LucideIcon } from '../primitives/lucide-icon';

@Component({
  selector: 'map-header',
  imports: [Button, LucideIcon, LoginDialog, SettingsPanel],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <header class="relative z-30 bg-sidebar text-sidebar-foreground shadow-md">
      <div class="flex items-center justify-between px-4 h-14">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 bg-sidebar-primary rounded flex items-center justify-center">
            <span class="text-sidebar-primary-foreground font-bold text-sm">BC</span>
          </div>
          <div class="hidden sm:block">
            <h1 class="font-semibold text-sm leading-tight">Licensing &amp; Events Map</h1>
            <p class="text-xs text-sidebar-foreground/70">Liquor &amp; cannabis locations</p>
          </div>
        </div>

        <div class="hidden md:flex items-center gap-2">
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="searchToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isSearchOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="Search" class="w-4 h-4 mr-2"></svg>
              Search
            </button>
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="filterToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isFilterOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="Filter" class="w-4 h-4 mr-2"></svg>
              Filter
            </button>
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="locateUser.emit()"
              class="text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent"
            >
              <svg lucide="Navigation" class="w-4 h-4 mr-2"></svg>
              My Location
            </button>

            @if (canViewInsights()) {
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="insightsToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isInsightsOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="TrendingUp" class="w-4 h-4 mr-2"></svg>
              Insights
            </button>
            }

            @if (showFollowingMenu()) {
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="followingToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isFollowingOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="Heart" class="w-4 h-4 mr-2"></svg>
              Following
            </button>
            }

            @if (auth.isStaff()) {
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="pipelineToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isPipelineOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="FileText" class="w-4 h-4 mr-2"></svg>
              Pipeline
            </button>
            }

            @if (auth.isStaff()) {
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="inspectionPlannerToggle.emit()"
              [class]="'text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent ' + (isInspectionPlannerOpen() ? 'bg-sidebar-accent' : '')"
            >
              <svg lucide="ClipboardList" class="w-4 h-4 mr-2"></svg>
              Inspections
            </button>
            }

            @if (auth.isAdmin()) {
            <button
              uiButton
              variant="ghost"
              size="sm"
              (click)="showSettings.set(true)"
              class="text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent"
            >
              <svg lucide="Settings" class="w-4 h-4 mr-2"></svg>
              Settings
            </button>
            }

            <div class="w-px h-6 bg-sidebar-border mx-2"></div>

            @if (auth.isAuthenticated()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="showLogin.set(true)"
                class="flex items-center gap-2 px-3 py-1.5 bg-sidebar-accent rounded-md text-sidebar-foreground hover:bg-sidebar-accent/80"
              >
                <svg lucide="User" class="w-4 h-4"></svg>
                <span class="text-sm">{{ auth.user()?.name }}</span>
              </button>
            } @else {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="showLogin.set(true)"
                class="text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent"
              >
                <svg lucide="LogIn" class="w-4 h-4 mr-2"></svg>
                Login
              </button>
            }
          </div>

          <button uiButton variant="ghost" size="icon" class="md:hidden text-sidebar-foreground" (click)="showMobileMenu.set(!showMobileMenu())">
            @if (showMobileMenu()) {
              <svg lucide="X" class="w-5 h-5"></svg>
            } @else {
              <svg lucide="Menu" class="w-5 h-5"></svg>
            }
          </button>
        </div>

        @if (showMobileMenu()) {
          <div class="md:hidden border-t border-sidebar-border bg-sidebar p-4 space-y-2">
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="searchToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isSearchOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="Search" class="w-4 h-4 mr-2"></svg>
                Search
              </button>
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="filterToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isFilterOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="Filter" class="w-4 h-4 mr-2"></svg>
                Filter
              </button>
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="locateUser.emit(); showMobileMenu.set(false)"
                class="w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent"
              >
                <svg lucide="Navigation" class="w-4 h-4 mr-2"></svg>
                My Location
              </button>

            @if (canViewInsights()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="insightsToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isInsightsOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="TrendingUp" class="w-4 h-4 mr-2"></svg>
                Insights
              </button>
            }

            @if (showFollowingMenu()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="followingToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isFollowingOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="Heart" class="w-4 h-4 mr-2"></svg>
                Following
              </button>
            }

            @if (auth.isStaff()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="pipelineToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isPipelineOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="FileText" class="w-4 h-4 mr-2"></svg>
                Pipeline
              </button>
            }

            @if (auth.isStaff()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="inspectionPlannerToggle.emit(); showMobileMenu.set(false)"
                [class]="'w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent ' + (isInspectionPlannerOpen() ? 'bg-sidebar-accent' : '')"
              >
                <svg lucide="ClipboardList" class="w-4 h-4 mr-2"></svg>
                Inspections
              </button>
            }

            @if (auth.isAdmin()) {
              <button
                uiButton
                variant="ghost"
                size="sm"
                (click)="showSettings.set(true); showMobileMenu.set(false)"
                class="w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent"
              >
                <svg lucide="Settings" class="w-4 h-4 mr-2"></svg>
                Settings
              </button>
            }

            <div class="border-t border-sidebar-border pt-2 mt-2">
              @if (auth.isAuthenticated()) {
                <button
                  uiButton
                  variant="ghost"
                  size="sm"
                  (click)="showLogin.set(true); showMobileMenu.set(false)"
                  class="w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent"
                >
                  <svg lucide="User" class="w-4 h-4 mr-2"></svg>
                  <span>{{ auth.user()?.name }}</span>
                </button>
                <button
                  uiButton
                  variant="ghost"
                  size="sm"
                  (click)="auth.logout(); showMobileMenu.set(false)"
                  class="w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent"
                >
                  <svg lucide="LogOut" class="w-4 h-4 mr-2"></svg>
                  Sign Out
                </button>
              } @else {
                <button
                  uiButton
                  variant="ghost"
                  size="sm"
                  (click)="showLogin.set(true); showMobileMenu.set(false)"
                  class="w-full justify-start text-sidebar-foreground hover:bg-sidebar-accent"
                >
                  <svg lucide="LogIn" class="w-4 h-4 mr-2"></svg>
                  Login
                </button>
              }
            </div>
          </div>
        }
      </header>

      <login-dialog [open]="showLogin()" (openChange)="showLogin.set($event)" />
      @if (showSettings()) {
        <settings-panel (closed)="showSettings.set(false)" />
      }
  `,
})
export class MapHeader {
  readonly searchToggle = output<void>();
  readonly filterToggle = output<void>();
  readonly inspectionPlannerToggle = output<void>();
  readonly locateUser = output<void>();
  readonly insightsToggle = output<void>();
  readonly pipelineToggle = output<void>();
  readonly followingToggle = output<void>();
  readonly isSearchOpen = input.required<boolean>();
  readonly isFilterOpen = input.required<boolean>();
  readonly isInspectionPlannerOpen = input.required<boolean>();
  readonly isInsightsOpen = input.required<boolean>();
  readonly isPipelineOpen = input.required<boolean>();
  readonly isFollowingOpen = input.required<boolean>();

  protected readonly auth = injectAuth();
  protected readonly showLogin = signal(false);
  protected readonly showMobileMenu = signal(false);
  protected readonly showSettings = signal(false);

  protected readonly canViewInsights = computed(() => this.auth.isAuthenticated() && !this.auth.isPolice() && !this.auth.isLocalGovernment());
  protected readonly showFollowingMenu = computed(() => this.auth.isAuthenticated() && (this.auth.isPolice() || this.auth.isLocalGovernment()));
}
