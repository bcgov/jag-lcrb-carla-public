/** Legend popover with per-type counts. */
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { injectAuth } from './map-auth';
import { Button } from '../primitives/button';
import { LucideIcon } from '../primitives/lucide-icon';
import { POPOVER } from '../primitives/popover';

@Component({
  selector: 'map-legend',
  imports: [...POPOVER, Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="absolute bottom-6 left-4 z-10">
      <ui-popover align="start">
        <button uiPopoverTrigger uiButton variant="secondary" size="sm" class="shadow-lg">
          <svg lucide="Info" class="w-4 h-4 mr-2"></svg>
          Legend
        </button>
        <div uiPopoverContent *uiPopoverPortal class="w-64">
          <div class="space-y-3">
            <h4 class="font-semibold text-sm">Map Legend</h4>

            <div class="space-y-2">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <div class="p-1.5 bg-[#FFF4ED] rounded">
                    <svg lucide="Wine" class="w-4 h-4 text-[#C4784A]"></svg>
                  </div>
                  <span class="text-sm">Liquor License</span>
                </div>
                <span class="text-sm text-muted-foreground">{{ totalLiquor() }}</span>
              </div>

              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <div class="p-1.5 bg-[#E8F5E9] rounded">
                    <svg lucide="Leaf" class="w-4 h-4 text-[#4A9C6D]"></svg>
                  </div>
                  <span class="text-sm">Cannabis License</span>
                </div>
                <span class="text-sm text-muted-foreground">{{ totalCannabis() }}</span>
              </div>

              @if (auth.canViewSpecialEvents()) {
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <div class="p-1.5 bg-[#EDE7F6] rounded">
                      <svg lucide="PartyPopper" class="w-4 h-4 text-[#7C4DFF]"></svg>
                    </div>
                    <span class="text-sm">Special Event Permit</span>
                  </div>
                  <span class="text-sm text-muted-foreground">{{ totalSpecialEvents() }}</span>
                </div>
              }

              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <div class="p-1.5 bg-red-50 rounded">
                    <svg lucide="AlertTriangle" class="w-4 h-4 text-red-500"></svg>
                  </div>
                  <span class="text-sm">With Contraventions</span>
                </div>
                <span class="text-sm text-muted-foreground">{{ totalWithContraventions() }}</span>
              </div>

              @if (auth.isStaff()) {
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <div class="p-1.5 bg-amber-50 rounded">
                      <svg lucide="FileText" class="w-4 h-4 text-amber-500"></svg>
                    </div>
                    <span class="text-sm">Under Application</span>
                  </div>
                  <span class="text-sm text-muted-foreground">{{ totalApplications() }}</span>
                </div>
              }
            </div>
          </div>
        </div>
      </ui-popover>
    </div>
  `,
})
export class MapLegend {
  readonly totalLiquor = input.required<number>();
  readonly totalCannabis = input.required<number>();
  readonly totalSpecialEvents = input.required<number>();
  readonly totalWithContraventions = input.required<number>();
  readonly totalApplications = input.required<number>();
  protected readonly auth = injectAuth();
}
