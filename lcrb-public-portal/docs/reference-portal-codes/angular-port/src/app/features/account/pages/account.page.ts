/** Role-aware Account Settings; each tab body sits in `@if (activeTab() === ...)` so switching tabs resets its state, as Radix does in the prototype. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { CommentingSettings } from '@shared/ui/settings/commenting-settings';
import { AutoApprovalSettings } from '@shared/ui/settings/auto-approval-settings';
import { injectSession } from '@core/session/session-provider';
import { ALERT_DIALOG } from '@shared/ui/primitives/alert-dialog';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { type IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { Switch } from '@shared/ui/primitives/switch';
import { TABLE } from '@shared/ui/primitives/table';
import { TABS } from '@shared/ui/primitives/tabs';
import { type AccountConfig, type DelegatesConfig, type OrgIcon, type OrgTabConfig } from '@domain/models/account';
import type { UserRole } from '@domain/roles';
import { ACCOUNT_REPOSITORY } from '@data-access/repositories/account/account.repository';

// Prototype fixtures for fields the mock session doesn't carry; name, email and organization come from the session.
const mockAccountData = {
  user: {
    firstName: 'Matt',
    lastName: 'Harding',
    email: 'matt.harding@hardinghospitality.ca',
    phone: '(250) 555-1234',
    lastLogin: 'May 28, 2026 at 10:30 AM',
  },
  delegates: [
    {
      id: '1',
      name: 'Jane Smith',
      email: 'jane.smith@example.com',
      role: 'Full Access',
      status: 'Active',
      addedOn: 'Jan 15, 2026',
    },
    {
      id: '2',
      name: 'Bob Johnson',
      email: 'bob.j@example.com',
      role: 'View Only',
      status: 'Pending',
      addedOn: 'May 20, 2026',
    },
  ],
};

/** Maps the config's icon key to a lucide icon. */
const ORG_ICONS: Record<OrgIcon, IconName> = {
  building: 'Building2',
  landmark: 'Landmark',
  shield: 'Shield',
  briefcase: 'Briefcase',
};

