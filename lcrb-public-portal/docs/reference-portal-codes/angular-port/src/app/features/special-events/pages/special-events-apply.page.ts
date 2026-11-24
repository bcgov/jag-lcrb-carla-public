/** SEP application on the shared framework (classic or Guided); submits to the SEP store, then reloads to `/special-events?submitted=<id>`. Helpers are components. */
import { ChangeDetectionStrategy, Component, DestroyRef, afterNextRender, computed, inject, input } from '@angular/core';
import { toast } from '@spartan-ng/brain/sonner';
import {
  ApplicationHeaderAction,
  ApplicationLayout,
  ApplicationNavigation,
  ApplicationProvider,
  ApplicationSection,
  InfoBox,
  injectApplicationContext,
  type ApplicationStep,
} from '@shared/forms';
import { AssistantFormBridge, type BridgeFieldMeta } from '@shared/forms/assistant-bridge';
import {
  AreaFieldF,
  CheckboxFieldF,
  CheckboxGroupFieldF,
  Conditional,
  RadioFieldF,
  SelectFieldF,
  TextFieldF,
  YES_NO,
} from '@shared/forms/fields';
import { FocusFlow } from '@shared/forms/focus-flow';
import { ModeExplainer } from '@shared/forms/mode-explainer';
import { ApplicationModeToggle } from '@shared/forms/mode-toggle';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { SepAccessGuard } from '@core/permissions/sep-access-guard';
import { formatTimeOfDay } from '@domain/special-events/seps';
import { SepApplyFacade } from '../facades/sep-apply.facade';
import { SEP_FIELD_HELP } from '@domain/ai/portal-knowledge';
import { consumeServingsPrefill } from '@core/navigation/servings-prefill';
import { injectApplicationMode } from '@shared/forms/application-mode';
import type { FieldValidation } from '@domain/forms/field-validation';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

