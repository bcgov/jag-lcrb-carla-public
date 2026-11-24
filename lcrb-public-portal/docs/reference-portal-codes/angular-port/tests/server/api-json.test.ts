import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { aiStub, failingModel, jsonModel, textModel } from './support/ai-stub';
import { useServer } from './support/server';

vi.mock('ai', async (orig) => (await import('./support/ai-stub')).mockAiModule(await orig()));

const server = useServer();

afterEach(() => vi.unstubAllEnvs());

beforeEach(() => {
  aiStub.model = undefined;
  vi.spyOn(console, 'error').mockImplementation(() => undefined);
  vi.spyOn(console, 'log').mockImplementation(() => undefined);
  // document-check services must take their simulated (no network) paths.
  vi.stubEnv('BC_REGISTRIES_API_KEY', '');
  vi.stubEnv('RESPONSIBLE_SERVICE_API_KEY', '');
});

const err = (message: string, statusCode?: number) => Object.assign(new Error(message), { statusCode });

interface Spec {
  name: string;
  /** Valid minimal request body. */
  valid: unknown;
  /** Raw model output for generateText (a string means plain text). */
  output: unknown;
  /** Invalid requests: body, expected status, expected JSON body. */
  invalid: [label: string, body: unknown, status: number, json: unknown][];
  /** Fallback error text the route passes to friendlyJsonError. */
  fallback: string;
  /** Assert the 200 response body. */
  check: (json: any) => void;
}

const badJson = (status: number, json: unknown): Spec['invalid'][number] => ['non-JSON body', 'not json', status, json];
const INVALID_BODY = { error: 'Invalid request body.' };

