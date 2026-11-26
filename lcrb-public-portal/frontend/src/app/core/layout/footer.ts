/** BC Gov site footer. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BCLogo } from '@shared/ui/bc-gov/bc-logo';
import { path } from '@/app/route-registry';

@Component({
  selector: 'bc-gov-footer',
  imports: [RouterLink, BCLogo],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <footer class="w-full border-t-2 border-[#fcba19] bg-white text-[#2d2d2d]">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <!-- Logo and Help -->
          <div class="flex items-center gap-4">
            <bc-logo class="h-9 w-auto" />
            <div class="text-sm text-[#474543]">
              <p>We can help if you have any issues.</p>
              <a [routerLink]="path('public.contact')" class="font-medium text-[#255A90] hover:underline">Call or email us</a>
            </div>
          </div>

          <!-- Links -->
          <nav class="flex flex-wrap gap-x-6 gap-y-2 text-sm">
            <a routerLink="/" class="text-[#474543] hover:text-[#013366] hover:underline transition-colors">Home</a>
            <a [routerLink]="path('public.disclaimer')" class="text-[#474543] hover:text-[#013366] hover:underline transition-colors">Disclaimer</a>
            <a [routerLink]="path('public.privacy')" class="text-[#474543] hover:text-[#013366] hover:underline transition-colors">Privacy</a>
            <a [routerLink]="path('public.accessibility')" class="text-[#474543] hover:text-[#013366] hover:underline transition-colors">Accessibility</a>
            <a [routerLink]="path('public.copyright')" class="text-[#474543] hover:text-[#013366] hover:underline transition-colors">Copyright</a>
          </nav>
        </div>
      </div>
    </footer>
  `,
})
export class BCGovFooter {
  protected readonly path = path;}
