/** Static content inside LegalPage (metadata is set by the router). */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LegalPage } from '@shared/ui/bc-gov/legal-page';

@Component({
  selector: 'contact-page',
  imports: [LegalPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <legal-page
      title="Contact Us"
      intro="Get help with liquor and cannabis licences, authorizations, and Special Event Permits."
    >
      <p>The Liquor and Cannabis Regulation Branch (LCRB) is here to help. If you have questions about an application, a licence, or using this portal, reach out using the details below.</p>
      <h2>Liquor &amp; Special Event Permit enquiries</h2>
      <p><strong>Phone:</strong> 1-866-209-2111 (toll-free in Canada) or 250-952-5787 (Victoria)<br /><strong>Email:</strong>{{ ' ' }}<a href="mailto:LCRBLiquor@gov.bc.ca">LCRBLiquor&#64;gov.bc.ca</a></p>
      <h2>Cannabis licensing enquiries</h2>
      <p><strong>Phone:</strong> 1-866-209-2111<br /><strong>Email:</strong>{{ ' ' }}<a href="mailto:LCRBCannabis@gov.bc.ca">LCRBCannabis&#64;gov.bc.ca</a></p>
      <h2>Cannabis policy enquiries</h2>
      <p><strong>Email:</strong>{{ ' ' }}<a href="mailto:Cannabisregs@gov.bc.ca">Cannabisregs&#64;gov.bc.ca</a></p>
      <h2>Complaints</h2>
      <p>To report a concern or file a complaint about a licensed establishment:<br /><strong>Email:</strong>{{ ' ' }}<a href="mailto:LCRBComplaints@gov.bc.ca">LCRBComplaints&#64;gov.bc.ca</a></p>
      <h2>Hours of operation</h2>
      <p>Monday to Friday, 8:30 a.m. to 4:30 p.m. Pacific Time, excluding statutory holidays.</p>
      <h2>Mailing address</h2>
      <p>Liquor and Cannabis Regulation Branch<br />PO Box 9292 Stn Prov Govt<br />Victoria, BC V8W 9J8</p>
      <h2>In-portal assistance</h2>
      <p>While signed in, you can also use the AI licensing assistant for instant answers to common questions about applications, requirements, and timelines.</p>
    </legal-page>
  `,
})
export class ContactPage {}
