/* The Special Event Permit flow as data. */

import type { FlowConfig } from "@domain/forms/flow-config"
import { YES_NO as yesNo } from "@domain/forms/field-schema"
import { SEP_FIELD_HELP } from "@domain/ai/portal-knowledge"

/** Guided Mode's view of this application. */
export const sepGuidedConfig: FlowConfig = {
  id: "sep",
  title: "Special Event Permit Application",
  subtitle: "Apply for a permit to serve liquor at your special event",
  fee: "$25.00",
  returnTo: "/special-events",
  currentNav: "/special-events",
  steps: [
    {
      id: "applicant",
      title: "Applicant",
      sections: [
        {
          title: "Event name and terms",
          fields: [
            {
              name: "eventName",
              label: "Event Name",
              type: "text",
              required: true,
              placeholder: "e.g., Summer Community BBQ, Wedding Reception",
              hint: "Give your event a descriptive name.",
              validation: { required: true, maxLength: 120 },
              help: SEP_FIELD_HELP.eventName,
            },
            {
              name: "termsAccepted",
              label: "I have read and agree to the Special Event Permit terms and conditions",
              type: "checkbox",
              required: true,
              // testers pointed out Guided Mode asked them to agree to terms they had never been shown.
              readBefore: {
                title: "SEP Terms and Conditions",
                paragraphs: [
                  {
                    text: "This is a summary of the most common terms and conditions governing Special Event Permits (SEPs). You must read and agree to these terms before you can continue this application.",
                  },
                  {
                    heading: "Private and Public Special Event Permits:",
                    text: 'A "private special event" is an event at which attendance is limited to invited guests or members of the organization hosting the event. A "public special event" is an event at which attendance is open to the public either for no charge or by purchase of a ticket at the door.',
                  },
                  {
                    heading: "Host Responsibilities:",
                    text: "The SEP holder, or host, is responsible for the liquor service provided under this permit. At all times, a person appointed by the host organization must be responsible for liquor service and hold a Responsible Beverage Service certificate.",
                  },
                  {
                    heading: "Types of Liquor:",
                    text: "At a private special event you may sell or serve any type of liquor. At a public special event you may sell or serve beer, cider, coolers and wine only.",
                  },
                ],
              },
            },
          ],
        },
      ],
    },
    {
      id: "eligibility",
      title: "Eligibility",
      sections: [
        {
          title: "Tell us about your event",
          fields: [
            {
              name: "isPrivateResidence",
              label: "Is this event being hosted at a private residence?",
              type: "radio",
              required: true,
              options: yesNo,
              hint: "Private residences are not eligible for Special Event Permits.",
              help: SEP_FIELD_HELP.isPrivateResidence,
            },
            {
              name: "isPublicProperty",
              label: "Is your event being held on public property?",
              type: "radio",
              required: true,
              options: yesNo,
              hint: "Events on public property need written approval from the local government or First Nation. You can upload it later in the application.",
              help: SEP_FIELD_HELP.isPublicProperty,
            },
            {
              name: "isSignificantEvent",
              label: "Is this an event of provincial, national, or international significance?",
              type: "radio",
              required: true,
              options: yesNo,
              help: SEP_FIELD_HELP.isSignificantEvent,
            },
            {
              name: "eventType",
              label: "Is your event private or public?",
              type: "select",
              required: true,
              options: [
                { value: "private", label: "Private - Invited guests only" },
                { value: "public", label: "Public - Open to the public" },
              ],
              help: SEP_FIELD_HELP.eventType,
            },
            {
              name: "eventStartDate",
              label: "When does your event start?",
              type: "date",
              required: true,
              hint: "We recommend applying at least 14 days before your event.",
              // was a hard `minDaysFromNow: 14` stop.
              validation: {
                required: true,
                notPast: true,
                warnIfWithinDays: {
                  days: 14,
                  message:
                    "This event is less than 14 days away. You can still apply, but it may not be reviewed in time — especially if it needs police or local government review.",
                },
              },
              help: SEP_FIELD_HELP.eventStartDate,
            },
            {
              name: "hasCertificate",
              label: "Do you have your Serving It Right (SIR) or Special Event Server (SES) certificate?",
              type: "radio",
              required: true,
              options: [
                { value: "yes", label: "Yes" },
                { value: "no", label: "No - I will complete training before my event" },
              ],
              help: SEP_FIELD_HELP.hasCertificate,
            },
            {
              name: "certificateNumber",
              label: "SIR/SES Certificate Number",
              type: "text",
              required: true,
              placeholder: "Enter your certificate number",
              visibleWhenEquals: { field: "hasCertificate", value: "yes" },
              validation: { required: "Enter your certificate number.", minLength: 4 },
              help: SEP_FIELD_HELP.certificateNumber,
            },
            {
              name: "eventOccasion",
              label: "What's the occasion of your event?",
              type: "textarea",
              required: true,
              placeholder: "Describe the purpose or occasion of your event",
              hint: "Maximum 255 characters.",
              validation: { required: true, maxLength: 255 },
              help: SEP_FIELD_HELP.eventOccasion,
            },
            {
              name: "hasAdmissionCharge",
              label: "Are you charging an event admission price?",
              type: "radio",
              required: true,
              options: yesNo,
              help: SEP_FIELD_HELP.hasAdmissionCharge,
            },
          ],
        },
      ],
    },
    {
      id: "location",
      title: "Location",
      sections: [
        {
          title: "Event information",
          fields: [
            {
              name: "eventCity",
              label: "Special event city",
              type: "text",
              required: true,
              placeholder: "Start typing to search cities...",
              hint: "All locations must be in the same city, district or Indigenous Nation.",
              validation: { required: true },
              help: SEP_FIELD_HELP.eventCity,
            },
            {
              name: "isAnnualEvent",
              label: "Is this an annual event?",
              type: "radio",
              required: true,
              options: yesNo,
            },
            {
              name: "locationName",
              label: "Event Location Name",
              type: "text",
              required: true,
              placeholder: "e.g., Community Centre, Hotel Ballroom",
              validation: { required: true },
              help: SEP_FIELD_HELP.locationName,
            },
            {
              name: "venueType",
              label: "Venue Type",
              type: "text",
              required: true,
              placeholder: "e.g., community centre, convention centre, banquet hall",
              validation: { required: true },
              help: SEP_FIELD_HELP.venueType,
            },
            {
              name: "addressLine1",
              label: "Address Line 1",
              type: "text",
              required: true,
              placeholder: "Street address",
              validation: { required: true },
              help: SEP_FIELD_HELP.addressLine1,
            },
            {
              name: "postalCode",
              label: "Postal Code",
              type: "text",
              required: true,
              placeholder: "V0V 0V0",
              validation: { required: true, postalCode: true },
              help: SEP_FIELD_HELP.postalCode,
            },
            {
              name: "maxGuests",
              label: "Maximum number of guests at this location at any time",
              type: "number",
              required: true,
              hint: "Events expecting 500 or more guests are routed to police for review.",
              validation: { required: true, min: 1 },
              help: SEP_FIELD_HELP.maxGuests,
            },
            {
              name: "eventDate",
              label: "Date of your event",
              type: "date",
              required: true,
              hint: "Indoor service: 9 a.m.–2 a.m. Outdoor service: 9 a.m.–10 p.m.",
              validation: { required: true, notPast: true },
              help: SEP_FIELD_HELP.eventDate,
            },
            {
              name: "eventStartTime",
              label: "Event starts",
              type: "time",
              required: true,
              validation: { required: true },
              help: SEP_FIELD_HELP.eventStartTime,
            },
            {
              name: "eventEndTime",
              label: "Event ends",
              type: "time",
              required: true,
              validation: { required: true },
              help: SEP_FIELD_HELP.eventEndTime,
            },
          ],
        },
      ],
    },
    {
      id: "liquor",
      title: "Liquor",
      sections: [
        {
          title: "Liquor details",
          fields: [
            {
              name: "liquorTypes",
              label: "What types of liquor will you serve?",
              type: "checkbox-group",
              required: true,
              options: [
                { value: "beer", label: "Beer" },
                { value: "wine", label: "Wine" },
                { value: "cider", label: "Cider" },
                { value: "coolers", label: "Coolers" },
                { value: "spirits", label: "Spirits (private events only)" },
              ],
              hint: "Public special events may only serve beer, cider, coolers and wine. Spirits are permitted at private events only.",
              validation: { required: "Select at least one type of liquor." },
              help: SEP_FIELD_HELP.liquorTypes,
            },
            {
              name: "liquorSaleType",
              label: "Will you be selling liquor or providing it free of charge?",
              type: "radio",
              required: true,
              options: [
                { value: "selling", label: "Selling" },
                { value: "free", label: "Free of charge" },
              ],
              help: SEP_FIELD_HELP.liquorSaleType,
            },
            {
              name: "estimatedDrinks",
              label: "Estimated total number of standard drinks to be served",
              type: "number",
              required: true,
              hint: "Our Servings Calculator in the Resources section can help you estimate this.",
              validation: { required: true, min: 1 },
              help: SEP_FIELD_HELP.estimatedDrinks,
            },
            {
              name: "hasSecurity",
              label: "Will you have security staff at your event?",
              type: "radio",
              required: true,
              options: yesNo,
              help: SEP_FIELD_HELP.hasSecurity,
            },
            {
              name: "securityStaffCount",
              label: "Number of security staff",
              type: "number",
              required: true,
              visibleWhenEquals: { field: "hasSecurity", value: "yes" },
              validation: { required: "Enter the number of security staff.", min: 1 },
              help: SEP_FIELD_HELP.securityStaffCount,
            },
          ],
        },
      ],
    },
    // Fields intentionally omitted - see note 2 in this config's header comment.
    { id: "summary", title: "Summary", sections: [] },
  ],
}
