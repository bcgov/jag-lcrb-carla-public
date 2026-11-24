import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, describe, expect, it } from 'vitest';
import { DraftSummaryDialog, type DraftSummary } from './draft-summary-dialog';

const DRAFT: DraftSummary = {
  id: 'FP-126117',
  type: 'Food Primary Licence Application',
  reference: 'FP - Test',
  lastModified: '2026-05-20',
  completionPercent: 45,
  sections: [
    { name: 'Business', status: 'complete' },
    { name: 'Service area', status: 'in-progress' },
    { name: 'Documents', status: 'not-started' },
  ],
  captured: [{ label: 'Legal name', value: 'Acme Ltd.' }],
};

@Component({
  imports: [DraftSummaryDialog],
  template: `<draft-summary-dialog [draft]="draft()" [open]="open()" (openChange)="changes.push($event); open.set($event)" [resumeHref]="href()" />`,
})
class Host {
  draft = signal<DraftSummary | null>(DRAFT);
  open = signal(true);
  href = signal('/applications/new?type=liquor-primary');
  changes: boolean[] = [];
}

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  f.detectChanges();
  await f.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  f.detectChanges();
};
const content = () => document.querySelector('ui-dialog-content') as HTMLElement | null;

describe('DraftSummaryDialog', () => {
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('renders nothing without a draft', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const f = TestBed.createComponent(Host);
    f.componentInstance.draft.set(null);
    await settle(f);
    expect(content()).toBeNull();
  });

  it('shows the read-only summary: progress, sections, captured fields and the resume link', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const f = TestBed.createComponent(Host);
    await settle(f);
    const el = content()!;
    expect(el.className).toContain('max-h-[85vh] overflow-y-auto sm:max-w-lg');
    expect(el.querySelector('h2')!.className).toContain('text-balance');
    expect(el.querySelector('h2')!.textContent).toBe('Food Primary Licence Application');
    expect(el.querySelector('p[data-slot="dialog-description"]')!.textContent).toBe('Job No: FP-126117 · FP - Test');
    expect(el.textContent).toContain('This is a read-only summary of your draft. Select Resume editing to continue your application.');
    expect(el.textContent).toContain('45% complete');
    expect(el.textContent).toContain(`Last saved ${new Date('2026-05-20').toLocaleDateString()}`);
    const bar = el.querySelector('.bg-bc-blue.rounded-full') as HTMLElement;
    expect(bar.className).toBe('h-full rounded-full bg-bc-blue');
    expect(bar.style.width).toBe('45%');
    const rows = [...el.querySelectorAll('ul > li')];
    expect(rows.map((r) => r.textContent!.replace(/\s+/g, ' ').trim())).toEqual(['Business Complete', 'Service area In progress', 'Documents Not started']);
    expect(rows[0].querySelector('span + span')!.className).toBe('flex items-center gap-1.5 text-xs font-medium text-green-600');
    expect(rows[1].querySelector('span + span')!.className).toContain('text-amber-600');
    expect(rows[2].querySelector('span + span')!.className).toContain('text-muted-foreground');
    expect(el.textContent).toContain('Details entered so far');
    expect(el.querySelector('dl dt')!.textContent).toBe('Legal name');
    expect(el.querySelector('dl dd')!.textContent).toBe('Acme Ltd.');
    const link = el.querySelector('a')!;
    expect(link.textContent!.trim()).toBe('Resume editing');
    expect(link.getAttribute('href')).toBe('/applications/new?type=liquor-primary');
    expect(link.className).toContain('bg-bc-blue text-white hover:bg-bc-blue/90');
  });

  it('omits the captured section when empty and reports Close', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const f = TestBed.createComponent(Host);
    f.componentInstance.draft.set({ ...DRAFT, captured: [] });
    await settle(f);
    expect(content()!.textContent).not.toContain('Details entered so far');
    ([...content()!.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Close') as HTMLButtonElement).click();
    await settle(f);
    expect(f.componentInstance.changes).toEqual([false]);
    expect(content()).toBeNull();
  });
});
