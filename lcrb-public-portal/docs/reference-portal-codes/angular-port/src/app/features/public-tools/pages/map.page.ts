/** Public Licensing & Events Map page. Client-only, so it loads lazily behind "Loading map…" (ssr:false becomes @defer). `AuthProvider` is provided here. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovHeader } from '@core/layout/header';
import { AuthProvider } from '@shared/ui/map/map-auth';
import { MapExperience } from '@shared/ui/map/map-experience';

@Component({
  selector: 'map-page',
  imports: [BCGovHeader, MapExperience],
  providers: [AuthProvider],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/map" />
      <main id="main-content" class="flex-1">
        @defer (on immediate) {
          <map-experience />
        } @loading {
          <div class="flex h-[calc(100vh-3.5rem)] w-full items-center justify-center bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
        } @placeholder {
          <div class="flex h-[calc(100vh-3.5rem)] w-full items-center justify-center bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
        }
      </main>
    </div>
  `,
})
export class MapPage {}
