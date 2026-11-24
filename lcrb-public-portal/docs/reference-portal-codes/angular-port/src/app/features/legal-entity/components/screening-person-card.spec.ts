import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it } from 'vitest';
import type { ScreeningPerson } from '../state/screening-store';
import { ScreeningPersonCard } from './screening-person-card';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

const person = (over: Partial<ScreeningPerson>): ScreeningPerson => ({
  id: 'person-x',
  name: 'Sam Lee',
  role: 'Director',
  email: 'sam.lee@example.com',
  category: 'liquor',
  formType: 'Personal History Summary',
  status: 'not_started',
  ...over,
});

@Component({ imports: [ScreeningPersonCard], template: `<div screeningPersonCard [person]="person()" actor="Alex Rivera"></div>` })
class Host {
  person = signal(person({}));
}

async function mount(p: ScreeningPerson) {
  const fixture = TestBed.createComponent(Host);
  fixture.componentInstance.person.set(p);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return fixture.nativeElement as HTMLElement;
}

afterEach(() => (document.body.innerHTML = ''));

describe('ScreeningPersonCard', () => {
  it('shows the status badge for each state', async () => {
    const labels: [ScreeningPerson['status'], string][] = [
      ['not_started', 'Not started'],
      ['invited', 'Invitation sent'],
      ['submitted', 'Uploaded — syncing'],
      ['synced', 'Submitted to LCRB'],
    ];
    for (const [status, label] of labels) {
      const el = await mount(person({ status }));
      expect(el.querySelector('span[uiBadge]')!.textContent).toContain(label);
      document.body.innerHTML = '';
    }
  });

  it('offers a retry for a person whose sync is pending, with a read-write document manager', async () => {
    const el = await mount(person({ status: 'submitted' }));
    expect(el.textContent).toContain('Sync to LCRB pending — you can retry submission.');
    expect(el.querySelector('document-manager')).not.toBeNull();
    expect(el.textContent).toContain('Drag and drop files here');
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.includes('Submit completed form to LCRB'))).toBe(true);
  });

  it('shows a synced person read-only, with the Dynamics case', async () => {
    const el = await mount(person({ status: 'synced', dynamicsCaseId: 'LCRB-1', submittedAt: '2026-03-15T12:00:00.000Z' }));
    expect(el.textContent).toContain('Submitted to LCRB for review — case LCRB-1 on Mar 15, 2026.');
    expect(el.textContent).not.toContain('Drag and drop files here');
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.includes('Submit completed form to LCRB'))).toBe(false);
  });
});