/** Field metadata published to the AI assistant so it can read, explain, highlight and fill the form; mirrors the fields rendered in StepContent by step index. */
const sepFieldMeta: BridgeFieldMeta[] = [
  { step: 0, name: 'eventName', label: 'Event Name', type: 'text', required: true, help: SEP_FIELD_HELP['eventName'] },
  { step: 0, name: 'termsAccepted', label: 'Agree to terms and conditions', type: 'checkbox', required: true },
  { step: 1, name: 'isPrivateResidence', label: 'Hosted at a private residence?', type: 'radio', required: true, help: SEP_FIELD_HELP['isPrivateResidence'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 1, name: 'isPublicProperty', label: 'Held on public property?', type: 'radio', required: true, help: SEP_FIELD_HELP['isPublicProperty'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 1, name: 'isSignificantEvent', label: 'Event of provincial/national significance?', type: 'radio', required: true, help: SEP_FIELD_HELP['isSignificantEvent'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 1, name: 'eventType', label: 'Private or public event?', type: 'select', required: true, help: SEP_FIELD_HELP['eventType'], options: [{ value: 'private', label: 'Private - Invited guests only' }, { value: 'public', label: 'Public - Open to the public' }] },
  { step: 1, name: 'eventStartDate', label: 'Event start date', type: 'date', required: true, help: SEP_FIELD_HELP['eventStartDate'] },
  { step: 1, name: 'hasCertificate', label: 'Have your SIR/SES certificate?', type: 'radio', required: true, help: SEP_FIELD_HELP['hasCertificate'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No - I will complete training before my event' }] },
  { step: 1, name: 'certificateNumber', label: 'SIR/SES Certificate Number', type: 'text', help: SEP_FIELD_HELP['certificateNumber'] },
  { step: 1, name: 'eventOccasion', label: 'Occasion of your event', type: 'textarea', required: true, help: SEP_FIELD_HELP['eventOccasion'] },
  { step: 1, name: 'hasAdmissionCharge', label: 'Charging an admission price?', type: 'radio', required: true, help: SEP_FIELD_HELP['hasAdmissionCharge'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 2, name: 'eventCity', label: 'Special event city', type: 'text', required: true, help: SEP_FIELD_HELP['eventCity'] },
  { step: 2, name: 'isAnnualEvent', label: 'Is this an annual event?', type: 'radio', required: true, options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 2, name: 'locationName', label: 'Event Location Name', type: 'text', required: true, help: SEP_FIELD_HELP['locationName'] },
  { step: 2, name: 'venueType', label: 'Venue Type', type: 'text', required: true, help: SEP_FIELD_HELP['venueType'] },
  { step: 2, name: 'addressLine1', label: 'Address Line 1', type: 'text', required: true, help: SEP_FIELD_HELP['addressLine1'] },
  { step: 2, name: 'postalCode', label: 'Postal Code', type: 'text', required: true, help: SEP_FIELD_HELP['postalCode'] },
  { step: 2, name: 'maxGuests', label: 'Maximum number of guests', type: 'number', required: true, help: SEP_FIELD_HELP['maxGuests'] },
  { step: 2, name: 'eventDate', label: 'Date of your event', type: 'date', required: true, help: SEP_FIELD_HELP['eventDate'] },
  { step: 2, name: 'eventStartTime', label: 'Event starts', type: 'time', required: true, help: SEP_FIELD_HELP['eventStartTime'] },
  { step: 2, name: 'eventEndTime', label: 'Event ends', type: 'time', required: true, help: SEP_FIELD_HELP['eventEndTime'] },
  { step: 3, name: 'liquorTypes', label: 'Types of liquor served', type: 'checkbox-group', required: true, help: SEP_FIELD_HELP['liquorTypes'], options: [{ value: 'beer', label: 'Beer' }, { value: 'wine', label: 'Wine' }, { value: 'cider', label: 'Cider' }, { value: 'coolers', label: 'Coolers' }, { value: 'spirits', label: 'Spirits (private events only)' }] },
  { step: 3, name: 'liquorSaleType', label: 'Selling or free of charge?', type: 'radio', required: true, help: SEP_FIELD_HELP['liquorSaleType'], options: [{ value: 'selling', label: 'Selling' }, { value: 'free', label: 'Free of charge' }] },
  { step: 3, name: 'estimatedDrinks', label: 'Estimated total standard drinks', type: 'number', required: true, help: SEP_FIELD_HELP['estimatedDrinks'] },
  { step: 3, name: 'hasSecurity', label: 'Will you have security staff?', type: 'radio', required: true, help: SEP_FIELD_HELP['hasSecurity'], options: [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }] },
  { step: 3, name: 'securityStaffCount', label: 'Number of security staff', type: 'number', help: SEP_FIELD_HELP['securityStaffCount'] },
  { step: 4, name: 'confirmAccuracy', label: 'Confirm information is accurate', type: 'checkbox', required: true },
  { step: 4, name: 'acknowledgeFalseInfo', label: 'Acknowledge false-information consequences', type: 'checkbox', required: true },
];

const sepSteps: ApplicationStep[] = [
  { id: 'applicant', title: 'Applicant', description: 'Your details and terms' },
  { id: 'eligibility', title: 'Eligibility', description: 'Event type and requirements' },
  { id: 'location', title: 'Location', description: 'Venue and service areas' },
  { id: 'liquor', title: 'Liquor', description: 'Service details' },
  { id: 'summary', title: 'Summary', description: 'Review and submit' },
];

const DRAFT_META = {
  flowId: 'sep',
  title: 'Special Event Permit Application',
  resumeHref: path('special-events.apply'),
};

/** Formats a "YYYY-MM-DD" form value as "Sep 22, 2026" for the summary step. */
function formatIsoDate(value?: string): string {
  if (!value) return '';
  const parsed = new Date(`${value}T00:00:00`);
  if (Number.isNaN(parsed.getTime())) return value;
  return parsed.toLocaleDateString('en-CA', { month: 'short', day: 'numeric', year: 'numeric' });
}

/** One reviewed answer on the summary step. Renders an explicit "Not provided" rather than an empty cell so a skipped optional field is unambiguous. */
@Component({
  selector: 'div[sepSummaryItem]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <dt class="text-xs font-medium uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
    <dd [attr.class]="'mt-0.5 text-sm ' + (hasValue() ? 'text-foreground' : 'text-muted-foreground italic')">
      {{ display() }}
    </dd>
  `,
})
export class SepSummaryItem {
  readonly label = input.required<string>();
  readonly value = input<string | undefined>(undefined);
  protected readonly hasValue = computed(() => !!(this.value() && this.value()!.trim()));
  protected readonly display = computed(() => (this.hasValue() ? this.value()! : 'Not provided'));
}

/** Review-and-submit screen, extracted so Guided Mode renders it as its `reviewSlot`; both modes show the same answers, fee table and two required declaration checkboxes. */
@Component({
  selector: 'sep-summary-step',
  imports: [ApplicationSection, InfoBox, CheckboxFieldF, SepSummaryItem],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <div infoBox variant="info" [title]="'Almost done'">
        Please review your application details before submitting. Most SEP applications are approved instantly, but some
        require police review.
      </div>
      <div applicationSection [title]="'Application Summary'" [description]="'Review your application details'">
        <div class="space-y-6">
          <!-- The step promised a review but only rendered the fee table, so applicants couldn't check their answers before submitting. -->
          <div class="p-4 border rounded-lg">
            <h4 class="font-semibold text-[#013366] mb-3">Your answers</h4>
            <dl class="grid gap-x-6 gap-y-3 sm:grid-cols-2">
              <div sepSummaryItem label="Event name" [value]="str('eventName')"></div>
              <div sepSummaryItem label="Event type" [value]="eventTypeLabel()"></div>
              <div sepSummaryItem label="Date" [value]="dateLabel()"></div>
              <div sepSummaryItem label="Service window" [value]="windowLabel()"></div>
              <div sepSummaryItem label="Venue" [value]="str('locationName')"></div>
              <div sepSummaryItem label="Address" [value]="addressLabel()"></div>
              <div sepSummaryItem label="Maximum guests" [value]="str('maxGuests')"></div>
              <div sepSummaryItem label="Liquor service" [value]="serviceLabel()"></div>
              <div sepSummaryItem label="Liquor types" [value]="typesLabel()"></div>
              <div sepSummaryItem label="Estimated drinks" [value]="str('estimatedDrinks')"></div>
              <div sepSummaryItem label="Security staff" [value]="securityLabel()"></div>
            </dl>
          </div>
          <div class="p-4 border rounded-lg">
            <h4 class="font-semibold text-[#013366] mb-3">Fees</h4>
            <dl class="space-y-2 text-sm">
              <div class="flex justify-between">
                <dt class="text-muted-foreground">Application Fee</dt>
                <dd class="font-medium">$25.00</dd>
              </div>
              <div class="flex justify-between border-t pt-2">
                <dt class="font-semibold">Total</dt>
                <dd class="font-semibold">$25.00</dd>
              </div>
            </dl>
          </div>
          <div
            checkboxFieldF
            [name]="'confirmAccuracy'"
            [label]="'I confirm that all information provided is accurate and complete'"
            [required]="true"
          ></div>
          <div
            checkboxFieldF
            [name]="'acknowledgeFalseInfo'"
            [label]="'I understand that providing false information may result in permit denial or revocation'"
            [required]="true"
          ></div>
        </div>
      </div>
    </div>
  `,
})
export class SepSummaryStep {
  private readonly ctx = injectApplicationContext();
  protected str(key: string): string | undefined {
    return this.ctx.formData()[key] as string | undefined;
  }
  protected readonly eventTypeLabel = computed(() => {
    const t = this.str('eventType');
    return t === 'private' ? 'Private — invited guests only' : t === 'public' ? 'Public — open to the public' : '';
  });
  protected readonly dateLabel = computed(() => formatIsoDate(this.str('eventDate') || this.str('eventStartDate')));
  protected readonly windowLabel = computed(() => {
    const s = this.str('eventStartTime');
    const e = this.str('eventEndTime');
    return s && e ? `${formatTimeOfDay(s)} – ${formatTimeOfDay(e)}` : '';
  });
  protected readonly addressLabel = computed(() =>
    [this.str('addressLine1'), this.str('eventCity'), this.str('postalCode')].filter(Boolean).join(', '),
  );
  protected readonly serviceLabel = computed(() => {
    const t = this.str('liquorSaleType');
    return t === 'selling' ? 'Sale of liquor' : t ? 'Free of charge' : '';
  });
  protected readonly typesLabel = computed(() => {
    const t = this.ctx.formData()['liquorTypes'];
    return Array.isArray(t) ? (t as string[]).map((x) => x.charAt(0).toUpperCase() + x.slice(1)).join(', ') : '';
  });
  protected readonly securityLabel = computed(() => {
    const d = this.ctx.formData();
    if (d['hasSecurity'] === 'yes') return d['securityStaffCount'] ? `Yes — ${d['securityStaffCount']} on site` : 'Yes';
    return d['hasSecurity'] === 'no' ? 'No' : '';
  });
}

@Component({
  selector: 'sep-step-content',
  imports: [
    ApplicationSection,
    InfoBox,
    Conditional,
    TextFieldF,
    AreaFieldF,
    SelectFieldF,
    RadioFieldF,
    CheckboxFieldF,
    CheckboxGroupFieldF,
    SepSummaryStep,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @switch (ctx.currentStep()) {
      @case (0) {
        <div class="space-y-6">
          <div applicationSection [title]="'Event Name'" [description]="'Enter a descriptive name for your event'">
            <div
              textFieldF
              [name]="'eventName'"
              [label]="'Event Name'"
              [required]="true"
              [placeholder]="'e.g., Summer Community BBQ, Wedding Reception'"
              [validation]="v.eventName"
            ></div>
          </div>

          <div applicationSection [title]="'Applicant Information'" [description]="'Pulled from your account'">
            <div class="p-4 bg-muted rounded-lg">
              <p class="font-medium">Tyee_firstname_testing LRCB Training_surname_testing</p>
              <p class="text-sm text-muted-foreground">6667779999</p>
              <p class="text-sm text-muted-foreground">test&#64;test.com</p>
            </div>
            <p class="text-sm text-muted-foreground mt-2">
              To update your information, visit
              <a [href]="path('account.settings')" class="text-[#1A5A96] hover:underline">Manage Account</a>.
            </p>
          </div>

          <div
            applicationSection
            [title]="'General Terms and Conditions'"
            [description]="'Please read and acknowledge the Terms and Conditions to begin'"
          >
            <div class="p-4 border rounded-lg max-h-64 overflow-y-auto text-sm space-y-3">
              <h4 class="font-semibold">SEP Terms and Conditions</h4>
              <p>
                This is a summary of the most common terms and conditions governing Special Event Permits (SEPs). You
                must read and agree to these terms before you can continue this application.
              </p>
              <p>
                <strong>Private and Public Special Event Permits:</strong> A &quot;private special event&quot; is an
                event at which attendance is limited to invited guests or members of the organization hosting the event.
                A &quot;public special event&quot; is an event at which attendance is open to the public either for no
                charge or by purchase of a ticket at the door.
              </p>
              <p>
                <strong>Host Responsibilities:</strong> The SEP holder, or host, is responsible for the liquor service
                provided under this permit. At all times, a person appointed by the host organization must be
                responsible for liquor service and hold a Responsible Beverage Service certificate.
              </p>
              <p>
                <strong>Types of Liquor:</strong> At a private special event you may sell or serve any type of liquor.
                At a public special event you may sell or serve beer, cider, coolers and wine only.
              </p>
            </div>
            <div class="mt-4">
              <div
                checkboxFieldF
                [name]="'termsAccepted'"
                [label]="'I have read and agree to these terms and conditions'"
                [required]="true"
              ></div>
            </div>
          </div>
        </div>
      }
      @case (1) {
        <div class="space-y-6">
          <div
            applicationSection
            [title]="'Tell us about your event'"
            [description]="'Used to determine eligibility and what type of permit you require'"
          >
            <div class="space-y-6">
              <div
                radioFieldF
                [name]="'isPrivateResidence'"
                [label]="'Is this event being hosted at a private residence?'"
                [required]="true"
                [options]="yesNo"
                [hint]="'Private residences are not eligible for Special Event Permits.'"
              ></div>
              <ng-container *conditional="formData()['isPrivateResidence'] === 'yes'">
                <div infoBox variant="error" [title]="'Not eligible for a Special Event Permit'">
                  Events at a private residence are not eligible for a SEP. Please review the eligibility requirements
                  before continuing.
                </div>
              </ng-container>
              <div
                radioFieldF
                [name]="'isPublicProperty'"
                [label]="'Is your event being held on public property?'"
                [required]="true"
                [options]="yesNo"
                [hint]="'Events on public property must have written approval from local government or First Nations.'"
              ></div>
              <ng-container *conditional="formData()['isPublicProperty'] === 'yes'">
                <div infoBox variant="warning" [title]="'Local government approval required'">
                  You will need written approval from the local government or First Nation. You can upload this document
                  later in the application.
                </div>
              </ng-container>
              <div
                radioFieldF
                [name]="'isSignificantEvent'"
                [label]="'Is this an event of provincial, national, or international significance?'"
                [required]="true"
                [options]="yesNo"
              ></div>
              <div
                selectFieldF
                [name]="'eventType'"
                [label]="'Is your event private or public?'"
                [required]="true"
                [options]="eventTypeOptions"
              ></div>
              <div
                textFieldF
                [name]="'eventStartDate'"
                [label]="'When does your event start?'"
                [type]="'date'"
                [required]="true"
                [hint]="'We recommend applying at least 14 days before your event.'"
                [validation]="v.eventStartDate"
              ></div>
              <div
                radioFieldF
                [name]="'hasCertificate'"
                [label]="'Do you have your Serving It Right (SIR) or Special Event Server (SES) certificate?'"
                [required]="true"
                [options]="certificateOptions"
              ></div>
              <ng-container *conditional="formData()['hasCertificate'] === 'yes'">
                <div
                  textFieldF
                  [name]="'certificateNumber'"
                  [label]="'SIR/SES Certificate Number'"
                  [placeholder]="'Enter your certificate number'"
                  [required]="true"
                  [validation]="v.certificateNumber"
                ></div>
              </ng-container>
              <div
                areaFieldF
                [name]="'eventOccasion'"
                [label]="'What\\'s the occasion of your event? (maximum 255 characters)'"
                [required]="true"
                [placeholder]="'Describe the purpose or occasion of your event'"
                [validation]="v.eventOccasion"
              ></div>
              <div
                radioFieldF
                [name]="'hasAdmissionCharge'"
                [label]="'Are you charging an event admission price?'"
                [required]="true"
                [options]="yesNo"
              ></div>
            </div>
          </div>
        </div>
      }
      @case (2) {
        <div class="space-y-6">
          <div
            applicationSection
            [title]="'Event Information'"
            [description]="'All locations must be in the same city/district/Indigenous Nation.'"
          >
            <div class="space-y-6">
              <div
                textFieldF
                [name]="'eventCity'"
                [label]="'Special event city'"
                [required]="true"
                [placeholder]="'Start typing to search cities...'"
                [validation]="v.required"
              ></div>
              <div radioFieldF [name]="'isAnnualEvent'" [label]="'Is this an annual event?'" [required]="true" [options]="yesNo"></div>
            </div>
          </div>

          <div applicationSection [title]="'Location 1'" [description]="'Enter the details for your event venue'">
            <div class="space-y-6">
              <div
                textFieldF
                [name]="'locationName'"
                [label]="'Event Location Name'"
                [required]="true"
                [placeholder]="'e.g., Community Centre, Hotel Ballroom'"
                [validation]="v.required"
              ></div>
              <div
                textFieldF
                [name]="'venueType'"
                [label]="'Venue Type'"
                [required]="true"
                [placeholder]="'e.g., community centre, convention centre, banquet hall'"
                [validation]="v.required"
              ></div>
              <div
                textFieldF
                [name]="'addressLine1'"
                [label]="'Address Line 1'"
                [required]="true"
                [placeholder]="'Street address'"
                [validation]="v.required"
              ></div>
              <div
                textFieldF
                [name]="'postalCode'"
                [label]="'Postal Code'"
                [required]="true"
                [placeholder]="'V0V 0V0'"
                [validation]="v.postalCode"
              ></div>
              <div
                textFieldF
                [name]="'maxGuests'"
                [label]="'Maximum number of guests at this location at any time'"
                [type]="'number'"
                [required]="true"
                [hint]="'Events expecting 500 or more guests are routed to police for review.'"
                [validation]="v.maxGuests"
              ></div>
            </div>
          </div>

          <div applicationSection [title]="'Event Date(s)'" [description]="'Indoor: 9 a.m.–2 a.m. Outdoor: 9 a.m.–10 p.m.'">
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div
                textFieldF
                [name]="'eventDate'"
                [label]="'Date of your event'"
                [type]="'date'"
                [required]="true"
                [validation]="v.eventDate"
              ></div>
              <div
                textFieldF
                [name]="'eventStartTime'"
                [label]="'Event starts'"
                [type]="'time'"
                [required]="true"
                [validation]="v.required"
              ></div>
              <div
                textFieldF
                [name]="'eventEndTime'"
                [label]="'Event ends'"
                [type]="'time'"
                [required]="true"
                [validation]="v.required"
              ></div>
            </div>
          </div>
        </div>
      }
      @case (3) {
        <div class="space-y-6">
          <div applicationSection [title]="'Liquor Details'" [description]="'Tell us about the liquor you plan to serve'">
            <div class="space-y-6">
              <div
                checkboxGroupFieldF
                [name]="'liquorTypes'"
                [label]="'What types of liquor will you serve?'"
                [required]="true"
                [validation]="v.liquorTypes"
                [options]="liquorOptions"
              ></div>
              <ng-container *conditional="spiritsAtPublic()">
                <div infoBox variant="error" [title]="'Spirits not allowed at public events'">
                  Public special events may only serve beer, cider, coolers and wine. Remove spirits or change your
                  event to private.
                </div>
              </ng-container>
              <div
                radioFieldF
                [name]="'liquorSaleType'"
                [label]="'Will you be selling liquor or providing it free of charge?'"
                [required]="true"
                [options]="saleOptions"
              ></div>
              <div
                textFieldF
                [name]="'estimatedDrinks'"
                [label]="'Estimated total number of standard drinks to be served'"
                [type]="'number'"
                [required]="true"
                [hint]="'Use our Servings Calculator in the Resources section to estimate this.'"
                [validation]="v.estimatedDrinks"
              ></div>
            </div>
          </div>

          <div applicationSection [title]="'Security'" [description]="'Security requirements for your event'">
            <div class="space-y-6">
              <div
                radioFieldF
                [name]="'hasSecurity'"
                [label]="'Will you have security staff at your event?'"
                [required]="true"
                [options]="yesNo"
              ></div>
              <ng-container *conditional="formData()['hasSecurity'] === 'yes'">
                <div
                  textFieldF
                  [name]="'securityStaffCount'"
                  [label]="'Number of security staff'"
                  [type]="'number'"
                  [required]="true"
                  [validation]="v.securityStaffCount"
                ></div>
              </ng-container>
            </div>
          </div>
        </div>
      }
      @case (4) {
        <sep-summary-step />
      }
    }
  `,
})
export class SepStepContent {
  protected readonly path = path;
  protected readonly ctx = injectApplicationContext();
  protected readonly formData = this.ctx.formData;
  protected readonly yesNo = YES_NO;
  protected readonly eventTypeOptions = [
    { value: 'private', label: 'Private - Invited guests only' },
    { value: 'public', label: 'Public - Open to the public' },
  ];
  protected readonly certificateOptions = [
    { value: 'yes', label: 'Yes' },
    { value: 'no', label: 'No - I will complete training before my event' },
  ];
  protected readonly liquorOptions = [
    { value: 'beer', label: 'Beer' },
    { value: 'wine', label: 'Wine' },
    { value: 'cider', label: 'Cider' },
    { value: 'coolers', label: 'Coolers' },
    { value: 'spirits', label: 'Spirits (private events only)' },
  ];
  protected readonly saleOptions = [
    { value: 'selling', label: 'Selling' },
    { value: 'free', label: 'Free of charge' },
  ];
  protected readonly spiritsAtPublic = computed(() => {
    const d = this.ctx.formData();
    return d['eventType'] === 'public' && Array.isArray(d['liquorTypes']) && (d['liquorTypes'] as string[]).includes('spirits');
  });
  protected readonly v: Record<string, FieldValidation> = {
    required: { required: true },
    eventName: { required: true, maxLength: 120 },
    eventStartDate: {
      required: true,
      notPast: true,
      warnIfWithinDays: {
        days: 14,
        message:
          'This event is less than 14 days away. You can still apply, but it may not be reviewed in time — especially if it needs police or local government review.',
      },
    },
    certificateNumber: { required: 'Enter your certificate number.', minLength: 4 },
    eventOccasion: { required: true, maxLength: 255 },
    postalCode: { required: true, postalCode: true },
    maxGuests: { required: true, min: 1 },
    eventDate: { required: true, notPast: true },
    liquorTypes: { required: 'Select at least one type of liquor.' },
    estimatedDrinks: { required: true, min: 1 },
    securityStaffCount: { required: 'Enter the number of security staff.', min: 1 },
  };
}

@Component({
  selector: 'sep-application-inner',
  imports: [
    ApplicationLayout,
    ApplicationHeaderAction,
    ApplicationNavigation,
    ApplicationModeToggle,
    AssistantFormBridge,
    FocusFlow,
    ModeExplainer,
    SepStepContent,
    SepSummaryStep,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <!-- Render the classic form until the stored mode preference has been read on the client, so the first paint is the same as before hydration. -->
    @if (hydrated() && mode() === 'guided') {
      <!-- The same review component the classic form renders, so the two modes can never disagree about what is being submitted. -->
      <ng-template #reviewSlot><sep-summary-step /></ng-template>
      <!-- This flow submits synchronously and then navigates, so there is no pending window to report. -->
      <focus-flow
        [config]="guidedConfig"
        (exitMode)="setMode('form')"
        (submitFlow)="handleSubmit()"
        [submitting]="false"
        [reviewSlot]="reviewSlot"
      />
    } @else {
      <application-layout
        [title]="'Special Event Permit Application'"
        [applicationName]="'Apply for a permit to serve liquor at your special event'"
      >
        @if (hydrated()) {
          <application-mode-toggle appHeaderAction [mode]="mode()" (modeChange)="setMode($event)" />
        }
        <assistant-form-bridge [flowTitle]="'Special Event Permit Application'" [fieldMeta]="fieldMeta" />
        <!-- First-visit explainer, only on the opening step. -->
        @if (hydrated() && ctx.currentStep() === 0) {
          <div class="mb-6">
            <mode-explainer [mode]="mode()" (modeChange)="setMode($event)" />
          </div>
        }
        <sep-step-content />
        <application-navigation [onSave]="goToList" [onSubmit]="handleSubmit" />
      </application-layout>
    }
  `,
})
export class SepApplicationInner {
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  protected readonly ctx = injectApplicationContext();
  private readonly sepApply = inject(SepApplyFacade);
  private readonly applicationMode = injectApplicationMode();
  protected readonly mode = this.applicationMode.mode;
  protected readonly setMode = this.applicationMode.setMode;
  protected readonly hydrated = this.applicationMode.hydrated;
  protected readonly guidedConfig = this.specialEventsRepository.guidedConfig();
  protected readonly fieldMeta = sepFieldMeta;

  constructor() {
    // Carry the Servings Calculator estimate into the liquor-service fields; deferred to a timer so it lands after draft hydration and wins over any stale saved value.
    const destroyRef = inject(DestroyRef);
    afterNextRender(() => {
      const prefill = consumeServingsPrefill();
      if (!prefill) return;
      const timer = setTimeout(() => {
        const updates: Record<string, unknown> = {};
        if (prefill.estimatedDrinks) updates['estimatedDrinks'] = prefill.estimatedDrinks;
        if (prefill.maxGuests) updates['maxGuests'] = prefill.maxGuests;
        if (prefill.liquorTypes.length) updates['liquorTypes'] = prefill.liquorTypes;
        if (Object.keys(updates).length === 0) return;
        this.ctx.updateFormData(updates);
        toast.info('Prefilled from the Servings Calculator', {
          description:
            'We carried over your estimated drinks, guest count, and liquor types. Review and adjust as needed.',
        });
      }, 0);
      destroyRef.onDestroy(() => clearTimeout(timer));
    });
  }

  protected readonly goToList = (): void => {
    window.location.assign(path('special-events.list'));
  };

  // Submit orchestration lives in the SEP apply facade.
  protected readonly handleSubmit = (): void => {
    this.sepApply.submit(this.ctx.formData());
  };
}

@Component({
  selector: 'special-events-apply-page',
  imports: [SepAccessGuard, BCGovHeader, BCGovFooter, ApplicationProvider, SepApplicationInner],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <sep-access-guard>
      <div class="min-h-screen flex flex-col">
        <bc-gov-header currentPage="/special-events" />
        <main id="main-content" class="flex-1">
          <application-provider [steps]="steps" [draftId]="'draft:sep'" [draftMeta]="draftMeta">
            <sep-application-inner />
          </application-provider>
        </main>
        <bc-gov-footer />
      </div>
    </sep-access-guard>
  `,
})
export class SpecialEventsApplyPage {
  protected readonly steps = sepSteps;
  protected readonly draftMeta = DRAFT_META;
}