@Component({
  selector: 'account-profile-tab',
  imports: [Badge, Button, CARD, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <div uiCard>
        <div uiCardHeader>
          <div class="flex items-center justify-between">
            <div>
              <div uiCardTitle>Personal Information</div>
              <div uiCardDescription>{{ config().identity.profileDescription }}</div>
            </div>
            <button uiButton variant="outline" (click)="isEditing.set(!isEditing())" class="gap-2">
              <svg lucide="Edit" class="h-4 w-4"></svg>{{ isEditing() ? 'Cancel' : 'Edit' }}
            </button>
          </div>
        </div>
        <div uiCardContent class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="space-y-2">
              <label uiLabel>First Name</label>
              @if (isEditing()) {
                <input uiInput [value]="firstName()" />
              } @else {
                <p class="text-sm py-2">{{ firstName() }}</p>
              }
            </div>
            <div class="space-y-2">
              <label uiLabel>Last Name</label>
              @if (isEditing()) {
                <input uiInput [value]="lastName()" />
              } @else {
                <p class="text-sm py-2">{{ lastName() }}</p>
              }
            </div>
            <div class="space-y-2">
              <label uiLabel>Email Address</label>
              @if (isEditing()) {
                <input uiInput type="email" [value]="email()" />
              } @else {
                <p class="text-sm py-2 flex items-center gap-2">
                  <svg lucide="Mail" class="h-4 w-4 text-muted-foreground"></svg>{{ email() }}
                </p>
              }
            </div>
            <div class="space-y-2">
              <label uiLabel>Phone Number</label>
              @if (isEditing()) {
                <input uiInput type="tel" [value]="phone" />
              } @else {
                <p class="text-sm py-2 flex items-center gap-2">
                  <svg lucide="Phone" class="h-4 w-4 text-muted-foreground"></svg>{{ phone }}
                </p>
              }
            </div>
          </div>

          @if (isEditing()) {
            <div class="flex justify-end gap-3 pt-4 border-t">
              <button uiButton variant="outline" (click)="isEditing.set(false)">Cancel</button>
              <button uiButton class="bg-[#013366] hover:bg-[#38598A]">Save Changes</button>
            </div>
          }
        </div>
      </div>

      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>{{ config().identity.cardTitle }}</div>
          <div uiCardDescription>{{ config().identity.cardDescription }}</div>
        </div>
        <div uiCardContent>
          <div class="space-y-4">
            <div class="flex items-center justify-between py-3 border-b">
              <div>
                <p class="text-sm font-medium">{{ config().identity.usernameLabel }}</p>
                <p class="text-sm text-muted-foreground">{{ username() }}</p>
              </div>
              <span uiBadge variant="outline" class="bg-green-50 text-green-700 border-green-200">
                <svg lucide="CheckCircle" class="h-3 w-3 mr-1"></svg>Verified
              </span>
            </div>
            <div class="flex items-center justify-between py-3">
              <div>
                <p class="text-sm font-medium">Last Login</p>
                <p class="text-sm text-muted-foreground">{{ lastLogin }}</p>
              </div>
              <button uiButton variant="link" class="text-[#255A90] gap-1 p-0 h-auto">
                <svg lucide="ExternalLink" class="h-4 w-4"></svg>{{ config().identity.manageLabel }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Shown when delegation isn't available for the role (LCRB staff). -->
      @if (config().delegatesUnavailableNote) {
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle class="text-base">Delegate Access</div>
          </div>
          <div uiCardContent>
            <p class="text-sm text-muted-foreground leading-relaxed">{{ config().delegatesUnavailableNote }}</p>
          </div>
        </div>
      }
    </div>
  `,
})
export class AccountProfileTab {
  readonly config = input.required<AccountConfig>();
  private readonly session = injectSession().session;
  protected readonly isEditing = signal(false);
  protected readonly phone = mockAccountData.user.phone;
  protected readonly lastLogin = mockAccountData.user.lastLogin;

  // Prefer the signed-in identity, falling back to fixtures for fields the session doesn't carry.
  protected readonly firstName = computed(() => this.session()?.firstName ?? mockAccountData.user.firstName);
  protected readonly lastName = computed(() => this.session()?.lastName ?? mockAccountData.user.lastName);
  protected readonly email = computed(() => this.session()?.email ?? mockAccountData.user.email);

  // Derive the sign-in username from the session so it matches the name shown above it; fall back to the fixture.
  protected readonly username = computed(() => {
    const email = this.session()?.email;
    return email ? email.split('@')[0].replace(/[.+]/g, '_').toLowerCase() : this.config().identity.username;
  });
}

/** Organization tab, labelled Business, Jurisdiction, Agency or Branch by role. */
@Component({
  selector: 'account-organization-tab',
  imports: [Badge, Button, CARD, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <div uiCard>
        <div uiCardHeader>
          <div class="flex flex-wrap items-start justify-between gap-3">
            <div>
              <div uiCardTitle>{{ org().cardTitle }}</div>
              <div uiCardDescription>{{ org().cardDescription }}</div>
            </div>
            <span uiBadge variant="outline" class="bg-blue-50 text-blue-700 border-blue-200">{{ org().sourceBadge }}</span>
          </div>
        </div>
        <div uiCardContent>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            @for (field of org().fields; track field.label) {
              <div class="space-y-1">
                <label uiLabel class="text-muted-foreground text-xs">{{ field.label }}</label>
                <p [attr.class]="'text-sm ' + (field.isOrgName ? 'font-medium' : '')">
                  <!-- The signed-in organization wins over the fixture value. -->
                  {{ field.isOrgName ? (session()?.organization ?? field.value) : field.value }}
                </p>
              </div>
            }
          </div>

          <div class="mt-6 p-4 bg-muted/50 rounded-lg">
            <p class="text-sm text-muted-foreground leading-relaxed">{{ org().footnote }}</p>
            @if (org().externalLink; as link) {
              <a uiButton variant="link" class="text-[#255A90] gap-1 p-0 h-auto mt-2" [attr.href]="link.href" target="_blank" rel="noopener noreferrer">
                <svg lucide="ExternalLink" class="h-4 w-4"></svg>{{ link.label }}
              </a>
            }
          </div>
        </div>
      </div>

      @if (addresses().length > 0) {
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Addresses</div>
            <div uiCardDescription>
              {{ addresses().length > 1 ? 'Mailing and physical addresses on file' : 'Address on file' }}
            </div>
          </div>
          <div uiCardContent>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              @for (address of addresses(); track address.label) {
                <div class="space-y-2">
                  <label uiLabel class="flex items-center gap-2">
                    <svg lucide="MapPin" class="h-4 w-4 text-muted-foreground"></svg>{{ address.label }}
                  </label>
                  <p class="text-sm">{{ address.value }}</p>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
})
export class AccountOrganizationTab {
  readonly org = input.required<OrgTabConfig>();
  protected readonly session = injectSession().session;
  protected readonly addresses = computed(() => this.org().addresses ?? []);
}

@Component({
  selector: 'account-notifications-tab',
  imports: [Button, CARD, LucideIcon, Switch],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Notification Channels</div>
          <div uiCardDescription>Choose how you want to receive notifications</div>
        </div>
        <div uiCardContent class="space-y-6">
          <div class="flex items-center justify-between py-3 border-b">
            <div>
              <p class="font-medium">Email Notifications</p>
              <p class="text-sm text-muted-foreground">Receive notifications via email to {{ email() }}</p>
            </div>
            <button
              uiSwitch
              [checked]="channels().email"
              (checkedChange)="setChannel('email', $event)"
              aria-label="Email notifications"
            ></button>
          </div>
          <div class="flex items-center justify-between py-3">
            <div>
              <p class="font-medium">SMS Notifications</p>
              <p class="text-sm text-muted-foreground">Receive text messages to {{ phone }}</p>
            </div>
            <button
              uiSwitch
              [checked]="channels().sms"
              (checkedChange)="setChannel('sms', $event)"
              aria-label="SMS notifications"
            ></button>
          </div>
        </div>
      </div>

      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Notification Types</div>
          <div uiCardDescription>Select which notifications you want to receive</div>
        </div>
        <div uiCardContent class="space-y-0">
          @for (type of config().notificationTypes; track type.id; let index = $index) {
            <div
              [attr.class]="
                'flex items-center justify-between gap-4 py-4 ' +
                (index < config().notificationTypes.length - 1 ? 'border-b' : '')
              "
            >
              <div>
                <p class="font-medium">{{ type.label }}</p>
                <p class="text-sm text-muted-foreground leading-relaxed">{{ type.description }}</p>
              </div>
              <button
                uiSwitch
                [checked]="types()[type.id] ?? false"
                (checkedChange)="updateType(type.id, $event)"
                [attr.aria-label]="type.label"
              ></button>
            </div>
          }
        </div>
      </div>

      <div class="flex items-center justify-end gap-3">
        @if (saved()) {
          <p class="text-sm text-muted-foreground flex items-center gap-1.5">
            <svg lucide="CheckCircle" class="h-4 w-4 text-green-600"></svg>Preferences saved
          </p>
        }
        <button uiButton class="bg-[#013366] hover:bg-[#38598A]" (click)="saved.set(true)">Save Preferences</button>
      </div>
    </div>
  `,
})
export class AccountNotificationsTab {
  private readonly accountRepository = inject(ACCOUNT_REPOSITORY);
  readonly role = input.required<UserRole>();
  readonly config = input.required<AccountConfig>();
  private readonly session = injectSession().session;
  protected readonly channels = signal({ email: true, sms: false });
  // Notification types differ by role, so seed state from the role's config (once, like useState's initializer).
  private seeded: Record<string, boolean> | undefined;
  private readonly typesOverride = signal<Record<string, boolean> | null>(null);
  protected readonly types = computed(() => this.typesOverride() ?? (this.seeded ??= this.accountRepository.defaultNotificationState(this.role())));
  protected readonly saved = signal(false);
  protected readonly phone = mockAccountData.user.phone;
  protected readonly email = computed(() => this.session()?.email ?? mockAccountData.user.email);

  protected setChannel(channel: 'email' | 'sms', checked: boolean): void {
    this.channels.update((prev) => ({ ...prev, [channel]: checked }));
    this.saved.set(false);
  }

  protected updateType(id: string, value: boolean): void {
    this.typesOverride.set({ ...this.types(), [id]: value });
    this.saved.set(false);
  }
}

interface Delegate {
  id: string;
  name: string;
  email: string;
  role: string;
  status: 'Active' | 'Pending';
  addedOn: string;
}

const ACCESS_LEVELS = [
  { value: 'full-access', label: 'Full Access' },
  { value: 'view-only', label: 'View Only' },
  { value: 'submit-only', label: 'Submit Only' },
] as const;

const roleToValue = (label: string) => label.toLowerCase().replace(/ /g, '-');
const valueToRole = (value: string) => ACCESS_LEVELS.find((a) => a.value === value)?.label ?? 'View Only';

@Component({
  selector: 'account-delegates-tab',
  imports: [ALERT_DIALOG, Badge, Button, CARD, DIALOG, Input, Label, LucideIcon, SELECT, TABLE],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <div uiCard>
        <div uiCardHeader>
          <div class="flex items-center justify-between">
            <div>
              <div uiCardTitle>{{ copy().cardTitle }}</div>
              <div uiCardDescription>{{ copy().cardDescription }}</div>
            </div>
            <ui-dialog [state]="addOpen() ? 'open' : 'closed'" (stateChanged)="onDialogState($event)">
              <button uiButton class="bg-[#013366] hover:bg-[#38598A] gap-2" (click)="addOpen.set(true)">
                <svg lucide="Plus" class="h-4 w-4"></svg>{{ copy().addButtonLabel }}
              </button>
              <ui-dialog-content *uiDialogPortal>
                <div uiDialogHeader>
                  <h2 uiDialogTitle>{{ copy().addDialogTitle }}</h2>
                  <p uiDialogDescription>{{ copy().addDialogDescription }}</p>
                </div>
                <div class="space-y-4 py-2">
                  <div class="space-y-2">
                    <label uiLabel for="delegate-name">Full Name</label>
                    <input
                      uiInput
                      id="delegate-name"
                      [value]="form().name"
                      (input)="setField('name', $any($event.target).value)"
                      placeholder="e.g. Jane Smith"
                    />
                  </div>
                  <div class="space-y-2">
                    <label uiLabel for="delegate-email">Email Address</label>
                    <input
                      uiInput
                      id="delegate-email"
                      type="email"
                      [value]="form().email"
                      (input)="setField('email', $any($event.target).value)"
                      placeholder="name@example.com"
                    />
                  </div>
                  <div class="space-y-2">
                    <label uiLabel for="delegate-access">Access Level</label>
                    <ui-select [value]="form().accessLevel" (valueChange)="setField('accessLevel', $event ?? '')">
                      <button uiSelectTrigger id="delegate-access"><span uiSelectValue></span></button>
                      <ng-template uiSelectContent>
                        @for (a of accessLevels; track a.value) {
                          <ui-select-item [value]="a.value">{{ a.label }}</ui-select-item>
                        }
                      </ng-template>
                    </ui-select>
                  </div>
                  @if (formError()) {
                    <p class="text-sm text-destructive">{{ formError() }}</p>
                  }
                </div>
                <div uiDialogFooter>
                  <button uiButton variant="outline" (click)="addOpen.set(false)">Cancel</button>
                  <button uiButton class="bg-[#013366] hover:bg-[#38598A]" (click)="handleAdd()">{{ copy().addButtonLabel }}</button>
                </div>
              </ui-dialog-content>
            </ui-dialog>
          </div>
        </div>
        <div uiCardContent>
          <p class="text-sm text-muted-foreground leading-relaxed mb-4">{{ copy().note }}</p>
          <ui-table>
            <thead uiTableHeader>
              <tr uiTableRow>
                <th uiTableHead>Name</th>
                <th uiTableHead>Email</th>
                <th uiTableHead>Access Level</th>
                <th uiTableHead>Status</th>
                <th uiTableHead>Added</th>
                <th uiTableHead class="text-right">Actions</th>
              </tr>
            </thead>
            <tbody uiTableBody>
              @for (delegate of rows(); track delegate.id) {
                <tr uiTableRow>
                  <td uiTableCell class="font-medium">{{ delegate.name }}</td>
                  <td uiTableCell>{{ delegate.email }}</td>
                  <td uiTableCell>
                    <ui-select [value]="roleToValue(delegate.role)" (valueChange)="handleRoleChange(delegate.id, $event ?? '')">
                      <button uiSelectTrigger class="w-[130px]"><span uiSelectValue></span></button>
                      <ng-template uiSelectContent>
                        @for (a of accessLevels; track a.value) {
                          <ui-select-item [value]="a.value">{{ a.label }}</ui-select-item>
                        }
                      </ng-template>
                    </ui-select>
                  </td>
                  <td uiTableCell>
                    @if (delegate.status === 'Active') {
                      <span uiBadge variant="outline" class="bg-green-50 text-green-700 border-green-200">
                        <svg lucide="CheckCircle" class="h-3 w-3 mr-1"></svg>Active
                      </span>
                    } @else {
                      <span uiBadge variant="outline" class="bg-yellow-50 text-yellow-700 border-yellow-200">Pending</span>
                    }
                  </td>
                  <td uiTableCell class="text-muted-foreground">{{ delegate.addedOn }}</td>
                  <td uiTableCell class="text-right">
                    <ui-alert-dialog>
                      <button uiButton uiAlertDialogTrigger variant="ghost" size="icon" class="text-destructive hover:text-destructive">
                        <svg lucide="Trash2" class="h-4 w-4"></svg>
                      </button>
                      <ui-alert-dialog-content *uiAlertDialogPortal>
                        <div uiAlertDialogHeader>
                          <h2 uiAlertDialogTitle>Remove Delegate?</h2>
                          <p uiAlertDialogDescription>
                            This will revoke {{ delegate.name }}'s access to your account. They will no longer be able to view or manage your licences and applications.
                          </p>
                        </div>
                        <div uiAlertDialogFooter>
                          <button uiAlertDialogCancel>Cancel</button>
                          <button uiAlertDialogAction class="bg-destructive hover:bg-destructive/90" (click)="handleRemove(delegate.id)">Remove</button>
                        </div>
                      </ui-alert-dialog-content>
                    </ui-alert-dialog>
                  </td>
                </tr>
              }
              @if (rows().length === 0) {
                <tr uiTableRow>
                  <td uiTableCell colspan="6" class="py-8 text-center text-sm text-muted-foreground">
                    No delegates yet. Use &ldquo;Add Delegate&rdquo; to invite someone to your account.
                  </td>
                </tr>
              }
            </tbody>
          </ui-table>
        </div>
      </div>

      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Access Level Descriptions</div>
        </div>
        <div uiCardContent>
          <div class="space-y-4">
            <div class="flex gap-4">
              <span uiBadge variant="outline" class="shrink-0">Full Access</span>
              <p class="text-sm text-muted-foreground">
                Can view, edit, and submit applications and licence changes. Cannot add or remove other delegates.
              </p>
            </div>
            <div class="flex gap-4">
              <span uiBadge variant="outline" class="shrink-0">View Only</span>
              <p class="text-sm text-muted-foreground">
                Can view licences and applications but cannot make changes or submissions.
              </p>
            </div>
            <div class="flex gap-4">
              <span uiBadge variant="outline" class="shrink-0">Submit Only</span>
              <p class="text-sm text-muted-foreground">
                Can view and submit applications but cannot make changes to existing licences.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class AccountDelegatesTab {
  readonly delegates = input.required<DelegatesConfig>();
  protected readonly copy = computed(() => this.delegates());
  protected readonly accessLevels = ACCESS_LEVELS;
  protected readonly roleToValue = roleToValue;

  protected readonly rows = signal<Delegate[]>([...(mockAccountData.delegates as Delegate[])]);
  protected readonly addOpen = signal(false);
  protected readonly form = signal({ name: '', email: '', accessLevel: 'view-only' });
  protected readonly formError = signal<string | null>(null);

  private resetForm(): void {
    this.form.set({ name: '', email: '', accessLevel: 'view-only' });
    this.formError.set(null);
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button): reset the form then. */
  protected onDialogState(state: string): void {
    if (state === 'closed' && this.addOpen()) {
      this.addOpen.set(false);
      this.resetForm();
    }
  }

  protected setField(field: 'name' | 'email' | 'accessLevel', value: string): void {
    this.form.update((f) => ({ ...f, [field]: value }));
  }

  protected handleAdd(): void {
    const form = this.form();
    const name = form.name.trim();
    const email = form.email.trim();
    if (!name || !email) {
      this.formError.set('Please enter both a name and an email address.');
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.formError.set('Please enter a valid email address.');
      return;
    }
    const newDelegate: Delegate = {
      id: `delegate-${Date.now()}`,
      name,
      email,
      role: valueToRole(form.accessLevel),
      status: 'Pending',
      addedOn: new Date().toLocaleDateString('en-CA', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      }),
    };
    this.rows.update((prev) => [...prev, newDelegate]);
    this.addOpen.set(false);
    this.resetForm();
  }

  protected handleRoleChange(id: string, value: string): void {
    this.rows.update((prev) => prev.map((d) => (d.id === id ? { ...d, role: valueToRole(value) } : d)));
  }

  protected handleRemove(id: string): void {
    this.rows.update((prev) => prev.filter((d) => d.id !== id));
  }
}

@Component({
  selector: 'account-page',
  imports: [
    AccountDelegatesTab,
    AccountNotificationsTab,
    AccountOrganizationTab,
    AccountProfileTab,
    AutoApprovalSettings,
    BCGovFooter,
    BCGovHeader,
    CommentingSettings,
    LucideIcon,
    PageHero,
    TABS,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/account" />

      <main class="flex-1">
        <page-hero
          containerClassName="mx-auto max-w-5xl px-4 sm:px-6 lg:px-8"
          [breadcrumbs]="breadcrumbs()"
          title="Account Settings"
          [description]="config().description"
        />

        <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Tabs -->
          <div uiTabs [(value)]="activeTab" class="space-y-6">
            <div
              uiTabsList
              class="grid w-full grid-cols-2 sm:grid-cols-3 lg:w-auto lg:inline-flex"
              aria-label="Account settings sections"
            >
              @for (tab of tabs(); track tab.value) {
                <button uiTabsTrigger [value]="tab.value" class="gap-2">
                  <svg [lucide]="tab.icon" class="h-4 w-4 hidden sm:block"></svg>{{ tab.label }}
                </button>
              }
            </div>

            <!-- Profile Tab -->
            <div uiTabsContent value="profile">
              @if (activeTab() === 'profile') {
                <account-profile-tab [config]="config()" />
              }
            </div>

            <!-- Organization Tab: Business / Jurisdiction / Agency / Branch -->
            @if (config().org; as org) {
              <div uiTabsContent value="organization">
                @if (activeTab() === 'organization') {
                  <account-organization-tab [org]="org" />
                }
              </div>
            }

            <!-- Notifications Tab -->
            <div uiTabsContent value="notifications">
              @if (activeTab() === 'notifications') {
                <account-notifications-tab [role]="role()" [config]="config()" />
              }
            </div>

            <!-- Delegates Tab: not offered to centrally provisioned LCRB staff -->
            @if (config().delegates; as delegates) {
              <div uiTabsContent value="delegates">
                @if (activeTab() === 'delegates') {
                  <account-delegates-tab [delegates]="delegates" />
                }
              </div>
            }

            <!-- Commenting Tab: Local Government / Indigenous Nation only -->
            @if (config().showCommenting) {
              <div uiTabsContent value="commenting">
                @if (activeTab() === 'commenting') {
                  <commenting-settings />
                }
              </div>
            }

            <!-- Auto-approval Tab: Police agencies only -->
            @if (config().showSepAutoApproval) {
              <div uiTabsContent value="auto-approval">
                @if (activeTab() === 'auto-approval') {
                  <auto-approval-settings />
                }
              </div>
            }
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class AccountPage {
  private readonly accountRepository = inject(ACCOUNT_REPOSITORY);
  private readonly session = injectSession().session;
  // Every partner type gets this panel; tabs and labels come from the role's config.
  protected readonly role = computed<UserRole>(() => this.session()?.role ?? 'licensee');
  protected readonly config = computed(() => this.accountRepository.accountConfig(this.role()));
  protected readonly activeTab = signal<string | undefined>('profile');

  protected readonly breadcrumbs = computed(() => [
    { label: this.config().home.label, href: this.config().home.href },
    { label: 'Account Settings' },
  ]);

  // Build the tab list from the config so the grid sizes itself correctly.
  protected readonly tabs = computed<{ value: string; label: string; icon: IconName }[]>(() => {
    const config = this.config();
    return [
      { value: 'profile', label: 'Profile', icon: 'User' },
      ...(config.org ? [{ value: 'organization', label: config.org.label, icon: ORG_ICONS[config.org.icon] }] : []),
      { value: 'notifications', label: 'Notifications', icon: 'Bell' as IconName },
      ...(config.delegates ? [{ value: 'delegates', label: config.delegates.label, icon: 'Users' as IconName }] : []),
      ...(config.showCommenting ? [{ value: 'commenting', label: 'Commenting', icon: 'MessageSquare' as IconName }] : []),
      ...(config.showSepAutoApproval ? [{ value: 'auto-approval', label: 'Auto-approval', icon: 'Shield' as IconName }] : []),
    ];
  });
}
