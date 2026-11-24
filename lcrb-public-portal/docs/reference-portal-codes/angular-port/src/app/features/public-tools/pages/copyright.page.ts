/** Static content inside LegalPage (metadata is set by the router). */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LegalPage } from '@shared/ui/bc-gov/legal-page';
import { path } from '@/app/route-registry';

@Component({
  selector: 'copyright-page',
  imports: [LegalPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <legal-page
      title="Copyright"
      intro="Terms for using and reproducing content published on this website."
      updated="June 1, 2026"
    >
      <p>&copy; {{ year }} Province of British Columbia. All rights reserved.</p>
      <p>The materials on this website are protected by Crown copyright, except where otherwise noted. Content is owned by the Government of British Columbia and the Liquor and Cannabis Regulation Branch (LCRB) unless attributed to another source.</p>
      <h2>Permitted use</h2>
      <p>You may reproduce information on this website for your own personal, non-commercial use, or for non-commercial purposes within your organization, provided that:</p>
      <ul>
      <li>The information is reproduced accurately and not used in a misleading way.</li>
      <li>The Province of British Columbia is identified as the source.</li>
      <li>The reproduction is not represented as an official version of the materials, nor as having been made in affiliation with or with the endorsement of the Province.</li>
      </ul>
      <h2>Commercial use</h2>
      <p>Any commercial reproduction, distribution, or use of materials from this website requires prior written permission. For permission requests, please{{ ' ' }}<a [href]="path('public.contact')">contact the LCRB</a>.</p>
      <h2>Trademarks</h2>
      <p>The British Columbia logo and other Province of British Columbia marks are official symbols of the Government of British Columbia and may not be used without written permission.</p>
    </legal-page>
  `,
})
export class CopyrightPage {
  protected readonly path = path;
  protected readonly year = new Date().getFullYear();
}
