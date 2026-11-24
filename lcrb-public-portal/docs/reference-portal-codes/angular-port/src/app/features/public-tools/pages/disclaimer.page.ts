/** Static content inside LegalPage (metadata is set by the router). */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LegalPage } from '@shared/ui/bc-gov/legal-page';
import { path } from '@/app/route-registry';

@Component({
  selector: 'disclaimer-page',
  imports: [LegalPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <legal-page
      title="Disclaimer"
      intro="Terms governing your use of the BC Liquor and Cannabis Regulation Branch (LCRB) online licensing portal."
      updated="June 1, 2026"
    >
      <p>The Province of British Columbia and the Liquor and Cannabis Regulation Branch (LCRB) provide this website and its content for general information only. While we make every effort to ensure the information is accurate and current, it is provided &ldquo;as is&rdquo; without warranty of any kind, either express or implied.</p>
      <h2>No warranty</h2>
      <p>The Province does not warrant the accuracy, completeness, or currency of any information on this site, and is not responsible for any loss or damage of any kind arising from your use of, or reliance on, this website or any linked third-party site. Information about licences, permits, fees, and timelines may change without notice.</p>
      <h2>Not legal advice</h2>
      <p>Nothing on this website constitutes legal advice. The governing law is set out in the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/15019" target="_blank" rel="noopener noreferrer">Liquor Control and Licensing Act</a>, the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/18006" target="_blank" rel="noopener noreferrer">Cannabis Control and Licensing Act</a>, and their associated regulations. If there is any discrepancy between the information on this site and the legislation, the legislation prevails.</p>
      <h2>External links</h2>
      <p>This website may contain links to external sites that are not operated by the Province. These links are provided for convenience only, and the Province is not responsible for the content, accuracy, or privacy practices of any external site.</p>
      <h2>Service availability</h2>
      <p>Access to online services may be interrupted for maintenance, upgrades, or reasons beyond our control. The Province is not liable for any consequences of service unavailability.</p>
      <h2>Questions</h2>
      <p>If you have questions about this disclaimer, please <a [href]="path('public.contact')">contact the LCRB</a>.</p>
    </legal-page>
  `,
})
export class DisclaimerPage {
  protected readonly path = path;}
