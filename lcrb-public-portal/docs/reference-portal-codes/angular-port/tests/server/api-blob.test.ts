import { get, list, put } from '@vercel/blob';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { searchDirectory } from '@server-lib/data/entity-directory';
import { useServer } from './support/server';

vi.mock('@vercel/blob', () => ({ put: vi.fn(), get: vi.fn(), list: vi.fn() }));

const server = useServer();
const putMock = vi.mocked(put);
const getMock = vi.mocked(get);
const listMock = vi.mocked(list);

beforeEach(() => {
  vi.resetAllMocks();
  vi.spyOn(console, 'error').mockImplementation(() => undefined);
});

describe('GET /api/entity-directory', () => {
  it('returns searchDirectory results for the query, scope and source, after >= 250 ms', async () => {
    const expected = searchDirectory({ query: 'a', scope: 'business', source: 'bc-registry' });
    const started = Date.now();
    const res = await fetch(`${server.base()}/api/entity-directory?q=a&scope=business&source=bc-registry`);
    const elapsed = Date.now() - started;
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ results: JSON.parse(JSON.stringify(expected)) });
    expect(expected.length).toBeGreaterThan(0);
    expect(elapsed).toBeGreaterThanOrEqual(240);
  });

  it('defaults to q="", scope=all, source=all when no params are given', async () => {
    const expected = searchDirectory({ query: '', scope: 'all', source: 'all' });
    const res = await fetch(`${server.base()}/api/entity-directory`);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ results: JSON.parse(JSON.stringify(expected)) });
  });
});

describe('POST /api/upload', () => {
  const send = (form: FormData) => fetch(`${server.base()}/api/upload`, { method: 'POST', body: form });
  const form = (subjectId?: string) => {
    const f = new FormData();
    f.set('file', new Blob(['hello'], { type: 'text/plain' }), 'plan.txt');
    if (subjectId !== undefined) f.set('subjectId', subjectId);
    return f;
  };

  it('stores under documents/<subjectId>/<filename> and returns the blob details', async () => {
    putMock.mockResolvedValue({ url: 'https://blob/x', pathname: 'documents/s1/plan-abc.txt' } as never);
    const res = await send(form('s1'));
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({
      url: 'https://blob/x',
      pathname: 'documents/s1/plan-abc.txt',
      filename: 'plan.txt',
      contentType: 'text/plain',
      size: 5,
    });
    expect(putMock).toHaveBeenCalledTimes(1);
    const [path, , opts] = putMock.mock.calls[0];
    expect(path).toBe('documents/s1/plan.txt');
    expect(opts).toEqual({ access: 'private', addRandomSuffix: true });
  });

  it.each([[undefined], ['']])('falls back to "general" when subjectId is %j', async (subjectId) => {
    putMock.mockResolvedValue({ url: 'u', pathname: 'p' } as never);
    expect((await send(form(subjectId))).status).toBe(200);
    expect(putMock.mock.calls[0][0]).toBe('documents/general/plan.txt');
  });

  it('returns 400 when no file is provided', async () => {
    const f = new FormData();
    f.set('subjectId', 's1');
    const res = await send(f);
    expect(res.status).toBe(400);
    expect(await res.json()).toEqual({ error: 'No file provided' });
    expect(putMock).not.toHaveBeenCalled();
  });

  it('returns 500 when put throws', async () => {
    putMock.mockRejectedValue(new Error('blob down'));
    const res = await send(form());
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: 'Upload failed' });
  });

  it('returns 500 (not 400) for a non-multipart body, as is', async () => {
    const res = await server.post('/api/upload', { a: 1 });
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: 'Upload failed' });
  });
});

describe('GET /api/file', () => {
  const url = (qs = '?pathname=documents/s1/plan.txt') => `${server.base()}/api/file${qs}`;
  const blob = { etag: '"e1"', contentType: 'application/pdf' };

  it('returns 400 when pathname is missing', async () => {
    const res = await fetch(url(''));
    expect(res.status).toBe(400);
    expect(await res.json()).toEqual({ error: 'Missing pathname' });
    expect(getMock).not.toHaveBeenCalled();
  });

  it('returns 404 "Not found" when get returns null', async () => {
    getMock.mockResolvedValue(null);
    const res = await fetch(url());
    expect(res.status).toBe(404);
    expect(await res.text()).toBe('Not found');
  });

  it('passes if-none-match through and answers 304 with ETag and cache-control', async () => {
    getMock.mockResolvedValue({ statusCode: 304, stream: null, blob } as never);
    const res = await fetch(url(), { headers: { 'if-none-match': '"e1"' } });
    expect(res.status).toBe(304);
    expect(res.headers.get('etag')).toBe('"e1"');
    expect(res.headers.get('cache-control')).toBe('private, no-cache');
    expect(getMock).toHaveBeenCalledWith('documents/s1/plan.txt', { access: 'private', ifNoneMatch: '"e1"' });
  });

  it('streams the body with content-type, ETag and cache-control on 200', async () => {
    const stream = new Blob(['PDFDATA']).stream();
    getMock.mockResolvedValue({ statusCode: 200, stream, blob } as never);
    const res = await fetch(url());
    expect(res.status).toBe(200);
    expect(res.headers.get('content-type')).toBe('application/pdf');
    expect(res.headers.get('etag')).toBe('"e1"');
    expect(res.headers.get('cache-control')).toBe('private, no-cache');
    expect(await res.text()).toBe('PDFDATA');
    expect(getMock).toHaveBeenCalledWith('documents/s1/plan.txt', { access: 'private', ifNoneMatch: undefined });
  });

  it('returns 500 when get throws', async () => {
    getMock.mockRejectedValue(new Error('boom'));
    const res = await fetch(url());
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: 'Failed to serve file' });
  });
});

describe('GET /api/files', () => {
  it('lists the documents/ prefix and maps blobs to file metadata', async () => {
    listMock.mockResolvedValue({
      blobs: [
        { pathname: 'documents/s1/plan-abc.pdf', size: 10, uploadedAt: '2026-01-02T03:04:05.000Z' },
        { pathname: 'documents/', size: 0, uploadedAt: '2026-01-03T00:00:00.000Z' },
      ],
    } as never);
    const res = await fetch(`${server.base()}/api/files`);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({
      files: [
        { pathname: 'documents/s1/plan-abc.pdf', filename: 'plan-abc.pdf', size: 10, uploadedAt: '2026-01-02T03:04:05.000Z' },
        { pathname: 'documents/', filename: 'unknown', size: 0, uploadedAt: '2026-01-03T00:00:00.000Z' },
      ],
    });
    expect(listMock).toHaveBeenCalledWith({ prefix: 'documents/' });
  });

  it('returns 500 when list throws', async () => {
    listMock.mockRejectedValue(new Error('boom'));
    const res = await fetch(`${server.base()}/api/files`);
    expect(res.status).toBe(500);
    expect(await res.json()).toEqual({ error: 'Failed to list files' });
  });
});
