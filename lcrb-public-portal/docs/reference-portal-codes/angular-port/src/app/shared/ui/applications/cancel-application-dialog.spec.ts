import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it } from 'vitest';
import { Button } from '../primitives/button';
import { CancelApplicationDialog, CancelApplicationTrigger } from './cancel-application-dialog';

@Component({
  imports: [CancelApplicationDialog],
  template: `<cancel-application-dialog
    subject="Liquor Primary Licence (LP-1)"
    [noun]="noun()"
    [feePaid]="feePaid()"
    [consequence]="consequence()"
    (confirmed)="reasons.push($event)"
  />`,
})
class Uncontrolled {
  noun = signal<'application' | 'permit'>('application');
  feePaid = signal(true);
  consequence = signal<string | undefined>(undefined);
  reasons: string[] = [];
}

@Component({
  imports: [CancelApplicationDialog],
  template: `<cancel-application-dialog
    subject="Event (SEP-1)"
    noun="permit"
    [open]="open()"
    (openChange)="changes.push($event); open.set($event)"
    (confirmed)="reasons.push($event)"
  />`,
})
class Controlled {
  open = signal(true);
  changes: boolean[] = [];
  reasons: string[] = [];
}

@Component({
  imports: [CancelApplicationDialog, CancelApplicationTrigger, Button],
  template: `<cancel-application-dialog subject="Event (SEP-2)" noun="permit"
    ><button cancelTrigger uiButton variant="outline" id="custom">Cancel permit</button></cancel-application-dialog
  >`,
})
class CustomTrigger {}

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  f.detectChanges();
  await f.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  f.detectChanges();
};
const content = () => document.querySelector('ui-dialog-content') as HTMLElement | null;
const btn = (text: string) => [...document.querySelectorAll('button')].find((b) => b.textContent!.trim() === text) as HTMLButtonElement;
const radio = (label: string) =>
  [...document.querySelectorAll('input[type="radio"]')].find((r) => (r as HTMLInputElement).value === label) as HTMLInputElement;
const checkbox = () => document.querySelector('[role="checkbox"]') as HTMLButtonElement;
const pickRadio = async (f: Parameters<typeof settle>[0], label: string) => {
  const r = radio(label);
  r.checked = true;
  r.dispatchEvent(new Event('change'));
  await settle(f);
};

