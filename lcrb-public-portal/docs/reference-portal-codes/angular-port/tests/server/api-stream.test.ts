import { beforeEach, describe, expect, it, vi } from 'vitest';
import { EASTER_EGG_REPLY } from '@server-lib/ai/easter-egg';
import { aiStub, failingModel, streamModel } from './support/ai-stub';
import { useServer } from './support/server';

vi.mock('ai', async (orig) => (await import('./support/ai-stub')).mockAiModule(await orig()));

const server = useServer();

beforeEach(() => {
  aiStub.model = undefined;
  vi.spyOn(console, 'error').mockImplementation(() => undefined);
  vi.spyOn(console, 'log').mockImplementation(() => undefined);
});

const GENERIC = 'Sorry — I ran into a problem reaching the assistant. Please try again in a moment.';
const TRANSIENT =
  "The assistant is briefly unavailable because it's handling too many requests at once. Please wait a few seconds and try again.";
const QUOTA_PREFIX = 'The AI features have reached their usage limit';

const userMsg = (text: string) => ({ id: 'm1', role: 'user', parts: [{ type: 'text', text }] });
const err = (message: string, statusCode?: number) => Object.assign(new Error(message), { statusCode });

interface Chunk {
  type: string;
  [k: string]: unknown;
}

/** POST, then read the SSE body into parsed chunks plus the [DONE] marker. */
async function sse(path: string, body: unknown) {
  const res = await server.post(path, body);
  const text = await res.text();
  const lines = text.split('\n').filter((l) => l.startsWith('data: ')).map((l) => l.slice(6));
  return {
    res,
    chunks: lines.filter((l) => l !== '[DONE]').map((l) => JSON.parse(l) as Chunk),
    done: lines.at(-1) === '[DONE]',
  };
}
const types = (chunks: Chunk[]) => chunks.map((c) => c.type);

const routes = [
  { name: 'assistant', body: { messages: [userMsg('hi')], context: { role: 'applicant', pathname: '/' } } },
  { name: 'licence-finder', body: { messages: [userMsg('hi')] } },
  { name: 'planner-advisor', body: { messages: [userMsg('hi')], context: { planSummary: 'A pub' } } },
  { name: 'one-door', body: { messages: [userMsg('hi')] } },
] as const;

describe.each(routes)('POST /api/$name', ({ name, body }) => {
  const path = `/api/${name}`;

  it('streams a text turn as a UI message SSE stream', async () => {
    aiStub.model = streamModel({ text: 'Hello there' });
    const { res, chunks, done } = await sse(path, body);
    expect(res.status).toBe(200);
    expect(res.headers.get('content-type')).toContain('text/event-stream');
    expect(res.headers.get('x-vercel-ai-ui-message-stream')).toBe('v1');
    expect(types(chunks)).toEqual([
      'start', 'start-step', 'text-start', 'text-delta', 'text-end', 'finish-step', 'finish',
    ]);
    expect(chunks.find((c) => c.type === 'text-delta')).toMatchObject({ delta: 'Hello there' });
    expect(done).toBe(true);
  });

  it('streams the friendly error text when the model fails', async () => {
    aiStub.model = failingModel(new Error('boom'));
    const { res, chunks } = await sse(path, body);
    expect(res.status).toBe(200); // errors travel in-stream
    expect(chunks.find((c) => c.type === 'error')).toEqual({ type: 'error', errorText: GENERIC });
  });

  it('streams the transient and quota messages for rate-limit and quota failures', async () => {
    aiStub.model = failingModel(err('Too many requests', 429));
    const limited = await sse(path, body);
    expect(limited.chunks.find((c) => c.type === 'error')).toEqual({ type: 'error', errorText: TRANSIENT });

    aiStub.model = failingModel(err('Free tier requests are rate-limited. Upgrade to paid credits.'));
    const quota = await sse(path, body);
    expect(quota.chunks.find((c) => c.type === 'error')?.errorText).toEqual(expect.stringContaining(QUOTA_PREFIX));
  });

  it('has no body validation: a non-JSON body gives the Express 500 error, not a stream', async () => {
    const res = await server.post(path, 'not json');
    expect(res.status).toBe(500);
    expect(res.headers.get('content-type')).not.toContain('text/event-stream');
  });
});