const specs: Spec[] = [
  {
    name: 'activity-search',
    valid: {
      query: 'who approved?',
      entries: ['a', 'b'].map((id) => ({
        id,
        at: '2026-01-01',
        category: 'c',
        action: 'a',
        summary: 's',
        actor: 'x',
        subjectId: null,
      })),
    },
    output: { answer: 'Found one.', matchIds: ['b', 'ghost'], interpretation: 'approvals' },
    // No JSON validation: a non-JSON body lands in the route's catch block.
    invalid: [
      badJson(500, { error: 'Search failed.', retryable: false, kind: 'transient' }),
      ['blank query', { query: '  ', entries: [] }, 400, { error: 'Empty query' }],
    ],
    fallback: 'Search failed.',
    check: (j) =>
      expect(j).toEqual({ answer: 'Found one.', interpretation: 'approvals', matchIds: ['b'] }), // invented id dropped
  },
  {
    name: 'adverse-media',
    valid: { program: 'cannabis', applicantName: 'Acme', people: [{ name: 'Jo', roles: ['Director'] }, { name: 'JO' }] },
    output: {
      overall: { outcome: 'clear', escalates: false, summary: 'Nothing.' },
      subjects: [{ name: 'Acme', type: 'business', outcome: 'clear', matchConfidence: 'unclear', summary: 's', articles: [] }],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['bad program', { program: 'x' }, 400, { error: 'Missing or invalid program.' }],
      ['no subjects', { program: 'liquor' }, 400, { error: 'No subjects to scan.' }],
    ],
    fallback: 'Could not complete the adverse-media scan.',
    check: (j) =>
      expect(j).toMatchObject({
        program: 'cannabis',
        escalationTarget: 'the Security Programs Division (SPD)',
        // business first, case-insensitive duplicate person dropped
        scannedSubjects: [{ name: 'Acme', type: 'business' }, { name: 'Jo', type: 'person' }],
        overall: { outcome: 'clear', escalates: false },
      }),
  },
  {
    name: 'assessment',
    valid: { title: 'Liquor Primary', applicant: 'Acme', summary: [{ label: 'Name', value: 'Acme' }] },
    output: {
      confidence: 84.6,
      recommendation: '  Low   touch ',
      report: 'A  B  \n\n\n\nC',
      highlights: [' x   y ', '  '],
      flags: [],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['no title/summary', { applicant: 'x' }, 400, { error: 'Missing application data.' }],
    ],
    fallback: 'Could not generate the assessment.',
    check: (j) => {
      expect(j).toMatchObject({
        confidence: 85,
        recommendation: 'Low touch',
        report: 'A B\n\nC',
        highlights: ['x y'],
        flags: [],
        model: 'openai/gpt-5-mini',
      });
      expect(Number.isNaN(Date.parse(j.generatedAt))).toBe(false);
      expect(j).not.toHaveProperty('agentSummary');
    },
  },
  {
    name: 'decision-rationale',
    valid: { decision: 'Deny permit', tone: 'deny', subjectType: 'SEP', title: 'Fair', reviewerRole: 'police' },
    output: '  Dear applicant, the permit is denied.  ',
    // No JSON validation: a non-JSON body lands in the route's catch block.
    invalid: [
      badJson(500, { error: 'Failed to draft rationale.', retryable: false, kind: 'transient' }),
      ['no subjectType', { decision: 'd' }, 400, { error: 'Missing decision or subjectType' }],
    ],
    fallback: 'Failed to draft rationale.',
    check: (j) => expect(j).toEqual({ text: 'Dear applicant, the permit is denied.' }),
  },
  {
    name: 'interpret-answer',
    valid: { field: { label: 'Opening time', type: 'time' }, text: 'half eight in the evening' },
    output: { value: '20:30', values: [], confidence: 0.9, note: '  assumed pm ' },
    invalid: [
      badJson(400, INVALID_BODY),
      ['blank text', { field: { label: 'x', type: 'text' }, text: ' ' }, 400, { error: 'Missing field or answer text.' }],
    ],
    fallback: 'Could not read that answer.',
    check: (j) => expect(j).toEqual({ value: '20:30', confidence: 0.9, note: 'assumed pm' }),
  },
  {
    name: 'licence-actions',
    valid: {
      licenceType: 'LP',
      licenceName: 'Pub',
      status: 'Active',
      expiryDate: '2027-01-01',
      daysToExpiry: 30,
      flags: [],
      isOperated: false,
      hasScheduledInspection: false,
      availableActions: [{ slug: 'renew', title: 'Renew', description: 'd' }],
    },
    output: {
      suggestions: [
        { title: ' Renew   now ', reason: 'Soon.', urgency: 'urgent', actionSlug: ' RENEW ' },
        { title: 'B', reason: 'r', urgency: 'soon', actionSlug: 'made-up' },
        ...['C', 'D', 'E'].map((t) => ({ title: t, reason: 'r', urgency: 'routine', actionSlug: '' })),
      ],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['no status', { licenceType: 'LP', availableActions: [] }, 400, { error: 'Missing licence data.' }],
    ],
    fallback: 'Could not generate suggestions.',
    check: (j) => {
      expect(j.suggestions).toHaveLength(4); // capped
      expect(j.suggestions[0]).toEqual({ title: 'Renew now', reason: 'Soon.', urgency: 'urgent', actionSlug: 'renew' }); // snapped
      expect(j.suggestions[1].actionSlug).toBe(''); // invented slug blanked
    },
  },
  {
    name: 'reviewer-actions',
    valid: { role: 'police', itemTitle: 'Fair', itemId: 'sep-1', status: 'Unassigned', facts: [] },
    output: {
      suggestions: [
        { title: ' Assign   it ', reason: ' now ', urgency: 'urgent', opensReview: true },
        ...['B', 'C', 'D', 'E'].map((t) => ({ title: t, reason: 'r', urgency: 'routine', opensReview: false })),
      ],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['unknown role', { role: 'bad', status: 's', facts: [] }, 400, { error: 'Missing or invalid reviewer item data.' }],
    ],
    fallback: 'Could not generate suggestions.',
    check: (j) => {
      expect(j.suggestions).toHaveLength(4);
      expect(j.suggestions[0]).toEqual({ title: 'Assign it', reason: 'now', urgency: 'urgent', opensReview: true });
    },
  },
  {
    name: 'reviewer-brief',
    valid: { subjectType: 'SEP', title: 'Fair', reviewerRole: 'police', facts: [], decisionOptions: ['Approve', 'Deny'] },
    output: {
      summary: ' All   good. ',
      riskLevel: 'low',
      attentionItems: [' one  ', ' '],
      recommendedDecision: 'approve the permit', // snapped to the allowed option
      rationale: ' Clean. ',
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['empty decisionOptions', { subjectType: 'SEP', facts: [], decisionOptions: [] }, 400, { error: 'Missing brief data.' }],
    ],
    fallback: 'Could not generate the reviewer brief.',
    check: (j) =>
      expect(j).toEqual({
        summary: 'All good.',
        riskLevel: 'low',
        attentionItems: ['one'],
        recommendedDecision: 'Approve',
        rationale: 'Clean.',
      }),
  },
  {
    name: 'status-explainer',
    valid: { subjectType: 'SEP', title: 'Fair', status: 'submitted' },
    output: { meaning: ' It   is in review. ', whatsNext: ['Wait ', ' '], actionRequired: false },
    invalid: [
      badJson(400, INVALID_BODY),
      ['no status', { subjectType: 'SEP' }, 400, { error: 'Missing subject or status.' }],
    ],
    fallback: 'Could not generate an explanation.',
    check: (j) => expect(j).toEqual({ meaning: 'It is in review.', whatsNext: ['Wait'], actionRequired: false }),
  },
  {
    name: 'suggest-applications',
    valid: {
      legalName: 'Acme',
      licences: [{ type: 'LP', name: 'Pub', status: 'Active', expiryDate: '2027-01-01', daysToExpiry: -5, flags: ['Renewal Due'] }],
      availableApplications: [{ slug: 'renew', title: 'Renew', description: 'd', licenceType: 'LP', kind: 'renewal' }],
    },
    output: {
      suggestions: [
        { title: ' Renew   it ', reason: 'Expired.', urgency: 'urgent', appSlug: 'RENEW' },
        { title: 'Ghost', reason: 'r', urgency: 'soon', appSlug: 'made-up' },
        ...[1, 2, 3, 4, 5].map((n) => ({ title: `R${n}`, reason: 'r', urgency: 'routine', appSlug: 'renew' })),
      ],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['no catalog', { licences: [] }, 400, { error: 'Missing business or catalog data.' }],
    ],
    fallback: 'Could not generate suggestions.',
    check: (j) => {
      // invented slug dropped, then capped at 4
      expect(j.suggestions).toHaveLength(4);
      expect(j.suggestions[0]).toEqual({ title: 'Renew it', reason: 'Expired.', urgency: 'urgent', appSlug: 'renew' });
      expect(j.suggestions.every((s: any) => s.appSlug === 'renew')).toBe(true);
    },
  },
  {
    name: 'workload-suggest',
    valid: { scope: 'police', stats: [{ label: 'Unassigned', value: '3' }], items: [], links: [{ label: 'Queue', href: '/q' }] },
    output: {
      suggestions: [
        { title: ' Triage ', reason: ' now ', urgency: 'urgent', linkLabel: 'Queue' },
        { title: 'B', reason: 'r', urgency: 'soon', linkLabel: 'Invented' },
        { title: 'C', reason: 'r', urgency: 'routine', linkLabel: null },
      ],
    },
    invalid: [
      badJson(400, INVALID_BODY),
      ['bad scope', { scope: 'bogus' }, 400, { error: 'Missing or invalid workload scope.' }],
    ],
    fallback: 'Could not generate suggestions.',
    check: (j) =>
      expect(j.suggestions).toEqual([
        { title: 'Triage', reason: 'now', urgency: 'urgent', linkLabel: 'Queue' },
        { title: 'B', reason: 'r', urgency: 'soon' }, // invented label dropped
        { title: 'C', reason: 'r', urgency: 'routine' },
      ]),
  },
];

