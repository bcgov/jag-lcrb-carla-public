/** Static content inside LegalPage (metadata is set by the router). */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LegalPage } from '@shared/ui/bc-gov/legal-page';
import { path } from '@/app/route-registry';

@Component({
  selector: 'accessibility-page',
  imports: [LegalPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <legal-page
      title="Accessibility"
      intro="We are committed to making the licensing portal usable by everyone, including people with disabilities."
      updated="June 1, 2026"
    >
      <p>The Province of British Columbia is committed to ensuring digital services are accessible to people of all abilities, in line with the{{ ' ' }}<a href="https://www.bclaws.gov.bc.ca/civix/document/id/complete/statreg/21019" target="_blank" rel="noopener noreferrer">Accessible British Columbia Act</a>.</p>
      <h2>Our standard</h2>
      <p>This portal is designed to conform with the Web Content Accessibility Guidelines (WCAG) 2.1 at Level AA. We aim to provide content that is perceivable, operable, understandable, and robust for all users.</p>
      <h2>Accessibility features</h2>
      <ul>
      <li>Keyboard navigation support across all pages and forms.</li>
      <li>Semantic structure and ARIA labelling for use with screen readers.</li>
      <li>Text and interface controls that meet AA colour-contrast ratios.</li>
      <li>An in-page accessibility toolbar for adjusting text size and contrast.</li>
      <li>Descriptive alternative text for meaningful images.</li>
      </ul>
      <h2>Using assistive technology</h2>
      <p>This site is intended to work with current versions of common assistive technologies, including screen readers and speech-recognition software, on up-to-date browsers.</p>
      <h2>Giving us feedback</h2>
      <p>If you encounter a barrier using this website, or need information in an alternate format, please <a [href]="path('public.contact')">contact the LCRB</a>. We welcome your feedback and will make reasonable efforts to provide the information or service you need.</p>
    </legal-page>
  `,
})
export class AccessibilityPage {
  protected readonly path = path;}
