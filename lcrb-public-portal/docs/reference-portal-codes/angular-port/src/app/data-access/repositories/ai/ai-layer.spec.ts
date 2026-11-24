/** : the copied AI client (`lib/ai/ai-client.ts`, unchanged from the prototype) and the ported `injectAiAutoRetry`. */
import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { hashKey, readAiJsonCache, requestAiJson } from '@data-access/repositories/ai/ai-client';
import { CLIENT_RETRY_DELAY_MS } from '@domain/ai/ai-errors';
import { injectAiAutoRetry } from '@data-access/repositories/ai/use-ai-auto-retry';

/** A fetch stub whose responses resolve when the test says so. */
function deferredFetch() {
  const pending: { url: string; resolve: (body: unknown, status?: number) => void }[] = [];
  const fn = vi.fn((url: string, _init?: RequestInit) => {
    return new Promise<Response>((res) => {
      pending.push({ url, resolve: (body, status = 200) => res(Response.json(body, { status })) });
    });
  });
  vi.stubGlobal('fetch', fn);
  return { fn, pending };
}
const flush = () => new Promise((r) => setTimeout(r, 0));

beforeEach(() => sessionStorage.clear());
afterEach(() => {
  vi.unstubAllGlobals();
  vi.useRealTimers();
});

describe('requestAiJson', () => {
  it('returns a cached result without a request', async () => {
    sessionStorage.setItem('k-cached', JSON.stringify({ a: 1 }));
    const { fn } = deferredFetch();
    expect(await requestAiJson({ url: '/api/nudges', body: {}, key: 'k-cached' })).toEqual({ a: 1 });
    expect(fn).not.toHaveBeenCalled();
    expect(readAiJsonCache('k-cached')).toEqual({ a: 1 });
  });

  it('shares one in-flight request between callers with the same key, then caches it', async () => {
    const { fn, pending } = deferredFetch();
    const a = requestAiJson({ url: '/api/nudges', body: { x: 1 }, key: 'k-dedupe' });
    const b = requestAiJson({ url: '/api/nudges', body: { x: 1 }, key: 'k-dedupe' });
    await flush();
    expect(fn).toHaveBeenCalledTimes(1);
    expect(fn.mock.calls[0][1]).toMatchObject({ method: 'POST', body: '{"x":1}' });
    pending[0].resolve({ ok: true });
    expect(await a).toEqual({ ok: true });
    expect(await b).toEqual({ ok: true });
    expect(readAiJsonCache('k-dedupe')).toEqual({ ok: true });
  });

  it('does not cache when cache is false, nor cache failures', async () => {
    const { fn, pending } = deferredFetch();
    const fresh = requestAiJson({ url: '/api/x', body: 'raw', key: 'k-nocache', cache: false });
    await flush();
    expect(fn.mock.calls[0][1]).toMatchObject({ body: 'raw' });
    pending[0].resolve({ v: 1 });
    await fresh;
    expect(readAiJsonCache('k-nocache')).toBeNull();

    const failing = requestAiJson({ url: '/api/x', body: {}, key: 'k-fail' });
    await flush();
    pending[1].resolve({ error: 'x' }, 500);
    await expect(failing).rejects.toThrow('AI request to /api/x failed: 500');
    expect(readAiJsonCache('k-fail')).toBeNull();
    // The in-flight entry is cleared, so a retry issues a fresh request.
    const retry = requestAiJson({ url: '/api/x', body: {}, key: 'k-fail' });
    await flush();
    expect(fn).toHaveBeenCalledTimes(3);
    pending[2].resolve({ v: 2 });
    expect(await retry).toEqual({ v: 2 });
  });

  it('runs at most 2 requests at once and queues the rest', async () => {
    const { fn, pending } = deferredFetch();
    const calls = ['k-c1', 'k-c2', 'k-c3'].map((key) => requestAiJson({ url: '/api/x', body: {}, key }));
    await flush();
    expect(fn).toHaveBeenCalledTimes(2);
    pending[0].resolve(1);
    await flush();
    expect(fn).toHaveBeenCalledTimes(3);
    pending[1].resolve(2);
    pending[2].resolve(3);
    expect(await Promise.all(calls)).toEqual([1, 2, 3]);
  });

  it('hashKey is stable and short', () => {
    expect(hashKey('abc')).toBe(hashKey('abc'));
    expect(hashKey('abc')).not.toBe(hashKey('abd'));
    expect(hashKey('x'.repeat(1000))).toMatch(/^[0-9a-z]{1,7}$/);
  });
});

describe('injectAiAutoRetry', () => {
  async function setup() {
    const status = signal('ready');
    const error = signal<Error | undefined>(undefined);
    const busy = signal(false);
    const regenerate = vi.fn();
    const clearError = vi.fn();
    let result!: ReturnType<typeof injectAiAutoRetry>;
    @Component({ template: '' })
    class Host {
      constructor() {
        result = injectAiAutoRetry({ status, error, busy, regenerate, clearError });
      }
    }
    const fixture = TestBed.createComponent(Host);
    const render = async () => {
      fixture.detectChanges();
      await fixture.whenStable();
    };
    await render();
    return { status, error, busy, regenerate, clearError, result: () => result, render };
  }

  it('retries a transient error once after the backoff, then counts down', async () => {
    vi.useFakeTimers();
    const t = await setup();
    t.status.set('error');
    t.error.set(new Error('Rate limit exceeded, please slow down'));
    await t.render();
    expect(t.result().reconnecting()).toBe(true);
    expect(t.result().showError()).toBe(false);

    vi.advanceTimersByTime(CLIENT_RETRY_DELAY_MS - 1);
    expect(t.regenerate).not.toHaveBeenCalled();
    vi.advanceTimersByTime(1);
    expect(t.clearError).toHaveBeenCalledTimes(1);
    expect(t.regenerate).toHaveBeenCalledTimes(1);

    // The retry fails again: no more automatic attempts, show the error and a countdown.
    t.error.set(new Error('Rate limit exceeded, please slow down'));
    await t.render();
    expect(t.result().reconnecting()).toBe(false);
    expect(t.result().showError()).toBe(true);
    const wait = t.result().waitSeconds();
    expect(wait).toBeGreaterThan(0);
    vi.advanceTimersByTime(1000);
    await t.render();
    expect(t.result().waitSeconds()).toBe(wait - 1);
    expect(t.regenerate).toHaveBeenCalledTimes(1);
  });

  it('manual retry resets the budget and regenerates', async () => {
    const t = await setup();
    t.result().retry();
    expect(t.clearError).toHaveBeenCalled();
    expect(t.regenerate).toHaveBeenCalledTimes(1);
    expect(t.result().waitSeconds()).toBe(0);
  });

  it('does not retry while busy', async () => {
    vi.useFakeTimers();
    const t = await setup();
    t.busy.set(true);
    t.error.set(new Error('Rate limit exceeded'));
    await t.render();
    vi.advanceTimersByTime(60_000);
    expect(t.regenerate).not.toHaveBeenCalled();
  });
});
