/** Gate page; the form posts natively to `POST /gate` (server/gate.ts) so the server redirect works as before. `error` and `next` come from the query string. */
import { afterNextRender, ChangeDetectionStrategy, Component, type ElementRef, inject, input, OnDestroy, viewChild } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { AccessibilityBar } from '@core/layout/accessibility-bar';
import { BCLogo } from '@shared/ui/bc-gov/bc-logo';
import { Button } from '@shared/ui/primitives/button';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { path } from '@/app/route-registry';

@Component({
  selector: 'gate-page',
  imports: [AccessibilityBar, BCLogo, Button, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-[#F2F2F2]">
      <bc-accessibility-bar />
      <div class="bg-white text-[#2d2d2d]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex items-center gap-3 md:gap-4 h-16 md:h-20">
            <bc-logo />
            <span class="text-base md:text-lg font-semibold text-[#013366] border-l border-[#dbdcdc] pl-3 md:pl-4">
              Liquor and Cannabis Licensing
            </span>
          </div>
        </div>
      </div>
      <div class="bg-[#FCBA19] h-1"></div>

      <main id="main-content" class="flex-1">
        <div class="max-w-md mx-auto px-4 sm:px-6 py-12 md:py-20">
          <div class="bg-white rounded-lg border border-[#D9D9D9] p-6 md:p-8">
            <div class="w-12 h-12 rounded-lg bg-[#013366] text-white flex items-center justify-center">
              <svg lucide="Lock" class="h-6 w-6" aria-hidden="true"></svg>
            </div>
            <h1 class="mt-4 text-2xl font-bold text-[#013366] text-balance">Restricted access</h1>
            <p class="mt-2 text-[#474543] leading-relaxed text-pretty">
              This is a private preview for invited testers. Enter the access code to continue.
            </p>

            @if (error()) {
              <div
                role="alert"
                class="mt-5 flex items-start gap-2 rounded-md bg-[#F4E1E2] border-l-4 border-[#CE3E39] p-3 text-sm text-[#2D2D2D]"
              >
                <svg lucide="AlertCircle" class="h-4 w-4 mt-0.5 text-[#CE3E39] shrink-0" aria-hidden="true"></svg>
                <span>That access code is not correct. Please check with your project contact and try again.</span>
              </div>
            }

            <form action="/gate" method="post" class="mt-6 flex flex-col gap-4">
              <input type="hidden" name="next" [value]="next() ?? path('account.login')" />
              <div class="flex flex-col gap-1.5">
                <label uiLabel for="code">Access code</label>
                <input
                  uiInput
                  #code
                  id="code"
                  name="code"
                  type="password"
                  autocomplete="off"
                  required
                  placeholder="Enter access code"
                />
              </div>
              <button uiButton type="submit" class="w-full bg-[#013366] hover:bg-[#38598A] text-white mt-1">
                <svg lucide="Lock" class="h-4 w-4" aria-hidden="true"></svg>
                Unlock
              </button>
            </form>
          </div>

          <p class="mt-6 text-center text-sm text-[#474543]">
            Need access? Contact the person who invited you to this preview.
          </p>
        </div>
      </main>

      <footer class="bg-white text-[#474543] text-sm border-t-2 border-[#fcba19]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 text-center sm:text-left">
          A demonstration prototype for the Province of British Columbia
        </div>
      </footer>
    </div>
  `,
})
export class GatePage implements OnDestroy {
  protected readonly path = path;
  readonly error = input<string | undefined>();
  readonly next = input<string | undefined>();

  private readonly meta = inject(Meta);
  private readonly code = viewChild.required<ElementRef<HTMLInputElement>>('code');

  constructor() {
    // metadata.robots in the prototype.
    this.meta.updateTag({ name: 'robots', content: 'noindex, nofollow' });
    // React's autoFocus focuses on mount without writing the attribute.
    afterNextRender(() => this.code().nativeElement.focus());
  }

  ngOnDestroy(): void {
    this.meta.removeTag('name="robots"');
  }
}
