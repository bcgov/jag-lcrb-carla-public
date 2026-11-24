import { TestBed } from '@angular/core/testing';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, onTestFinished, vi } from 'vitest';
import { listReceipts } from '@data-access/repositories/payments/receipt-model';
import { listDeliveriesForSubject } from '@data-access/repositories/notifications/notification-service';
import { ReceiptDownloadButton, ReceiptEmailStatus } from './receipt-actions';

/** No file is written: jsPDF's initialized hook swaps each new document's save() for a spy (the node build would write the PDF to disk). Returns the saved filenames. */
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

const receipt = () => ({ ...listReceipts()[0], payerEmail: 'payer@example.com' });

async function settle(fixture: { detectChanges: () => void; whenStable: () => Promise<unknown> }) {
  for (let i = 0; i < 2; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
  }
}

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});
afterEach(() => vi.unstubAllGlobals());

describe('ReceiptDownloadButton', () => {
  it('renders the default label, aria-label and brand classes, and saves the PDF on click', async () => {
    const saved = captureDownloads();
    const r = receipt();
    const fixture = TestBed.createComponent(ReceiptDownloadButton);
    fixture.componentRef.setInput('receipt', r);
    await settle(fixture);
    const btn = fixture.nativeElement.querySelector('button') as HTMLButtonElement;
    expect(btn.type).toBe('button');
    expect(btn.textContent?.trim()).toBe('Download PDF receipt');
    expect(btn.getAttribute('aria-label')).toBe(`Download PDF receipt ${r.receiptNumber}`);
    expect(btn.classList.contains('bg-[#013366]') && btn.classList.contains('hover:bg-[#38598A]') && !btn.classList.contains('bg-primary')).toBe(true);
    expect(btn.querySelector('svg')!.getAttribute('class')).toContain('mr-2 h-4 w-4');
    btn.click();
    expect(saved).toHaveLength(1);
    expect(saved[0]).toMatch(/\.pdf$/);
  });

  it('drops the brand classes for non-default variants and applies className', async () => {
    const fixture = TestBed.createComponent(ReceiptDownloadButton);
    fixture.componentRef.setInput('receipt', receipt());
    fixture.componentRef.setInput('variant', 'ghost');
    fixture.componentRef.setInput('className', 'ml-auto h-7');
    fixture.componentRef.setInput('label', 'Download');
    await settle(fixture);
    const btn = fixture.nativeElement.querySelector('button') as HTMLButtonElement;
    expect(btn.className).not.toContain('bg-[#013366]');
    expect(btn.className).toContain('ml-auto');
    expect(btn.textContent?.trim()).toBe('Download');
  });
});

describe('ReceiptEmailStatus', () => {
  async function mount() {
    const r = receipt();
    const fixture = TestBed.createComponent(ReceiptEmailStatus);
    fixture.componentRef.setInput('receipt', r);
    await settle(fixture);
    return { fixture, el: fixture.nativeElement as HTMLElement, r };
  }
  const button = (el: HTMLElement, text: string) =>
    [...el.querySelectorAll('button')].find((b) => b.textContent?.trim() === text) as HTMLButtonElement;

  it('offers to email the receipt, then shows the sent status with attachment and Resend', async () => {
    const { fixture, el, r } = await mount();
    for (const c of ['rounded-lg', 'border', 'border-border', 'bg-muted/30', 'p-4']) expect(el.firstElementChild!.classList.contains(c)).toBe(true);
    expect(el.textContent).toContain('Email a copy of this receipt');
    expect(el.textContent).toContain('payer@example.com');
    const email = button(el, 'Email receipt');
    expect(email.disabled).toBe(false);
    email.click();
    await settle(fixture);
    expect(listDeliveriesForSubject(r.receiptNumber).length).toBeGreaterThan(0);
    expect(el.textContent).toContain('Receipt emailed to payer@example.com');
    expect(el.textContent).toContain('.pdf');
    button(el, 'Resend').click();
    await settle(fixture);
    expect(el.textContent).toContain('2 sends');
  });

  it('opens the email preview dialog with the subject and a download button', async () => {
    const { fixture, el } = await mount();
    button(el, 'View email').click();
    await settle(fixture);
    await new Promise((res) => setTimeout(res, 20));
    const dialog = document.querySelector('[data-slot="dialog-content"]') as HTMLElement;
    expect(dialog).toBeTruthy();
    expect(dialog.className).toContain('max-w-2xl');
    expect(dialog.querySelector('pre')!.className).toContain('whitespace-pre-wrap');
    expect(dialog.textContent).toContain('PDF attachment');
    expect(dialog.textContent).toContain('Will be sent to payer@example.com');
  });

  it('disables sending when there is no recipient address', async () => {
    const fixture = TestBed.createComponent(ReceiptEmailStatus);
    fixture.componentRef.setInput('receipt', { ...listReceipts()[0], payerEmail: undefined });
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Sign in to email this receipt to your account address.');
    expect(button(el, 'Email receipt').disabled).toBe(true);
  });
});