// Document-check has its own bodies per document type; see the dedicated block below.
const modelFor = (output: unknown) => (typeof output === 'string' ? textModel(output) : jsonModel(output));

describe.each(specs)('POST /api/$name', (spec) => {
  const path = `/api/${spec.name}`;

  it.each(spec.invalid)('invalid input (%s) returns the route error', async (_label, body, status, json) => {
    aiStub.model = undefined; // must not reach the model
    const res = await server.post(path, body);
    expect(res.status).toBe(status);
    expect(await res.json()).toEqual(json);
  });

  it('returns 200 with the post-processed model output', async () => {
    aiStub.model = modelFor(spec.output);
    const res = await server.post(path, spec.valid);
    expect(res.status).toBe(200);
    spec.check(await res.json());
  });

  it('maps a model failure to the friendly 500 error', async () => {
    aiStub.model = failingModel(new Error('boom'));
    const res = await server.post(path, spec.valid);
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: spec.fallback, retryable: false, kind: 'transient' });
  });

  it('maps a rate-limit failure to 429 and a quota failure to 503', async () => {
    aiStub.model = failingModel(err('Too many requests', 429));
    const limited = await server.post(path, spec.valid);
    expect(limited.status).toBe(429);
    expect(await limited.json()).toMatchObject({ retryable: true, kind: 'transient' });

    aiStub.model = failingModel(err('Free tier requests are rate-limited. Upgrade to paid credits.'));
    const quota = await server.post(path, spec.valid);
    expect(quota.status).toBe(503);
    expect(await quota.json()).toMatchObject({ retryable: false, kind: 'quota' });
  });
});

