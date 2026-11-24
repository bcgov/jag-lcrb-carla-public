/** Slide-over listing the followed licences; `followedLicenses` holds licence IDs (owned by the map experience) resolved against the `allLicenses` seed. The follow list is client-only state. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { CannabisIcon } from './icons/cannabis-icon';
import { LiquorIcon } from './icons/liquor-icon';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { LucideIcon } from '../primitives/lucide-icon';
import { ScrollArea } from '../primitives/scroll-area';
import { MAP_REPOSITORY } from '@data-access/repositories/map/map.repository';
import { type License } from '@domain/models/map';

@Component({
  selector: 'following-panel',
  imports: [Badge, Button, ...CARD, LucideIcon, ScrollArea, LiquorIcon, CannabisIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isOpen()) {
      <div class="absolute top-4 right-4 z-30 w-full max-w-sm bg-background rounded-lg shadow-xl border overflow-hidden">
        <div class="flex items-center justify-between p-4 border-b bg-muted/30">
          <div class="flex items-center gap-2">
            <svg lucide="Heart" class="w-5 h-5 text-red-500"></svg>
            <h2 class="font-semibold">Following</h2>
            <span uiBadge variant="secondary" class="ml-1">
              {{ followedLicenses().length }}
            </span>
          </div>
          <button uiButton variant="ghost" size="icon" (click)="closed.emit()">
            <svg lucide="X" class="w-4 h-4"></svg>
          </button>
        </div>

        <div uiScrollArea class="h-[400px]">
          <div class="p-4 space-y-3">
            @if (followedLicenseData().length === 0) {
              <div class="text-center py-8 text-muted-foreground">
                <svg lucide="Heart" class="w-12 h-12 mx-auto mb-3 opacity-20"></svg>
                <p class="font-medium">No followed locations</p>
                <p class="text-sm mt-1">Click the heart icon on any license location to follow it</p>
              </div>
            } @else {
              @for (license of followedLicenseData(); track license.id) {
                <div uiCard class="overflow-hidden">
                  <div uiCardContent class="p-3">
                    <div class="flex items-start gap-3">
                      <div class="mt-1">
                        @if (license.licenseType === 'liquor') {
                          <svg liquorIcon class="w-5 h-5 text-amber-600"></svg>
                        } @else if (license.licenseType === 'cannabis') {
                          <svg cannabisIcon class="w-5 h-5 text-green-600"></svg>
                        } @else {
                          <svg lucide="Building2" class="w-5 h-5 text-purple-600"></svg>
                        }
                      </div>
                      <div class="flex-1 min-w-0">
                        <h3 class="font-medium text-sm truncate">{{ license.name }}</h3>
                        <p class="text-xs text-muted-foreground truncate">{{ license.licenseNumber }}</p>
                        <div class="flex items-center gap-1 mt-1 text-xs text-muted-foreground">
                          <svg lucide="MapPin" class="w-3 h-3"></svg>
                          <span class="truncate">{{ license.city }}</span>
                        </div>
                        @if (license.activeStatus === 'suspended') {
                          <span uiBadge variant="destructive" class="mt-2 text-xs"> Suspended </span>
                        }
                      </div>
                      <div class="flex flex-col gap-1">
                        <button uiButton variant="outline" size="sm" class="text-xs bg-transparent" (click)="selectLicense.emit(license)">
                          <svg lucide="Navigation" class="w-3 h-3 mr-1"></svg>
                          View
                        </button>
                        <button
                          uiButton
                          variant="ghost"
                          size="sm"
                          class="text-xs text-red-500 hover:text-red-600 hover:bg-red-50"
                          (click)="toggleFollow.emit(license.id)"
                        >
                          <svg lucide="Heart" class="w-3 h-3 mr-1 fill-current"></svg>
                          Unfollow
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              }
            }
          </div>
        </div>
      </div>
    }
  `,
})
export class FollowingPanel {
  private readonly map = inject(MAP_REPOSITORY);
  readonly isOpen = input.required<boolean>();
  readonly followedLicenses = input.required<string[]>();
  readonly closed = output<void>();
  readonly selectLicense = output<License>();
  readonly toggleFollow = output<string>();

  protected readonly followedLicenseData = computed(() => this.map.licenses().filter((l) => this.followedLicenses().includes(l.id)));
}
