import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { AnnouncementsList } from '../components/announcements-list';
import { path } from '@/app/route-registry';

@Component({
  selector: 'announcements-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, AnnouncementsList],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col">
      <bc-gov-header currentPage="/announcements" />
      <main id="main-content" class="flex-1 bg-muted/20">
        <page-hero
          containerClassName="mx-auto max-w-3xl px-4 sm:px-6 lg:px-8"
          [breadcrumbs]="breadcrumbs"
          title="Announcements"
          description="Policy updates, regulatory changes, and Portal release notes from the Liquor and Cannabis Regulation Branch."
        />
        <announcements-list />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class AnnouncementsPage {
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Announcements' }];
}
