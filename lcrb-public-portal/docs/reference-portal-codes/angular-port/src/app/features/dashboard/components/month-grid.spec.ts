import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import type { CalendarEvent } from '@domain/models/compliance';
import { MonthGrid } from './month-grid';

@Component({ template: '' })
class Blank {}

const events: CalendarEvent[] = [
  { id: 'a', type: 'renewal', title: 'Renew A', date: '2026-03-10', description: 'd', href: '/licences/1' },
  { id: 'b', type: 'fee', title: 'Fee B', date: '2026-03-10', description: 'd', href: '/licences/2' },
  { id: 'c', type: 'hearing', title: 'Hearing C', date: '2026-03-10', description: 'd', href: '/compliance' },
  { id: 'd', type: 'inspection', title: 'Inspect D', date: '2026-05-02', description: 'd', href: '/calendar' },
];

const tick = () => new Promise((r) => setTimeout(r, 0));
async function settle(f: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  f.detectChanges();
  await f.whenStable();
  await tick();
  f.detectChanges();
}

beforeEach(() => TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] }));
afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

describe('MonthGrid', () => {
  async function mount() {
    const fixture = TestBed.createComponent(MonthGrid);
    fixture.componentRef.setInput('events', events);
    fixture.componentRef.setInput('initialMonth', new Date(2026, 2, 15));
    document.body.appendChild(fixture.nativeElement);
    await settle(fixture);
    return { fixture, el: fixture.nativeElement as HTMLElement };
  }

  it('renders the month, two chips with an overflow count, and the legend', async () => {
    const { el } = await mount();
    expect(el.querySelector('h2')!.textContent).toBe('March 2026');
    expect(el.textContent).toContain('3 items');
    expect(el.textContent).toContain('+1 more');
    expect(el.querySelectorAll('[role="gridcell"]').length % 7).toBe(0);
    expect(el.textContent).toContain('Overdue');
  });

  it('opens the day popover and jumps to the next month with events', async () => {
    const { fixture, el } = await mount();
    (el.querySelector('button[aria-label^="Tuesday, March 10"]') as HTMLElement).click();
    await settle(fixture);
    const pop = document.querySelector('[data-slot="popover-content"]')!;
    expect(pop.textContent).toContain('Tuesday, March 10, 2026');
    expect(pop.querySelectorAll('li').length).toBe(3);
    (pop.querySelector('[aria-label="Close"]') as HTMLElement).click();
    await settle(fixture);
    [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Next with events'))!.click();
    await settle(fixture);
    expect(el.querySelector('h2')!.textContent).toBe('May 2026');
  });
});
