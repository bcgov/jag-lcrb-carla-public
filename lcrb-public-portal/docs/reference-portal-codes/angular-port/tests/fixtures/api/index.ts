/** Canned /api responses; text is marked as fixture content so screenshots never pass for real AI output. */
import type { ApiFixture } from '../../support/api-fixtures';
import { replyWithChips, uiStream } from './stream';

const NOTE = '[Fixture]';

export const API_FIXTURES: Record<string, ApiFixture> = {
  // JSON AI routes
  '/api/adverse-media': {
    json: {
      program: 'cannabis',
      escalationTarget: 'the Security Programs Division (SPD)',
      scannedSubjects: [{ name: 'Fixture Holdings Ltd.', type: 'business' }],
      overall: { outcome: 'clear', escalates: false, summary: `${NOTE} No adverse media identified.` },
      subjects: [
        {
          name: 'Fixture Holdings Ltd.',
          type: 'business',
          outcome: 'clear',
          matchConfidence: 'confirmed',
          summary: `${NOTE} Nothing adverse found in open sources.`,
          articles: [],
        },
      ],
    },
  },
  '/api/nudges': {
    json: {
      nudges: [
        {
          id: 'fixture-renewal',
          priority: 'high',
          title: `${NOTE} Renew your licence`,
          body: 'Your Liquor Primary licence expires soon.',
          ctaLabel: 'Renew now',
          ctaHref: '/licences',
        },
      ],
    },
  },
  '/api/reviewer-brief': {
    json: {
      summary: `${NOTE} A community picnic with modest attendance and standard liquor service.`,
      riskLevel: 'low',
      attentionItems: ['Confirm the serving times end before 20:00.'],
      recommendedDecision: 'Approve permit',
      rationale: `${NOTE} No referral triggers apply and the applicant has no history of contraventions.`,
    },
  },
  '/api/reviewer-actions': {
    json: {
      suggestions: [
        { title: `${NOTE} Assign the file to yourself`, reason: 'It is unassigned and the event is soon.', urgency: 'urgent', opensReview: true },
      ],
    },
  },
  '/api/licence-actions': {
    json: {
      suggestions: [{ title: `${NOTE} Renew before expiry`, reason: 'The licence expires within 60 days.', urgency: 'soon', actionSlug: '' }],
    },
  },
  '/api/suggest-applications': {
    json: {
      suggestions: [
        { title: `${NOTE} Renew your licence`, reason: 'A licence is due for renewal.', urgency: 'urgent', appSlug: 'licence-renewal' },
      ],
    },
  },
  '/api/workload-suggest': {
    json: {
      suggestions: [{ title: `${NOTE} Triage the unassigned files`, reason: 'Several files have no reviewer.', urgency: 'urgent' }],
    },
  },
  '/api/status-explainer': {
    json: {
      meaning: `${NOTE} Your application is being reviewed.`,
      whatsNext: ['A reviewer checks your documents.', 'We contact you if anything is missing.'],
      actionRequired: false,
    },
  },
  '/api/decision-rationale': {
    json: { text: `${NOTE} The permit is approved.\n\nStandard conditions apply.` },
  },
  '/api/activity-search': {
    json: { answer: `${NOTE} No matching entries.`, interpretation: 'Searched the audit trail.', matchIds: [] },
  },
  '/api/interpret-answer': {
    json: { value: 'Restaurant XYZ', confidence: 0.9, note: '' },
  },
  '/api/document-check': {
    json: {
      status: 'complete',
      summary: `${NOTE} The document looks complete.`,
      documentType: 'floor plan',
      present: ['Scale'],
      missing: [],
      advisories: [],
    },
  },
  '/api/assessment': {
    json: {
      confidence: 85,
      recommendation: 'Low-touch - appears complete',
      report: `${NOTE} ASSESSMENT SUMMARY`,
      highlights: ['All required fields provided.'],
      flags: [],
      agentSummary: `${NOTE} Appears complete.`,
      narrativeChecks: [
        { kind: 'consistency', label: 'Internal consistency', outcome: 'clear', confidence: 90, finding: 'Consistent.', observations: [] },
      ],
      model: 'fixture',
      generatedAt: '2026-11-02T18:00:00.000Z',
    },
  },

  // Streaming AI routes
  '/api/assistant': replyWithChips(`${NOTE} I can help with that.`, ['Start an application', 'Renew my licence']),
  '/api/licence-finder': replyWithChips(`${NOTE} Tell me about your business.`, ['A restaurant', 'A bar']),
  '/api/planner-advisor': replyWithChips(`${NOTE} Here is some planning advice.`, ['Zoning', 'Timelines']),
  '/api/one-door': uiStream(`${NOTE} Thanks, I can point you to the right branch.`),

  // Non-AI routes
  '/api/upload': {
    json: {
      url: 'https://fixture.blob.local/documents/test.pdf',
      pathname: 'documents/test.pdf',
      filename: 'test.pdf',
      contentType: 'application/pdf',
      size: 1024,
    },
  },
};

/** Routes answered by the app's own server (deterministic, no secrets needed). */
export const API_PASSTHROUGH = ['/api/entity-directory'];
