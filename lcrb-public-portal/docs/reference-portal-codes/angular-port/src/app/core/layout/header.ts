/** Header with BC Gov top bar, role-specific navigation and mobile menu. */
import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map } from 'rxjs';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { DROPDOWN_MENU } from '@shared/ui/primitives/dropdown-menu';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SHEET } from '@shared/ui/primitives/sheet';
import { NotificationBell } from './notification-bell';
import { GlobalSearch } from './global-search';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';
import { ROLES, type UserRole } from '@domain/roles';
import { AccessibilityBar } from './accessibility-bar';
import { path } from '../../route-registry';
import { BCLogo } from '@shared/ui/bc-gov/bc-logo';

interface NavLink {
  name: string;
  href: string;
}

interface NavGroup {
  name: string;
  /** Child links shown in a dropdown (desktop) / expandable section (mobile). */
  children: NavLink[];
}

type NavItem = NavLink | NavGroup;

function isNavGroup(item: NavItem): item is NavGroup {
  return 'children' in item;
}

// Navigation is role-specific; "Announcements" is appended to every role.
const NAV_BY_ROLE: Record<UserRole, NavItem[]> = {
  licensee: [
    { name: 'Dashboard', href: path('dashboard.home') },
    { name: 'My Applications', href: path('applications.list') },
    { name: 'Licences & Authorizations', href: path('licences.list') },
    {
      name: 'Business Structure',
      children: [
        { name: 'Legal Entity Builder', href: path('legal-entity.builder') },
        { name: 'Tied House', href: path('legal-entity.tied-house') },
      ],
    },
    {
      name: 'Compliance',
      children: [
        { name: 'Compliance & Enforcement', href: path('licences.compliance') },
        { name: 'Renewals & Calendar', href: path('dashboard.calendar') },
        { name: 'Security Screening', href: path('legal-entity.security-screening') },
        { name: 'Notices', href: path('dashboard.notices') },
        { name: 'Receipts', href: path('account.receipts') },
      ],
    },
    { name: 'Special Event Permits', href: path('special-events.list') },
    { name: 'Map', href: path('public.map') },
  ],
  applicant: [
    { name: 'My Special Event Permits', href: path('special-events.list') },
    { name: 'Apply for a Permit', href: path('special-events.apply') },
  ],
  lcrb: [
    { name: 'Intake Overview', href: path('staff.home') },
    { name: 'Case Triage', href: path('staff.triage') },
    { name: 'Map', href: path('staff.map') },
    {
      name: 'Analytics',
      children: [
        { name: 'Intake Analytics', href: path('staff.analytics') },
        { name: 'Site Analytics', href: path('staff.site-analytics') },
      ],
    },
  ],
  police: [
    { name: 'Dashboard', href: path('police.home') },
    { name: 'Upcoming Events', href: path('police.events') },
    { name: 'Review Queue', href: path('police.queue') },
    { name: 'Activity', href: path('dashboard.activity') },
  ],
  'local-government': [
    { name: 'Dashboard', href: path('lg.home') },
    { name: 'Review Queue', href: path('lg.queue') },
    { name: 'Regional Map', href: path('lg.map') },
    { name: 'Activity', href: path('dashboard.activity') },
  ],
};

const ANNOUNCEMENTS_NAV: NavLink = { name: 'Announcements', href: path('dashboard.announcements') };