describe('POST /api/assessment (agent pre-screen mode)', () => {
  it('adds a trimmed agentSummary and cleaned narrativeChecks when preScreen is sent', async () => {
    aiStub.model = jsonModel({
      confidence: 70,
      recommendation: 'r',
      report: 'x',
      highlights: [],
      flags: [],
      agentSummary: ' Looks   fine. ',
      narrativeChecks: [
        { kind: 'consistency', label: ' Internal  consistency ', outcome: 'clear', confidence: 90.4, finding: ' ok  ', observations: [' a  ', ' '] },
      ],
    });
    const res = await server.post('/api/assessment', {
      title: 'T',
      applicant: 'A',
      summary: [],
      preScreen: { involvesCannabis: false },
    });
    expect(res.status).toBe(200);
    const json = await res.json();
    expect(json.agentSummary).toBe('Looks fine.');
    expect(json.narrativeChecks).toEqual([
      { kind: 'consistency', label: 'Internal consistency', outcome: 'clear', confidence: 90, finding: 'ok', observations: ['a'] },
    ]);
  });
});

describe('POST /api/adverse-media (liquor)', () => {
  it('uses the enhanced suitability review as escalation target', async () => {
    aiStub.model = jsonModel({ overall: { outcome: 'clear', escalates: false, summary: 's' }, subjects: [] });
    const res = await server.post('/api/adverse-media', { program: 'liquor', applicantName: 'Acme' });
    expect(res.status).toBe(200);
    expect(await res.json()).toMatchObject({
      escalationTarget: 'an enhanced suitability review',
      scannedSubjects: [{ name: 'Acme', type: 'business' }],
    });
  });
});

describe('POST /api/interpret-answer (coercion)', () => {
  const ask = async (field: unknown, output: unknown) => {
    aiStub.model = jsonModel(output);
    const res = await server.post('/api/interpret-answer', { field, text: 'whatever' });
    expect(res.status).toBe(200);
    return res.json();
  };
  const answer = (o: object) => ({ value: '', values: [], confidence: 0.8, note: '', ...o });

  it('file fields short-circuit without calling the model', async () => {
    const res = await server.post('/api/interpret-answer', { field: { label: 'Doc', type: 'file' }, text: 'hi' });
    expect(await res.json()).toEqual({ value: null, confidence: 0, note: 'Please upload the document.' });
  });

  it('rejects an unusable time value as value null, confidence 0', async () => {
    expect(await ask({ label: 'T', type: 'time' }, answer({ value: 'evening' }))).toEqual({ value: null, confidence: 0, note: '' });
  });

  it('snaps a select value to the real option by label', async () => {
    const field = { label: 'S', type: 'select', options: [{ value: 'y', label: 'Yes' }] };
    expect(await ask(field, answer({ value: 'yes' }))).toMatchObject({ value: 'y', confidence: 0.8 });
  });

  it('de-duplicates checkbox-group values and drops unknown options', async () => {
    const field = { label: 'D', type: 'checkbox-group', options: [{ value: 'mon', label: 'Monday' }] };
    expect(await ask(field, answer({ values: ['mon', 'mon', 'xx'] }))).toMatchObject({ value: ['mon'] });
  });
});

describe('POST /api/nudges', () => {
  const body = { items: [{ kind: 'renewal', label: 'Pub', detail: 'Due', href: '/licences' }], today: '2026-01-01' };

  it.each([
    ['empty items', { items: [], today: 'x' }],
    ['missing items', {}],
    ['non-JSON body (route has no validation; its catch returns empty)', 'not json'],
  ])('returns 200 { nudges: [] } for %s without calling the model', async (_n, b) => {
    const res = await server.post('/api/nudges', b);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ nudges: [] });
  });

  it('returns the model output unchanged on success', async () => {
    const out = {
      nudges: [{ id: 'renew-1', priority: 'high', title: 'Renew', body: 'Do it.', ctaLabel: 'Renew now', ctaHref: '/licences' }],
    };
    aiStub.model = jsonModel(out);
    const res = await server.post('/api/nudges', body);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual(out);
  });

  it('returns 200 { nudges: [] } when the model fails', async () => {
    aiStub.model = failingModel(new Error('boom'));
    const res = await server.post('/api/nudges', body);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ nudges: [] });
  });
});

