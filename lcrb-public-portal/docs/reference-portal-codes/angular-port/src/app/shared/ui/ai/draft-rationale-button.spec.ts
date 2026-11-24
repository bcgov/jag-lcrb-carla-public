import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { API_FIXTURES } from '../../../../../tests/fixtures/api';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { DraftRationaleButton, type DraftRationaleInput } from './draft-rationale-button';

async function render(fixture: ComponentFixture<unknown>) {
  fixture.detectChanges();
  await fixture.whenStable();
}

async function until(fixture: ComponentFixture<unknown>, cond: () => boolean, label: string) {
  for (let i = 0; i < 100; i++) {
    await render(fixture);
    if (cond()) return;
    await new Promise((r) => setTimeout(r, 5));
  }
  throw new Error(`Timed out waiting for: ${label}`);
}

const byText = (el: HTMLElement, sel: string, text: string) =>
  Array.from(el.querySelectorAll<HTMLElement>(sel)).find((e) => e.textContent?.includes(text));

const input: DraftRationaleInput = {
  decision: 'Approve',
  tone: 'approve',
  subjectType: 'sep',
  title: 'Picnic',
  reviewerRole: 'police',
};

function make(label?: string) {
  const fixture = TestBed.createComponent(DraftRationaleButton);
  fixture.componentRef.setInput('input', input);
  if (label) fixture.componentRef.setInput('label', label);
  const drafts: string[] = [];
  fixture.componentInstance.draft.subscribe((t) => drafts.push(t));
  return { fixture, drafts };
}

describe('DraftRationaleButton', () => {
  beforeEach(() => {
    sessionStorage.clear();
    localStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the default and custom label with the prototype classes', async () => {
    stubFetch(API_FIXTURES);
    const { fixture } = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const btn = el.querySelector('button')!;
    expect(btn.textContent).toContain('Draft with AI');
    expect(btn.className).toBe(
      'inline-flex items-center gap-1.5 rounded-md border border-[#013366]/30 bg-[#013366]/5 px-2.5 py-1 text-xs font-medium text-[#013366] hover:bg-[#013366]/10 disabled:opacity-60',
    );
    expect(el.querySelector('div')!.className).toBe('flex items-center justify-between gap-2');
    const custom = make('Write it');
    await render(custom.fixture);
    expect((custom.fixture.nativeElement as HTMLElement).textContent).toContain('Write it');
  });

  it('posts the input, shows Drafting while loading, and emits the text', async () => {
    const stub = stubFetch(API_FIXTURES);
    const { fixture, drafts } = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => drafts.length === 1, 'draft');
    expect(stub.callsTo('/api/decision-rationale')[0].body).toEqual(input);
    expect(drafts[0]).toContain('The permit is approved.');
    expect(el.textContent).toContain('Draft with AI');
    expect(el.querySelector('button')!.disabled).toBe(false);
    expect(el.querySelector('.text-destructive')).toBeNull();
  });

  it('is disabled and says Drafting… while the request is pending', async () => {
    let release!: () => void;
    vi.stubGlobal(
      'fetch',
      vi.fn(() => new Promise<Response>((r) => (release = () => r(new Response('{}', { status: 500 }))))),
    );
    const { fixture } = make();
    await render(fixture);
    const el = fixture.nativeElement as HTMLElement;
    el.querySelector('button')!.click();
    await until(fixture, () => el.textContent!.includes('Drafting…'), 'loading');
    expect(el.querySelector('button')!.disabled).toBe(true);
    release();
    await until(fixture, () => el.textContent!.includes('Could not draft. Try again.'), 'error');
  });

  it('shows the error for a failed request and for an empty text, without emitting', async () => {
    stubFetch({ '/api/decision-rationale': { status: 500, json: { error: 'x' } } });
    const failed = make();
    await render(failed.fixture);
    (failed.fixture.nativeElement as HTMLElement).querySelector('button')!.click();
    await until(failed.fixture, () => (failed.fixture.nativeElement as HTMLElement).textContent!.includes('Could not draft. Try again.'), 'err1');
    expect(failed.drafts).toHaveLength(0);
    expect((failed.fixture.nativeElement as HTMLElement).querySelector('span.text-xs.text-destructive')).not.toBeNull();

    stubFetch({ '/api/decision-rationale': { json: { text: '' } } });
    const empty = make();
    await render(empty.fixture);
    (empty.fixture.nativeElement as HTMLElement).querySelector('button')!.click();
    await until(empty.fixture, () => (empty.fixture.nativeElement as HTMLElement).textContent!.includes('Could not draft. Try again.'), 'err2');
    expect(empty.drafts).toHaveLength(0);
  });
});