@Component({
  selector: 'bc-gov-header',
  imports: [RouterLink, NgTemplateOutlet, AccessibilityBar, BCLogo, Button, LucideIcon, ...DROPDOWN_MENU, ...SHEET, NotificationBell, GlobalSearch],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <header class="w-full">
      <bc-accessibility-bar />

      <!-- Top bar with BC Gov branding (white surface) -->
      <div class="bg-white text-[#2d2d2d] border-b border-[#dbdcdc]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex items-center justify-between h-16 md:h-20">
            <a
              [routerLink]="roleConfig().home"
              class="flex items-center gap-3 md:gap-4 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90] rounded"
            >
              <bc-logo />
              <span class="text-base md:text-lg lg:text-xl font-semibold tracking-tight text-[#013366] border-l border-[#dbdcdc] pl-3 md:pl-4">
                Liquor and Cannabis Licensing
              </span>
            </a>

            <!-- Desktop User Menu -->
            <div class="hidden md:flex items-center gap-3">
              <global-search />
              <span class="rounded-full bg-[#013366] text-white px-3 py-1 text-xs font-medium uppercase tracking-wide">
                {{ roleConfig().shortLabel }}
              </span>
              <notification-bell />
              <button uiButton variant="ghost" class="text-[#2d2d2d] hover:bg-[#f2f2f2] gap-2 h-auto py-2" [uiDropdownMenuTrigger]="userMenu" align="end">
                <div class="w-8 h-8 rounded bg-[#013366] text-white flex items-center justify-center text-sm font-medium">{{ initials() }}</div>
                <div class="text-left">
                  <div class="text-sm font-medium">{{ displayName() }}</div>
                  <div class="text-xs text-[#606060]">{{ displayOrg() }}</div>
                </div>
                <svg lucide="ChevronDown" class="h-4 w-4"></svg>
              </button>
              <ng-template #userMenu>
                <!-- Every partner type has an Account Settings panel; its tabs adapt to the role. -->
                <div uiDropdownMenuContent class="w-56">
                  <a uiDropdownMenuItem [routerLink]="path('account.settings')">Account Settings</a>
                  <div uiDropdownMenuSeparator></div>
                  <button uiDropdownMenuItem (triggered)="handleSignOut()" class="text-destructive">Sign Out</button>
                </div>
              </ng-template>
            </div>

            <!-- Mobile menu button -->
            <ui-sheet [state]="mobileMenuOpen() ? 'open' : 'closed'" (stateChanged)="mobileMenuOpen.set($event === 'open')">
              <button uiButton uiSheetTrigger variant="ghost" size="icon" class="md:hidden text-[#013366] hover:bg-[#f2f2f2]">
                <svg lucide="Menu" class="h-6 w-6"></svg>
                <span class="sr-only">Open menu</span>
              </button>
              <ui-sheet-content *uiSheetPortal side="right" class="w-80 bg-white text-[#2d2d2d] border-l border-[#dbdcdc]">
                <div class="flex flex-col h-full">
                  <div class="flex items-center justify-between mb-8">
                    <span class="text-lg font-semibold text-[#013366]">Menu</span>
                  </div>

                  <div class="p-4 bg-[#f2f2f2] border border-[#dbdcdc] rounded-lg mb-6">
                    <div class="flex items-center gap-3">
                      <div class="w-10 h-10 rounded bg-[#013366] text-white flex items-center justify-center font-bold">{{ initials() }}</div>
                      <div>
                        <div class="font-medium text-[#2d2d2d]">{{ displayName() }}</div>
                        <div class="text-sm text-[#606060]">{{ roleConfig().shortLabel }}</div>
                      </div>
                    </div>
                  </div>

                  <nav class="flex flex-col gap-1">
                    @for (item of navigation(); track item.name) {
                      @if (asGroup(item); as group) {
                        <div class="mt-2">
                          <div class="px-4 py-2 text-xs font-semibold uppercase tracking-wide text-[#606060]">{{ group.name }}</div>
                          @for (child of group.children; track child.name) {
                            <a
                              [routerLink]="child.href"
                              [class]="'flex items-center px-4 py-3 pl-6 rounded-lg transition-colors ' + (isActive(child.href) ? 'bg-[#FCBA19] text-[#013366] font-medium' : 'text-[#2d2d2d] hover:bg-[#f2f2f2]')"
                              (click)="mobileMenuOpen.set(false)"
                              >{{ child.name }}</a
                            >
                          }
                        </div>
                      } @else {
                        <a
                          [routerLink]="asLink(item).href"
                          [class]="'flex items-center justify-between px-4 py-3 rounded-lg transition-colors ' + (isActive(asLink(item).href) ? 'bg-[#FCBA19] text-[#013366] font-medium' : 'text-[#2d2d2d] hover:bg-[#f2f2f2]')"
                          (click)="mobileMenuOpen.set(false)"
                        >
                          {{ item.name }}
                          <ng-container [ngTemplateOutlet]="badge" [ngTemplateOutletContext]="{ $implicit: asLink(item) }" />
                        </a>
                      }
                    }
                  </nav>

                  <div class="mt-auto pt-6 border-t border-[#dbdcdc]">
                    <a [routerLink]="path('account.settings')" class="block px-4 py-3 text-[#2d2d2d] hover:bg-[#f2f2f2] rounded-lg" (click)="mobileMenuOpen.set(false)">Account Settings</a>
                    <button type="button" (click)="handleSignOut()" class="block w-full text-left px-4 py-3 text-[#a12622] hover:bg-[#f2f2f2] rounded-lg">Sign Out</button>
                  </div>
                </div>
              </ui-sheet-content>
            </ui-sheet>
          </div>
        </div>
      </div>

      <!-- Navigation bar (white surface with gold accent) -->
      <nav class="hidden md:block bg-white text-[#2d2d2d] border-b-2 border-[#fcba19]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex items-center gap-1 h-12">
            @for (item of navigation(); track item.name) {
              @if (asGroup(item); as group) {
                <button
                  type="button"
                  [attr.aria-current]="groupActive(group) ? 'page' : null"
                  [class]="'flex items-center gap-1 px-4 py-2 rounded text-sm font-medium transition-colors ' + (groupActive(group) ? 'bg-[#013366] text-white' : 'text-[#013366] hover:bg-[#f2f2f2]')"
                  [uiDropdownMenuTrigger]="groupMenu"
                  align="start"
                >
                  {{ group.name }}
                  <svg lucide="ChevronDown" class="h-4 w-4"></svg>
                </button>
                <ng-template #groupMenu>
                  <div uiDropdownMenuContent class="w-56">
                    @for (child of group.children; track child.name) {
                      <a uiDropdownMenuItem [routerLink]="child.href">{{ child.name }}</a>
                    }
                  </div>
                </ng-template>
              } @else {
                <a
                  [routerLink]="asLink(item).href"
                  [attr.aria-current]="isActive(asLink(item).href) ? 'page' : null"
                  [class]="'relative flex items-center gap-2 px-4 py-2 rounded text-sm font-medium transition-colors ' + (isActive(asLink(item).href) ? 'bg-[#013366] text-white' : 'text-[#013366] hover:bg-[#f2f2f2]')"
                >
                  {{ item.name }}
                  <ng-container [ngTemplateOutlet]="badge" [ngTemplateOutletContext]="{ $implicit: asLink(item) }" />
                </a>
              }
            }
          </div>
        </div>
      </nav>
    </header>

    <ng-template #badge let-link>
      @if (link.href === announcementsHref && unreadAnnouncements() > 0) {
        <span class="flex h-5 min-w-5 items-center justify-center rounded-full bg-[#D8292F] px-1.5 text-xs font-semibold text-white">
          {{ unreadAnnouncements() }}
          <span class="sr-only"> new announcements</span>
        </span>
      }
    </ng-template>
  `,
})
export class BCGovHeader {
  protected readonly path = path;
  /** Override the role used for navigation. Defaults to the session role. */
  readonly role = input<UserRole | undefined>(undefined);
  readonly currentPage = input<string | undefined>(undefined);

  private readonly sessionProvider = injectSession();
  private readonly router = inject(Router);
  private readonly announcements = inject(NOTIFICATIONS_REPOSITORY).unreadAnnouncements();
  protected readonly mobileMenuOpen = signal(false);
  protected readonly announcementsHref = ANNOUNCEMENTS_NAV.href;

  private readonly pathname = toSignal(
    this.router.events.pipe(
      filter((e) => e instanceof NavigationEnd),
      map(() => this.router.url.split(/[?#]/)[0]),
    ),
    { initialValue: this.router.url.split(/[?#]/)[0] },
  );

  // Prefer an explicit currentPage prop, else the live pathname, so the active state works on pages that don't pass it.
  private readonly activePath = computed(() => this.currentPage() ?? this.pathname() ?? '');
  private readonly activeRole = computed<UserRole>(() => this.role() ?? this.sessionProvider.session()?.role ?? 'licensee');
  protected readonly navigation = computed(() => [...NAV_BY_ROLE[this.activeRole()], ANNOUNCEMENTS_NAV]);
  protected readonly roleConfig = computed(() => ROLES[this.activeRole()]);
  protected readonly unreadAnnouncements = computed(() => this.announcements().count);

  // Best-matching nav href: equal to the path or a parent segment of it; the longest match wins.
  private readonly bestMatchHref = computed(() => {
    const activePath = this.activePath();
    const allHrefs = this.navigation().flatMap((item) => (isNavGroup(item) ? item.children.map((c) => c.href) : [item.href]));
    return allHrefs.filter((href) => activePath === href || activePath.startsWith(`${href}/`)).sort((a, b) => b.length - a.length)[0];
  });

  protected readonly displayName = computed(() => {
    const session = this.sessionProvider.session();
    return session ? `${session.firstName} ${session.lastName}` : 'Guest';
  });
  protected readonly displayOrg = computed(() => this.sessionProvider.session()?.organization ?? '');
  protected readonly initials = computed(() => {
    const session = this.sessionProvider.session();
    return session ? `${session.firstName.charAt(0)}${session.lastName.charAt(0)}` : 'GU';
  });

  protected isActive(href: string): boolean {
    return href === this.bestMatchHref();
  }
  protected groupActive(group: NavGroup): boolean {
    return group.children.some((c) => this.isActive(c.href));
  }
  protected asGroup(item: NavItem): NavGroup | null {
    return isNavGroup(item) ? item : null;
  }
  protected asLink(item: NavItem): NavLink {
    return item as NavLink;
  }

  protected handleSignOut(): void {
    this.sessionProvider.logout();
    void this.router.navigateByUrl(path('account.login'));
  }
}
