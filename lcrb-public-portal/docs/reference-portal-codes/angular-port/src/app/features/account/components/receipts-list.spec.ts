import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, onTestFinished, vi } from 'vitest';
import { listReceipts } from '@data-access/repositories/payments/receipt-model';
import { ReceiptsList } from './receipts-list';

/** jsPDF's initialized hook swaps save() for a spy so no PDF is written to disk; returns the saved filenames. */
function captureDownloads(): string[] {
  const names: string[] = [];
  const hook = [
    'initialized',
    function (this: { save: (name: string) => void }) {
      this.save = (name: string) => void names.push(name);
    },
  ];
  (jsPDF.API as unknown as { events: unknown[] }).events.push(hook);
  onTestFinished(() => {
    const events = (jsPDF.API as unknown as { events: unknown[] }).events;
    events.splice(events.indexOf(hook), 1);
  });
  return names;
}

async function render(params: Record<string, string> = {}) {
  TestBed.configureTestingModule({
    providers: [
      provideRouter([]),
      {
        provide: ActivatedRoute,
        useValue: { queryParamMap: of(convertToParamMap(params)), snapshot: { queryParamMap: convertToParamMap(params) } },
      },
    ],
  });
  const fixture = TestBed.createComponent(ReceiptsList);
  const el = fixture.nativeElement as HTMLElement;
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
  }
  return { fixture, el };
}

describe('ReceiptsList', () => {
  const scroll = vi.fn();
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    scroll.mockClear();
    Element.prototype.scrollIntoView = scroll;
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders nothing on the first render, then the seeded receipts with summary and badges', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const first = TestBed.createComponent(ReceiptsList);
    expect(first.nativeElement.querySelector('ul')).toBeNull();
    TestBed.resetTestingModule();

    const { el } = await render();
    const receipts = listReceipts();
    expect(receipts.length).toBeGreaterThan(0);
    expect(el.querySelectorAll('ul > li').length).toBe(receipts.length);
    expect(el.textContent).toMatch(/\d+ receipts? · /);
    expect(el.textContent).toContain(receipts[0].receiptNumber);
    const badge = el.querySelector('[data-slot="badge"]')!;
    expect(badge.className).toContain('border-transparent');
    expect(el.querySelector('h2')!.className).toBe('text-base font-semibold text-foreground');
    expect(el.querySelector('[data-slot="card"]')!.className).not.toContain('ring-2');
  });

  it('highlights the ?receipt= card and scrolls it into view', async () => {
    const target = listReceipts()[1] ?? listReceipts()[0];
    const { el } = await render({ receipt: target.receiptNumber });
    const cards = [...el.querySelectorAll('[data-slot="card"]')].filter((c) => c.className.includes('ring-[#FCBA19]'));
    expect(cards).toHaveLength(1);
    expect(cards[0].className).toContain('ring-2 ring-[#FCBA19]');
    expect(cards[0].textContent).toContain(target.receiptNumber);
    expect(scroll).toHaveBeenCalledWith({ block: 'center', behavior: 'smooth' });
  });

  it('does not scroll without a ?receipt= param', async () => {
    await render();
    expect(scroll).not.toHaveBeenCalled();
  });

  it('downloads the PDF through jsPDF save without writing a file', async () => {
    const saved = captureDownloads();
    const { el } = await render();
    const btn = [...el.querySelectorAll('button')].find((b) => b.textContent?.trim() === 'Download PDF')!;
    expect(btn.getAttribute('aria-label')).toMatch(/^Download PDF /);
    expect(btn.classList.contains('bg-[#013366]') && btn.classList.contains('hover:bg-[#38598A]') && !btn.classList.contains('bg-primary')).toBe(true);
    btn.click();
    expect(saved).toHaveLength(1);
    expect(saved[0]).toMatch(/\.pdf$/);
  });
});
