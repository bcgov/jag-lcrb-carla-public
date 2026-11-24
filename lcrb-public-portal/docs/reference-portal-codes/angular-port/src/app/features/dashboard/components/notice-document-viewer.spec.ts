import { TestBed } from '@angular/core/testing';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { NOTICES } from '@data-access/repositories/notifications/mock/notices';
import { NoticeDocumentViewer } from './notice-document-viewer';

const notice = NOTICES[0];
const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  f.detectChanges();
  await f.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  f.detectChanges();
};

/** jsPDF's initialized hook swaps save() for a no-op so no PDF is written. */
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => undefined;
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

describe('NoticeDocumentViewer', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    pdfEvents().push(noSave);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the trigger and the ghost download button with the prototype classes', async () => {
    const fixture = TestBed.createComponent(NoticeDocumentViewer);
    fixture.componentRef.setInput('notice', notice);
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    expect(el.firstElementChild!.className).toBe('mt-3 flex flex-wrap items-center gap-2');
    const buttons = el.querySelectorAll('button');
    expect(buttons[0].textContent!.trim()).toBe('Read full notice');
    expect(buttons[0].className).toContain('gap-1.5 bg-background');
    expect(buttons[1].textContent!.trim()).toBe('Download PDF');
    expect(document.querySelector('ui-dialog-content')).toBeNull();
  });

  it('opens the letter dialog and emits opened with the notice id', async () => {
    const fixture = TestBed.createComponent(NoticeDocumentViewer);
    fixture.componentRef.setInput('notice', notice);
    const ids: number[] = [];
    fixture.componentInstance.opened.subscribe((id) => ids.push(id));
    await settle(fixture);
    (fixture.nativeElement.querySelector('button') as HTMLButtonElement).click();
    await settle(fixture);
    const content = document.querySelector('ui-dialog-content')!;
    expect(ids).toEqual([notice.id]);
    expect(content.className).toContain('max-w-2xl');
    expect(content.querySelector('[data-slot="dialog-title"]')!.textContent).toBe(notice.title);
    expect(content.querySelector('[data-slot="dialog-description"]')!.textContent).toBe(
      `Official notice ${notice.document.documentNumber} — reference ${notice.reference}`,
    );
    expect(content.textContent).toContain('BRITISH COLUMBIA');
    expect(content.textContent).toContain(`RE: ${notice.document.subject}`);
    expect(content.textContent).toContain(notice.document.signatoryName);
  });

  it('disables the download buttons and shows the spinner while the PDF is generating', async () => {
    const errors = vi.spyOn(console, 'error').mockImplementation(() => undefined);
    const fixture = TestBed.createComponent(NoticeDocumentViewer);
    fixture.componentRef.setInput('notice', notice);
    await settle(fixture);
    const ghost = fixture.nativeElement.querySelectorAll('button')[1] as HTMLButtonElement;
    ghost.click();
    fixture.detectChanges();
    expect(ghost.disabled).toBe(true);
    expect(ghost.querySelector('svg.animate-spin')).not.toBeNull();
    for (let i = 0; i < 100 && ghost.disabled; i++) {
      await new Promise((r) => setTimeout(r, 20));
      fixture.detectChanges();
    }
    expect(ghost.disabled).toBe(false);
    expect(ghost.querySelector('svg.animate-spin')).toBeNull();
    errors.mockRestore();
  });
});
