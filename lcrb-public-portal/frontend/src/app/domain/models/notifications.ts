export type AnnouncementCategory = "policy" | "regulatory" | "release" | "service"

export interface ChangelogEntry {
  type: "added" | "improved" | "fixed" | "changed"
  text: string
}

export interface Announcement {
  id: string
  /** ISO date the announcement was published. */
  date: string
  category: AnnouncementCategory
  title: string
  /** Short summary shown in the list. */
  summary: string
  /** Full body paragraphs. */
  body: string[]
  /** Optional semantic version for Portal release notes. */
  version?: string
  /** Optional changelog for Portal release notes. */
  changelog?: ChangelogEntry[]
}

// Notices a licensee receives from the LCRB: a card summary plus the full structured letter.

export type NoticeType = "action_required" | "info" | "success"

/** A formal LCRB letter, structured so it renders identically on screen and in PDF. */
export interface NoticeDocument {
  /** Official correspondence/reference number printed on the letter. */
  documentNumber: string
  /** The "RE:" subject line. */
  subject: string
  recipientName: string
  recipientBusiness: string
  recipientAddress: string[]
  /** e.g. "Dear Ms. Harding:" */
  salutation: string
  /** Ordered body paragraphs of the letter. */
  body: string[]
  /** Closing valediction, e.g. "Sincerely,". */
  closing: string
  signatoryName: string
  signatoryTitle: string
  signatoryBranch: string
  /** Optional documents enclosed with / referenced by the letter. */
  enclosures?: string[]
  /** Optional regulatory citations shown in the letter footer. */
  references?: string[]
}

export interface Notice {
  id: number
  type: NoticeType
  title: string
  /** Short summary shown on the notice card. */
  message: string
  /** ISO date string. */
  date: string
  read: boolean
  /** Licence or application number this notice relates to. */
  reference: string
  document: NoticeDocument
}
