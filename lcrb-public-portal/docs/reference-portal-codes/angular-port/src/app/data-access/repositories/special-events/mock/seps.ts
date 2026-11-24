import { type SepCondition, type SepRecord } from "@domain/models/special-events"
import { STANDARD_CONDITIONS, sepOccursOnDate } from "@domain/special-events/seps"

const today = new Date()
function daysFromNow(n: number): string {
  const d = new Date(today)
  d.setDate(d.getDate() + n)
  return d.toISOString().slice(0, 10)
}
function daysAgo(n: number): string {
  return daysFromNow(-n)
}

function standardConditionObjects(): SepCondition[] {
  return STANDARD_CONDITIONS.map((text, i) => ({
    id: `std-${i + 1}`,
    text,
    standard: true,
  }))
}

/** In-memory SEP records for the prototype. */
export const SEP_RECORDS: SepRecord[] = [
  {
    id: "SEP-2026-04812",
    eventName: "Harbour City Music Festival",
    eventType: "Community Festival",
    applicantName: "Dana Whitfield",
    applicantOrg: "Nanaimo Arts Collective",
    applicantPhone: "(250) 555-0142",
    applicantEmail: "dana@nanaimoarts.example.ca",
    eventStart: daysFromNow(18),
    eventEnd: daysFromNow(20),
    startTime: "12:00",
    endTime: "23:00",
    venueName: "Maffeo Sutton Park",
    address: "100 Comox Rd",
    city: "Nanaimo",
    jurisdiction: "Nanaimo RCMP",
    lat: 49.1697,
    lng: -123.9362,
    expectedAttendance: 2200,
    minorsExpected: true,
    serviceType: "Beer garden (fenced)",
    status: "submitted",
    submittedDate: daysAgo(2),
    conditions: [],
    description:
      "Three-day outdoor music festival with a fenced beer garden, local food vendors, and all-ages admission. Liquor service confined to the beer garden.",
    siteplanProvided: true,
    history: [
      { date: daysAgo(2), actor: "Dana Whitfield", action: "Application submitted" },
    ],
  },
  {
    id: "SEP-2026-04788",
    eventName: "Westshore Charity Gala",
    eventType: "Fundraiser / Gala",
    applicantName: "Marcus Lee",
    applicantOrg: "Westshore Rotary Club",
    applicantPhone: "(250) 555-0199",
    applicantEmail: "marcus@westshorerotary.example.ca",
    eventStart: daysFromNow(9),
    eventEnd: daysFromNow(9),
    startTime: "18:00",
    endTime: "23:30",
    venueName: "Westshore Conference Centre",
    address: "1767 Island Hwy",
    city: "Langford",
    jurisdiction: "West Shore RCMP",
    lat: 48.4486,
    lng: -123.5054,
    expectedAttendance: 850,
    minorsExpected: false,
    serviceType: "Full bar service",
    status: "under-review",
    submittedDate: daysAgo(6),
    assignedOfficer: "Cst. A. Romero",
    conditions: standardConditionObjects().slice(0, 3),
    description:
      "Annual black-tie fundraising gala with seated dinner and full bar service. 19+ event with ticketed entry.",
    siteplanProvided: true,
    history: [
      { date: daysAgo(6), actor: "Marcus Lee", action: "Application submitted" },
      { date: daysAgo(4), actor: "Cst. A. Romero", action: "Assigned to self" },
      { date: daysAgo(3), actor: "Cst. A. Romero", action: "Added standard conditions" },
    ],
  },
  {
    id: "SEP-2026-04791",
    eventName: "Cowichan Valley Wedding Reception",
    eventType: "Private Event",
    applicantName: "Priya Sharma",
    applicantPhone: "(250) 555-0177",
    applicantEmail: "priya.sharma@example.ca",
    eventStart: daysFromNow(25),
    eventEnd: daysFromNow(25),
    startTime: "16:00",
    endTime: "01:00",
    venueName: "Private residence & marquee",
    address: "4521 Cowichan Lake Rd",
    city: "Duncan",
    jurisdiction: "North Cowichan / Duncan RCMP",
    lat: 48.7787,
    lng: -123.7079,
    expectedAttendance: 180,
    minorsExpected: true,
    serviceType: "Host bar",
    status: "auto-approved",
    submittedDate: daysAgo(5),
    conditions: standardConditionObjects(),
    description:
      "Outdoor wedding reception under a marquee on private property. Under the 500-attendee threshold and auto-approved by LCRB.",
    siteplanProvided: false,
    history: [
      { date: daysAgo(5), actor: "Priya Sharma", action: "Application submitted" },
      {
        date: daysAgo(5),
        actor: "LCRB System",
        action: "Auto-approved",
        note: "Attendance under 500 — auto-approved per policy. Police notified for awareness only.",
      },
    ],
  },
  {
    id: "SEP-2026-04760",
    eventName: "Downtown Street Party",
    eventType: "Community Festival",
    applicantName: "Tyler Brooks",
    applicantOrg: "Downtown Business Association",
    applicantPhone: "(250) 555-0123",
    applicantEmail: "tyler@downtownba.example.ca",
    eventStart: daysFromNow(40),
    eventEnd: daysFromNow(40),
    startTime: "14:00",
    endTime: "22:00",
    venueName: "200-block Main Street (closed)",
    address: "200 Main St",
    city: "Victoria",
    jurisdiction: "Victoria Police Department",
    lat: 48.4284,
    lng: -123.3656,
    expectedAttendance: 3500,
    minorsExpected: true,
    serviceType: "Multiple beer gardens",
    status: "changes-requested",
    submittedDate: daysAgo(8),
    assignedOfficer: "Sgt. K. Olsen",
    conditions: standardConditionObjects().slice(0, 2),
    description:
      "Large street festival with road closure and multiple licensed beer gardens. Requires a detailed security and crowd-management plan.",
    siteplanProvided: false,
    history: [
      { date: daysAgo(8), actor: "Tyler Brooks", action: "Application submitted" },
      { date: daysAgo(6), actor: "Sgt. K. Olsen", action: "Assigned to self" },
      {
        date: daysAgo(5),
        actor: "Sgt. K. Olsen",
        action: "Requested changes",
        note: "A site plan and security staffing plan are required before this can be approved. Please upload both and resubmit.",
      },
    ],
  },
  {
    id: "SEP-2026-04699",
    eventName: "Lakeside Beer & BBQ",
    eventType: "Community Festival",
    applicantName: "Erin Caldwell",
    applicantOrg: "Lakeside Lions Club",
    applicantPhone: "(250) 555-0188",
    applicantEmail: "erin@lakesidelions.example.ca",
    eventStart: daysAgo(12),
    eventEnd: daysAgo(12),
    startTime: "12:00",
    endTime: "20:00",
    venueName: "Lakeside Community Park",
    address: "55 Lakeshore Dr",
    city: "Kelowna",
    jurisdiction: "Kelowna RCMP",
    lat: 49.888,
    lng: -119.496,
    expectedAttendance: 1200,
    minorsExpected: true,
    serviceType: "Beer garden (fenced)",
    status: "approved",
    submittedDate: daysAgo(30),
    assignedOfficer: "Cpl. J. Nguyen",
    conditions: standardConditionObjects(),
    description:
      "Annual community beer and barbecue fundraiser with a fenced beer garden. Took place earlier this season.",
    siteplanProvided: true,
    history: [
      { date: daysAgo(30), actor: "Erin Caldwell", action: "Application submitted" },
      { date: daysAgo(28), actor: "Cpl. J. Nguyen", action: "Assigned to self" },
      { date: daysAgo(26), actor: "Cpl. J. Nguyen", action: "Added standard conditions" },
      {
        date: daysAgo(25),
        actor: "Cpl. J. Nguyen",
        action: "Approved",
        note: "Approved with standard conditions. Well-organized recurring event with no prior issues.",
      },
    ],
  },
  {
    id: "SEP-2026-04640",
    eventName: "Late Night Warehouse Event",
    eventType: "Private Event",
    applicantName: "Jordan Pierce",
    applicantPhone: "(250) 555-0166",
    applicantEmail: "jordan.pierce@example.ca",
    eventStart: daysAgo(20),
    eventEnd: daysAgo(20),
    startTime: "22:00",
    endTime: "03:00",
    venueName: "Industrial warehouse unit",
    address: "1200 Ledwidge Pl",
    city: "Surrey",
    jurisdiction: "Surrey RCMP",
    lat: 49.1913,
    lng: -122.8490,
    expectedAttendance: 900,
    minorsExpected: false,
    serviceType: "Full bar service",
    status: "denied",
    submittedDate: daysAgo(35),
    assignedOfficer: "Cst. M. Fontaine",
    conditions: [],
    description:
      "Late-night event in an industrial unit with full bar service running until 3am. Insufficient security plan and noise/safety concerns.",
    siteplanProvided: false,
    history: [
      { date: daysAgo(35), actor: "Jordan Pierce", action: "Application submitted" },
      { date: daysAgo(33), actor: "Cst. M. Fontaine", action: "Assigned to self" },
      {
        date: daysAgo(31),
        actor: "Cst. M. Fontaine",
        action: "Denied",
        note: "Denied due to inadequate security plan for a 900-person late-night event and unresolved venue safety concerns. Applicant may reapply with a complete security and site plan.",
      },
    ],
  },
  {
    id: "SEP-2026-04905",
    eventName: "Inner Harbour Food & Wine Fair",
    eventType: "Community Festival",
    applicantName: "Sophie Tremblay",
    applicantOrg: "Victoria Culinary Society",
    applicantPhone: "(250) 555-0210",
    applicantEmail: "sophie@victoriaculinary.example.ca",
    eventStart: daysFromNow(0),
    eventEnd: daysFromNow(0),
    startTime: "11:00",
    endTime: "20:00",
    venueName: "Ship Point, Inner Harbour",
    address: "1 Wharf St",
    city: "Victoria",
    jurisdiction: "Victoria Police Department",
    lat: 48.4235,
    lng: -123.3702,
    expectedAttendance: 1800,
    minorsExpected: true,
    serviceType: "Wine & beer garden (fenced)",
    status: "approved",
    submittedDate: daysAgo(22),
    assignedOfficer: "Cst. A. Romero",
    conditions: standardConditionObjects(),
    description:
      "All-day food and wine fair along the Inner Harbour with a fenced licensed tasting garden and local culinary vendors.",
    siteplanProvided: true,
    history: [
      { date: daysAgo(22), actor: "Sophie Tremblay", action: "Application submitted" },
      { date: daysAgo(15), actor: "Cst. A. Romero", action: "Approved", note: "Approved with standard conditions." },
    ],
  },
  {
    id: "SEP-2026-04907",
    eventName: "Cathedral Square Craft Beer Night",
    eventType: "Community Festival",
    applicantName: "Owen Fraser",
    applicantOrg: "Downtown Victoria Business Association",
    applicantPhone: "(250) 555-0222",
    applicantEmail: "owen@dvba.example.ca",
    eventStart: daysFromNow(0),
    eventEnd: daysFromNow(0),
    startTime: "17:00",
    endTime: "23:00",
    venueName: "Cathedral Square",
    address: "930 Burdett Ave",
    city: "Victoria",
    jurisdiction: "Victoria Police Department",
    lat: 48.4213,
    lng: -123.3617,
    expectedAttendance: 650,
    minorsExpected: false,
    serviceType: "Beer garden (fenced)",
    status: "approved",
    submittedDate: daysAgo(18),
    assignedOfficer: "Sgt. K. Olsen",
    conditions: standardConditionObjects().slice(0, 4),
    description:
      "Evening craft beer showcase featuring regional breweries. 19+ event with a fenced service area and live music.",
    siteplanProvided: true,
    history: [
      { date: daysAgo(18), actor: "Owen Fraser", action: "Application submitted" },
      { date: daysAgo(11), actor: "Sgt. K. Olsen", action: "Approved", note: "Approved with conditions." },
    ],
  },
  {
    id: "SEP-2026-04910",
    eventName: "Fernwood Neighbourhood Block Party",
    eventType: "Community Festival",
    applicantName: "Maya Robinson",
    applicantOrg: "Fernwood NRG",
    applicantPhone: "(250) 555-0234",
    applicantEmail: "maya@fernwoodnrg.example.ca",
    eventStart: daysFromNow(0),
    eventEnd: daysFromNow(0),
    startTime: "13:00",
    endTime: "21:00",
    venueName: "1300-block Gladstone Ave (closed)",
    address: "1300 Gladstone Ave",
    city: "Victoria",
    jurisdiction: "Victoria Police Department",
    lat: 48.4289,
    lng: -123.3486,
    expectedAttendance: 420,
    minorsExpected: true,
    serviceType: "Host bar",
    status: "auto-approved",
    submittedDate: daysAgo(9),
    conditions: standardConditionObjects(),
    description:
      "Family-friendly neighbourhood block party with a small licensed area. Under the 500-attendee threshold and auto-approved.",
    siteplanProvided: false,
    history: [
      { date: daysAgo(9), actor: "Maya Robinson", action: "Application submitted" },
      { date: daysAgo(9), actor: "LCRB System", action: "Auto-approved", note: "Attendance under 500 — auto-approved per policy." },
    ],
  },
]

export function getSepById(id: string): SepRecord | undefined {
  return SEP_RECORDS.find((s) => s.id === id)
}

/** SEPs that need police attention (not auto-approved, not historical). */
export function getActionableSeps(): SepRecord[] {
  return SEP_RECORDS.filter((s) =>
    ["submitted", "under-review", "changes-requested"].includes(s.status),
  )
}

/** SEP records active on a specific calendar day, optionally scoped to a set of (lower-cased) city names. */
export function getSepsOnDate(dateIso: string, cities?: Set<string>): SepRecord[] {
  return SEP_RECORDS.filter((s) => {
    if (s.status === "cancelled" || s.status === "denied") return false
    if (!sepOccursOnDate(s, dateIso)) return false
    if (cities && !cities.has(s.city.toLowerCase())) return false
    return true
  }).sort((a, b) => a.startTime.localeCompare(b.startTime))
}

/** Upcoming events (event start in the future, regardless of status). */
export function getUpcomingSeps(): SepRecord[] {
  const now = new Date().toISOString().slice(0, 10)
  return SEP_RECORDS.filter((s) => s.eventStart >= now).sort((a, b) =>
    a.eventStart.localeCompare(b.eventStart),
  )
}
