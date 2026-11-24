/** Demo sign-in form, or the account card when signed in. */
import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { injectAuth } from './map-auth';
import { ALERT } from '../primitives/alert';
import { Button } from '../primitives/button';
import { DIALOG } from '../primitives/dialog';
import { Input } from '../primitives/input';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';

@Component({
  selector: 'login-dialog',
  imports: [...DIALOG, ...ALERT, Button, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <ui-dialog-content *uiDialogPortal class="sm:max-w-md">
        @if (auth.isAuthenticated() && auth.user(); as user) {
          <div uiDialogHeader>
            <div class="flex items-center gap-3 mb-2">
              <div class="p-2 bg-primary/10 rounded-lg">
                <svg lucide="User" class="w-6 h-6 text-primary"></svg>
              </div>
              <div>
                <h2 uiDialogTitle>Account</h2>
                <p uiDialogDescription>Manage your session</p>
              </div>
            </div>
          </div>

          <div class="space-y-4">
            <div class="p-4 bg-muted rounded-lg space-y-3">
              <div class="flex items-center gap-3">
                <div class="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center">
                  <svg lucide="User" class="w-6 h-6 text-primary"></svg>
                </div>
                <div>
                  <p class="font-semibold text-lg">{{ user.name }}</p>
                  <div class="flex items-center gap-1">
                    <svg lucide="BadgeCheck" [class]="'w-4 h-4 ' + roleColor()"></svg>
                    <span [class]="'text-sm font-medium ' + roleColor()">{{ userRole() }}</span>
                  </div>
                </div>
              </div>

              <div class="flex items-center gap-2 text-sm text-muted-foreground">
                <svg lucide="Mail" class="w-4 h-4"></svg>
                <span>{{ user.email }}</span>
              </div>
            </div>

            <button uiButton variant="destructive" class="w-full" (click)="handleLogout()">
              <svg lucide="LogOut" class="w-4 h-4 mr-2"></svg>
              Sign Out
            </button>
          </div>
        } @else {
          <div uiDialogHeader>
            <div class="flex items-center gap-3 mb-2">
              <div class="p-2 bg-primary/10 rounded-lg">
                <svg lucide="Shield" class="w-6 h-6 text-primary"></svg>
              </div>
              <div>
                <h2 uiDialogTitle>LCRB Login</h2>
                <p uiDialogDescription>Access staff and admin features</p>
              </div>
            </div>
          </div>

          <form (submit)="handleSubmit($event)" class="space-y-4">
            @if (error()) {
              <div uiAlert variant="destructive">
                <div uiAlertDescription>{{ error() }}</div>
              </div>
            }

            <div class="space-y-2">
              <label uiLabel for="email">Email</label>
              <input
                uiInput
                id="email"
                type="email"
                placeholder="staff@gov.bc.ca"
                [value]="email()"
                (input)="email.set($any($event.target).value)"
                required
              />
            </div>

            <div class="space-y-2">
              <label uiLabel for="password">Password</label>
              <input uiInput id="password" type="password" [value]="password()" (input)="password.set($any($event.target).value)" required />
            </div>

            <button uiButton type="submit" class="w-full" [disabled]="loading()">
              @if (loading()) {
                <svg lucide="Loader2" class="w-4 h-4 mr-2 animate-spin"></svg>
              }
              Sign In
            </button>

            <div class="text-xs text-muted-foreground p-3 bg-muted rounded-lg space-y-2">
              <p class="font-medium">Demo Credentials (Password: lcrb2024)</p>
              <div class="grid grid-cols-2 gap-2">
                <div>
                  <p class="font-medium text-primary">Admin:</p>
                  <p>admin&#64;gov.bc.ca</p>
                </div>
                <div>
                  <p class="font-medium text-blue-600">Staff:</p>
                  <p>staff&#64;gov.bc.ca</p>
                </div>
                <div>
                  <p class="font-medium text-amber-600">Police:</p>
                  <p>police&#64;gov.bc.ca</p>
                </div>
                <div>
                  <p class="font-medium text-green-600">Local Gov:</p>
                  <p>lg&#64;gov.bc.ca</p>
                </div>
              </div>
            </div>
          </form>
        }
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class LoginDialog {
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();

  protected readonly auth = injectAuth();
  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly error = signal('');
  protected readonly loading = signal(false);

  protected readonly userRole = computed(() => {
    if (this.auth.isAdmin()) return 'Administrator';
    if (this.auth.isStaff()) return 'Staff';
    if (this.auth.isPolice()) return 'Police';
    if (this.auth.isLocalGovernment()) return 'Local Government';
    return 'User';
  });

  protected readonly roleColor = computed(() => {
    if (this.auth.isAdmin()) return 'text-primary';
    if (this.auth.isStaff()) return 'text-blue-600';
    if (this.auth.isPolice()) return 'text-amber-600';
    if (this.auth.isLocalGovernment()) return 'text-green-600';
    return 'text-muted-foreground';
  });

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }

  protected async handleSubmit(e: Event): Promise<void> {
    e.preventDefault();
    this.error.set('');
    this.loading.set(true);

    const success = await this.auth.login(this.email(), this.password());

    if (success) {
      this.openChange.emit(false);
      this.email.set('');
      this.password.set('');
    } else {
      this.error.set('Invalid credentials. Please try again.');
    }

    this.loading.set(false);
  }

  protected handleLogout(): void {
    this.auth.logout();
    this.openChange.emit(false);
  }
}