describe('POST /api/document-check', () => {
  const empty = { occupancy: null, corporate: null, lease: null, lgApproval: null, certificate: null };
  const base = { status: 'complete', summary: 'Looks fine.', documentType: 'doc', present: ['a'], missing: [], ...empty };
  const req = { fileData: 'data:application/pdf;base64,QQ==', filename: 'f.pdf', mediaType: 'application/pdf', label: 'Floor plan', criteria: ['Scale'] };
  const run = async (body: object, output: object) => {
    aiStub.model = jsonModel({ ...base, ...output });
    const res = await server.post('/api/document-check', { ...req, ...body });
    expect(res.status).toBe(200);
    return res.json();
  };

  it.each([
    ['non-JSON body', 'not json', INVALID_BODY],
    ['no fileData', { label: 'x' }, { error: 'Missing file or document label.' }],
    ['no label', { fileData: 'x' }, { error: 'Missing file or document label.' }],
  ])('invalid input (%s) returns 400', async (_n, body, json) => {
    const res = await server.post('/api/document-check', body);
    expect(res.status).toBe(400);
    expect(await res.json()).toEqual(json);
  });

  it('generic document: returns the assessment with no extraction blocks', async () => {
    const json = await run({}, {});
    expect(json).toEqual({ status: 'complete', summary: 'Looks fine.', documentType: 'doc', present: ['a'], missing: [], advisories: [] });
  });

  it('floor plan: sums spaces and derives advisories', async () => {
    const json = await run(
      { docType: 'floor-plan', context: { declaredArea: 200, currentApprovedCapacity: 50 } },
      {
        occupancy: {
          stampFound: true,
          spaces: [{ label: 'Main', load: 40 }, { label: 'Patio', load: 20 }],
          totalLoad: null,
          stampDate: '2020-01-01',
          floorAreaValue: 100,
          floorAreaUnit: 'sqft',
          scaleNoted: false,
        },
      },
    );
    expect(json.occupancy).toMatchObject({ stampFound: true, totalLoad: 60, stampExpired: true, floorArea: { value: 100, unit: 'sqft' } });
    expect(json.advisories).toEqual([
      'Occupancy load read from the stamp: 60 persons.',
      expect.stringContaining('older than 12 months'),
      expect.stringContaining('does not state a scale'),
      expect.stringContaining('differs from the declared area'),
      expect.stringContaining('HIGHER than the currently approved capacity (50)'),
    ]);
  });

  it('lease: matches address tolerantly and flags an expired, unsigned lease', async () => {
    const json = await run(
      { docType: 'lease', context: { applicationAddress: '123 Main St, BC' } },
      {
        lease: { found: true, premisesAddress: '123 Main Street', landlord: 'L', tenant: 'T', termStart: '1990-01-01', termEnd: '2000-01-01', signed: false },
      },
    );
    expect(json.lease).toMatchObject({ addressMatchesApplication: true, termExpired: true, signed: false });
    expect(json.advisories).toEqual(
      expect.arrayContaining([
        'Lease premises address matches the application address.',
        expect.stringContaining('(expired)'),
        'The lease does not appear to be signed by both parties.',
      ]),
    );
  });

  it('incorporation: uses the simulated BC Registries lookup (no network)', async () => {
    const json = await run(
      { docType: 'incorporation' },
      { corporate: { found: true, companyName: 'Acme Ltd', incorporationNumber: 'BC1234567', directors: ['Jane Doe'], companyStatus: 'Active' } },
    );
    expect(json.corporate).toMatchObject({ found: true, companyName: 'Acme Ltd', directors: ['Jane Doe'] });
    expect(json.corporate.registry.simulated).toBe(true);
    expect(json.advisories.length).toBeGreaterThan(0);
  });

  it('certificate: uses the simulated certificate registry (no network)', async () => {
    const json = await run(
      { docType: 'certificate' },
      {
        certificate: { found: true, program: 'serving-it-right', certificateNumber: 'SIR-1', holderName: 'Jane Doe', issueDate: '2024-01-01', expiryDate: '2030-01-01' },
      },
    );
    expect(json.certificate.simulated).toBe(true);
    expect(json.advisories).toContain(json.certificate.summary);
  });

  it('maps a model failure to the friendly 500 error and rate limit to 429', async () => {
    aiStub.model = failingModel(new Error('boom'));
    const res = await server.post('/api/document-check', req);
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: 'Could not analyze the document. You can still continue.', retryable: false, kind: 'transient' });
    aiStub.model = failingModel(err('Too many requests', 429));
    expect((await server.post('/api/document-check', req)).status).toBe(429);
  });
});