describe('CancelApplicationDialog', () => {
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('renders the default outlined destructive trigger and opens the dialog', async () => {
    const f = TestBed.createComponent(Uncontrolled);
    await settle(f);
    const trigger = (f.nativeElement as HTMLElement).querySelector('button')!;
    expect(trigger.textContent!.trim()).toBe('Cancel application');
    for (const c of ['gap-2', 'border-red-300', 'text-red-700', 'hover:bg-red-50', 'hover:text-red-800']) expect(trigger.classList.contains(c)).toBe(true);
    expect(content()).toBeNull();
    trigger.click();
    await settle(f);
    const el = content()!;
    expect(el.className).toContain('sm:max-w-lg');
    expect(el.querySelector('h2')!.textContent).toBe('Cancel this application?');
    expect(el.textContent).toContain('You are withdrawing Liquor Primary Licence (LP-1). This cannot be undone from the portal; you would need to start a new application.');
    expect(el.textContent).toContain('Application fees already paid are not refunded.');
    expect(el.textContent).toContain('LCRB and any partner agency reviewing the file will be notified immediately.');
    expect(el.querySelector('.border-amber-200')!.className).toBe('flex items-start gap-3 rounded-md border border-amber-200 bg-amber-50 p-3 text-sm text-amber-900');
    expect(el.querySelectorAll('input[type="radio"]').length).toBe(4);
    expect(el.textContent).toContain('Additional details (optional)');
    expect(([...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Cancel application') as HTMLButtonElement).disabled).toBe(true);
  });

  it('enables confirm only with a reason and the acknowledgement, then confirms and resets', async () => {
    const f = TestBed.createComponent(Uncontrolled);
    await settle(f);
    (f.nativeElement as HTMLElement).querySelector('button')!.click();
    await settle(f);
    const confirm = () => [...document.querySelectorAll('ui-dialog-content button')].find((b) => b.textContent!.trim() === 'Cancel application') as HTMLButtonElement;
    await pickRadio(f, 'Will reapply with different details');
    expect(radio('Will reapply with different details').parentElement!.className).toBe('flex cursor-pointer items-center gap-2 rounded-md border px-3 py-2 text-sm transition-colors border-bc-blue bg-bc-blue/5');
    expect(confirm().disabled).toBe(true);
    checkbox().click();
    await settle(f);
    expect(checkbox().getAttribute('aria-checked')).toBe('true');
    expect(confirm().disabled).toBe(false);
    const ta = document.querySelector('textarea') as HTMLTextAreaElement;
    ta.value = '  new plan  ';
    ta.dispatchEvent(new Event('input'));
    await settle(f);
    confirm().click();
    await settle(f);
    expect(f.componentInstance.reasons).toEqual(['Will reapply with different details — new plan']);
    expect(content()).toBeNull();

    (f.nativeElement as HTMLElement).querySelector('button')!.click();
    await settle(f);
    expect(radio('Will reapply with different details').checked).toBe(false);
    expect(checkbox().getAttribute('aria-checked')).toBe('false');
  });

  it('requires a description for "Other" and sends it as the reason', async () => {
    const f = TestBed.createComponent(Uncontrolled);
    await settle(f);
    (f.nativeElement as HTMLElement).querySelector('button')!.click();
    await settle(f);
    await pickRadio(f, 'Other');
    expect(document.querySelector('ui-dialog-content')!.textContent).toContain('Please describe (required)');
    checkbox().click();
    await settle(f);
    const confirm = () => [...document.querySelectorAll('ui-dialog-content button')].find((b) => b.textContent!.trim() === 'Cancel application') as HTMLButtonElement;
    expect(confirm().disabled).toBe(true);
    const ta = document.querySelector('textarea') as HTMLTextAreaElement;
    ta.value = ' closing the venue ';
    ta.dispatchEvent(new Event('input'));
    await settle(f);
    expect(confirm().disabled).toBe(false);
    confirm().click();
    await settle(f);
    expect(f.componentInstance.reasons).toEqual(['closing the venue']);
  });

  it('uses permit copy, hides the fee lines when no fee was paid and shows the consequence', async () => {
    const f = TestBed.createComponent(Uncontrolled);
    f.componentInstance.noun.set('permit');
    f.componentInstance.feePaid.set(false);
    f.componentInstance.consequence.set('The city has started reviewing.');
    await settle(f);
    expect((f.nativeElement as HTMLElement).querySelector('button')!.textContent!.trim()).toBe('Cancel permit');
    (f.nativeElement as HTMLElement).querySelector('button')!.click();
    await settle(f);
    const el = content()!;
    expect(el.textContent).not.toContain('Application fees already paid are not refunded.');
    expect(el.textContent).toContain('The city has started reviewing.');
    expect(el.textContent).toContain('I understand this withdrawal is final.');
    expect(btn('Keep permit')).toBeTruthy();
  });

  it('is driven by `open` when controlled, has no trigger, and reports dismissal', async () => {
    const f = TestBed.createComponent(Controlled);
    await settle(f);
    expect((f.nativeElement as HTMLElement).querySelector('button')).toBeNull();
    expect(content()).not.toBeNull();
    btn('Keep permit').click();
    await settle(f);
    expect(f.componentInstance.changes).toEqual([false]);
    expect(content()).toBeNull();
  });

  it('opens from a projected custom trigger', async () => {
    const f = TestBed.createComponent(CustomTrigger);
    await settle(f);
    const el = f.nativeElement as HTMLElement;
    expect(el.querySelectorAll('button').length).toBe(1);
    (el.querySelector('#custom') as HTMLButtonElement).click();
    await settle(f);
    expect(content()!.querySelector('h2')!.textContent).toBe('Cancel this permit?');
  });
});