describe('POST /api/assistant tools', () => {
  const body = { messages: [userMsg('help me')], context: { role: 'applicant', pathname: '/applications/new/x' } };

  it('server tool suggestReplies: input and output chunks, then the text step', async () => {
    aiStub.model = streamModel(
      { toolCalls: [{ name: 'suggestReplies', input: { replies: ['Yes', 'No'] } }] },
      { text: 'All done' },
    );
    const { chunks, done } = await sse('/api/assistant', body);
    expect(chunks.find((c) => c.type === 'tool-input-available')).toMatchObject({
      toolName: 'suggestReplies',
      input: { replies: ['Yes', 'No'] },
    });
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({ output: { replies: ['Yes', 'No'] } });
    expect(chunks.find((c) => c.type === 'text-delta')).toMatchObject({ delta: 'All done' });
    expect(done).toBe(true);
  });

  it('server tool suggestActions echoes its actions', async () => {
    const actions = [{ label: 'Start', href: '/applications/new/x' }];
    aiStub.model = streamModel({ toolCalls: [{ name: 'suggestActions', input: { actions } }] }, { text: 'Ok' });
    const { chunks } = await sse('/api/assistant', body);
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({ output: { actions } });
  });

  it('client tool setField: input only, no output, turn ends on tool-calls', async () => {
    aiStub.model = streamModel({ toolCalls: [{ name: 'setField', input: { name: 'legalName', value: 'Acme' } }] });
    const { chunks, done } = await sse('/api/assistant', body);
    expect(chunks.find((c) => c.type === 'tool-input-available')).toMatchObject({
      toolName: 'setField',
      input: { name: 'legalName', value: 'Acme' },
    });
    expect(types(chunks)).not.toContain('tool-output-available');
    expect(chunks.at(-1)).toMatchObject({ type: 'finish', finishReason: 'tool-calls' });
    expect(done).toBe(true);
  });

  it.each([
    ['navigateTo', { href: '/licences', label: 'Licences' }],
    ['highlightField', { name: 'legalName' }],
    ['goToStep', { step: 2 }],
  ])('client tool %s: input only, no output', async (toolName, input) => {
    aiStub.model = streamModel({ toolCalls: [{ name: toolName, input }] });
    const { chunks } = await sse('/api/assistant', body);
    expect(chunks.find((c) => c.type === 'tool-input-available')).toMatchObject({ toolName, input });
    expect(types(chunks)).not.toContain('tool-output-available');
  });

  it('easter egg: a licensee typing "vijaivir" gets the fixed reply without calling the model', async () => {
    aiStub.model = undefined; // would throw if the model were reached
    const { res, chunks, done } = await sse('/api/assistant', {
      messages: [userMsg('hi vijaivir')],
      context: { role: 'licensee', pathname: '/' },
    });
    expect(res.status).toBe(200);
    expect(types(chunks)[0]).toBe('start');
    expect(chunks.filter((c) => c.type === 'text-delta').map((c) => c.delta).join('')).toBe(EASTER_EGG_REPLY);
    expect(chunks.at(-1)).toMatchObject({ type: 'finish' });
    expect(done).toBe(true);
  });

  it('easter egg is licensee-only: an applicant reaches the model', async () => {
    aiStub.model = streamModel({ text: 'normal' });
    const { chunks } = await sse('/api/assistant', {
      messages: [userMsg('hi vijaivir')],
      context: { role: 'applicant', pathname: '/' },
    });
    expect(chunks.find((c) => c.type === 'text-delta')).toMatchObject({ delta: 'normal' });
  });

  it('defaults the context when it is omitted', async () => {
    aiStub.model = streamModel({ text: 'ok' });
    const { res } = await sse('/api/assistant', { messages: [userMsg('hi')] });
    expect(res.status).toBe(200);
  });
});

describe('server tools on the other routes', () => {
  it('licence-finder recommendApplications echoes recommendations', async () => {
    const recommendations = [
      { slug: 'food-primary', title: 'Food Primary', reason: 'Restaurant.', confidence: 'high', establishmentName: 'Acme', legalName: null },
    ];
    aiStub.model = streamModel({ toolCalls: [{ name: 'recommendApplications', input: { recommendations } }] }, { text: 'Why' });
    const { chunks } = await sse('/api/licence-finder', { messages: [userMsg('restaurant')] });
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({ output: { recommendations } });
  });

  it('one-door handoffToBranch resolves the branch destination', async () => {
    const input = { branch: 'lcrb', summary: 'You want a licence.', keywords: ['licence'] };
    aiStub.model = streamModel({ toolCalls: [{ name: 'handoffToBranch', input }] }, { text: 'Transferring' });
    const { chunks } = await sse('/api/one-door', { messages: [userMsg('licence')] });
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({
      output: { ...input, connected: true, destination: '/login?from=one-door' },
    });
  });

  it('one-door handoff to a branch with no href is not connected', async () => {
    const input = { branch: 'icbc', summary: 'You want plates.', keywords: ['plates'] };
    aiStub.model = streamModel({ toolCalls: [{ name: 'handoffToBranch', input }] }, { text: 'Transferring' });
    const { chunks } = await sse('/api/one-door', { messages: [userMsg('plates')] });
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({
      output: { branch: 'icbc', connected: false, destination: null },
    });
  });

  it('planner-advisor suggestReplies echoes replies', async () => {
    aiStub.model = streamModel({ toolCalls: [{ name: 'suggestReplies', input: { replies: ['Zoning'] } }] }, { text: 'Answer' });
    const { chunks } = await sse('/api/planner-advisor', { messages: [userMsg('plan')] });
    expect(chunks.find((c) => c.type === 'tool-output-available')).toMatchObject({ output: { replies: ['Zoning'] } });
  });
});
