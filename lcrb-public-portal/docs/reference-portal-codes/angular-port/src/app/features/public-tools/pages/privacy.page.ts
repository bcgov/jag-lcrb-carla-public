/** Static content inside LegalPage (metadata is set by the router). */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LegalPage } from '@shared/ui/bc-gov/legal-page';
import { path } from '@/app/route-registry';

@Component({
  selector: 'privacy-page',
  imports: [LegalPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <legal-page
      title="Privacy"
      intro="How the Liquor and Cannabis Regulation Branch collects, uses, and protects your personal information."
      updated="June 1, 2026"
    >
      <p>Your privacy is important to us. The Liquor and Cannabis Regulation Branch (LCRB) collects, uses, and discloses personal information in accordance with the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/96165_00" target="_blank" rel="noopener noreferrer">Freedom of Information and Protection of Privacy Act</a>{{ ' ' }}(FOIPPA).</p>
      <h2>What we collect</h2>
      <p>When you use this portal we may collect personal and business information that you provide, including your name, contact details, BCeID or BC Services Card identifiers, business and establishment details, and the contents of your licence, authorization, and permit applications.</p>
      <h2>How we use it</h2>
      <ul>
      <li>To process and assess your licence, authorization, and permit applications.</li>
      <li>To verify your identity and communicate with you about your submissions.</li>
      <li>To administer and enforce the liquor and cannabis licensing framework.</li>
      <li>To refer applications to local governments, Indigenous nations, and police where required.</li>
      </ul>
      <h2>Authority to collect</h2>
      <p>Personal information is collected under the authority of the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/15019" target="_blank" rel="noopener noreferrer">Liquor Control and Licensing Act</a>{{ ' ' }}and the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/18006" target="_blank" rel="noopener noreferrer">Cannabis Control and Licensing Act</a>, and section 26 of FOIPPA.</p>
      <h2>Protecting your information</h2>
      <p>The Province uses reasonable physical, technical, and administrative safeguards to protect your information against unauthorized access, collection, use, disclosure, or disposal.</p>
      <h2>Access and correction</h2>
      <p>You have the right to request access to, and correction of, your own personal information. To make a request or ask a question about the management of your personal information, please <a [href]="path('public.contact')">contact the LCRB</a>.</p>
    </legal-page>
  `,
})
export class PrivacyPage {
  protected readonly path = path;}
