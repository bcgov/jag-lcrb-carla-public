import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { COMPLIANCE_HISTORY, ENFORCEMENT_ACTIONS } from '@data-access/repositories/compliance/mock/enforcement';
import { EnforcementHub } from './enforcement-hub';

@Component({ template: '' })
class Blank {}

const tick = () => new Promise((r) => setTimeout(r, 0));
async function settle(f: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  f.detectChanges();
  await f.whenStable();
  await tick();
  f.detectChanges();
}

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Matt', lastName: 'Harding', organization: 'X' } as never);
});
afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

async function mount() {
  const fixture = TestBed.createComponent(EnforcementHub);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('EnforcementHub', () => {
  it('renders the tiles, the action cards and the history table from seed data', async () => {
    const { el } = await mount();
    expect(el.textContent).toContain('At a glance');
    expect(el.textContent).toContain('Open enforcement actions');
    expect(el.querySelectorAll('div[enforcementcard]').length).toBe(ENFORCEMENT_ACTIONS.length);
    expect(el.querySelectorAll('tbody tr').length).toBe(COMPLIANCE_HISTORY.length);
    const hrefs = [...el.querySelectorAll('stat-card a')].map((a) => a.getAttribute('href'));
    expect(hrefs).toEqual([
      '/compliance#active-actions-heading',
      '/compliance#history-heading',
      '/compliance#history-heading',
      '/calendar',
    ]);
    expect(el.querySelector('div[enforcementcard]')!.className).toBe('bg-white border rounded-lg overflow-hidden');
    // A direct child of space-y-10, so it needs a real box to take the margin.
    expect((el.querySelector('workload-suggestions') as HTMLElement).style.display).toBe('block');
  });

  it('opens the respond dialog and submits a statement, moving the action out of awaiting-response', async () => {
    const { fixture, el } = await mount();
    const btn = [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Respond to this action'))!;
    btn.click();
    await settle(fixture);
    const dialog = document.querySelector('[role="dialog"]')!;
    expect(dialog.textContent).toContain('How would you like to respond?');
    const submit = [...dialog.querySelectorAll('button')].find((b) => b.textContent!.includes('Submit response'))!;
    expect(submit.disabled).toBe(true);
    const ta = dialog.querySelector('textarea')!;
    ta.value = 'This is my written response.';
    ta.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(submit.disabled).toBe(false);
    submit.click();
    await settle(fixture);
    await new Promise((r) => setTimeout(r, 600));
    await settle(fixture);
    expect(document.querySelector('[role="dialog"]')).toBeNull();
    expect(el.textContent).toContain('Response submitted');
    expect(localStorage.length).toBeGreaterThan(1);
  });

  it('shows the hearing option placeholder and accept copy when the election changes', async () => {
    const { fixture, el } = await mount();
    [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Respond to this action'))!.click();
    await settle(fixture);
    const dialog = document.querySelector('[role="dialog"]')!;
    (dialog.querySelector('#election-request-hearing') as HTMLElement).click();
    await settle(fixture);
    expect(dialog.querySelector('textarea')!.placeholder).toContain('requesting a hearing');
    (dialog.querySelector('#election-accept') as HTMLElement).click();
    await settle(fixture);
    expect(dialog.querySelector('textarea')).toBeNull();
    expect(dialog.textContent).toContain('you accept the proposed action');
    const submit = [...dialog.querySelectorAll('button')].find((b) => b.textContent!.includes('Submit response'))!;
    expect(submit.disabled).toBe(false);
    [...dialog.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Cancel')!.click();
    await settle(fixture);
    expect(document.querySelector('[role="dialog"]')).toBeNull();
  });
});
