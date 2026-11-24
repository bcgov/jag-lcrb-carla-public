/** Licensing map for staff; client-only, so it loads in its own lazy chunk behind a "Loading map…" fallback. AuthProvider seeds the staff role from the LCRB session. */
import { ChangeDetectionStrategy, Component, computed } from '@angular/core';
import { BCGovHeader } from '@core/layout/header';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { AuthProvider } from '@shared/ui/map/map-auth';
import { MapExperience } from '@shared/ui/map/map-experience';
import { injectSession } from '@core/session/session-provider';

@Component({
  selector: 'lcrb-map-page',
  imports: [BCGovHeader, LcrbGate, MapExperience],
  providers: [AuthProvider],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/lcrb/map" />
      <main id="main-content" class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            @defer (on immediate) {
              <map-experience />
            } @loading {
              <div class="flex h-[calc(100vh-3.5rem)] w-full items-center justify-center bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
            } @placeholder {
              <div class="flex h-[calc(100vh-3.5rem)] w-full items-center justify-center bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
            }
          }
        </lcrb-gate>
      </main>
    </div>
  `,
})
export class LcrbMapPage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
