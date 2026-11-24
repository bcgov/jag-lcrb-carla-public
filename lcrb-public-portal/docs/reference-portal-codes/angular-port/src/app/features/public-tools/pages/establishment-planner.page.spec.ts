/** the establishment planner (plans list, create, edit, checklist, rename, delete, advisor). */
import { Component } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { replyWithChips } from '../../../../../tests/fixtures/api/stream';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import type { ApiFixture } from '../../../../../tests/support/api-fixtures';
import { createPlan, getPlan } from '../state/establishment-plan-store';
import { EstablishmentPlannerPage } from './establishment-planner.page';

@Component({ template: '' })
class Blank {}

let fixture: ComponentFixture<EstablishmentPlannerPage>;
let el: HTMLElement;

async function render() {
  fixture.detectChanges();
  await fixture.whenStable();
}
async function until(cond: () => boolean, label: string) {
  for (let i = 0; i < 100; i++) {
    await render();
    if (cond()) return;
    await new Promise((r) => setTimeout(r, 5));
  }
  throw new Error(`Timed out waiting for: ${label}`);
}
async function setup(fixtures: Record<string, ApiFixture> = {}, seed?: () => void) {
  const api = stubFetch(fixtures);
  seed?.();
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
  fixture = TestBed.createComponent(EstablishmentPlannerPage);
  el = fixture.nativeElement;
  for (let i = 0; i < 3; i++) await render();
  return { api, router: TestBed.inject(Router) };
}
const button = (name: string) =>
  [...el.querySelectorAll('button')].find((b) => (b.getAttribute('aria-label') ?? b.textContent?.trim().replace(/\s+/g, ' ')) === name);
const plans = () => Object.values(JSON.parse(localStorage.getItem('lcrb.v2.establishment-plans') ?? '{}')) as { id: string; title: string; answers: Record<string, unknown>; checklistDone: Record<string, boolean> }[];

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
});
afterEach(() => vi.unstubAllGlobals());

describe('EstablishmentPlannerPage', () => {
  it('renders the shell and intro with no saved plans', async () => {
    await setup();
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col bg-background');
    expect(el.querySelector('h1')!.textContent).toBe('Establishment Planner');
    expect(button('Start a new plan')).toBeTruthy();
    expect(el.textContent).not.toContain('Your saved plans');
  });

  it('creates a plan seeded from the business profile and opens the detail', async () => {
    await setup();
    button('Start a new plan')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');
    expect(plans()).toHaveLength(1);
    expect(plans()[0].title).toBe('Untitled establishment');
    expect(plans()[0].answers['applicantName']).toBe('LCRB Training Business 5 Ltd.');
    expect(el.querySelector('h1')!.textContent).toBe('Untitled establishment');
    expect(el.textContent).toContain('Choose an establishment type above');
    expect(el.textContent).toContain('Readiness');
    expect(el.textContent).toContain('Planner advisor');
    expect((el.querySelector('#applicantName') as HTMLInputElement).value).toBe('LCRB Training Business 5 Ltd.');
  });

  it('edits the answers, shows the summary and persists checklist progress to the store', async () => {
    await setup();
    button('Start a new plan')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');

    const typeBtn = [...el.querySelectorAll('fieldset button[aria-pressed]')][0] as HTMLButtonElement;
    typeBtn.click();
    await render();
    expect(typeBtn.getAttribute('aria-pressed')).toBe('true');
    expect(plans()[0].answers['establishmentType']).toBeTruthy();
    expect(el.textContent).toContain('Your feasibility summary');
    expect(el.textContent).toContain('Document & task checklist');

    const city = el.querySelector('#city') as HTMLInputElement;
    city.value = 'Victoria';
    city.dispatchEvent(new Event('input'));
    await render();
    expect(plans()[0].answers['city']).toBe('Victoria');

    const box = el.querySelector('button[role="checkbox"][id^="chk-"]') as HTMLButtonElement;
    box.click();
    await render();
    expect(Object.values(plans()[0].checklistDone)).toContain(true);
    expect(el.textContent).toMatch(/1 \/ \d+ done/);
  });

  it('clamps the licence counts and writes the clamped value back like a React controlled input', async () => {
    await setup();
    button('Start a new plan')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');
    const liquor = el.querySelector('#existingLiquor') as HTMLInputElement;
    liquor.value = '25';
    liquor.dispatchEvent(new Event('input'));
    await render();
    expect(plans()[0].answers['existingLiquorLicences']).toBe(20);
    expect(liquor.value).toBe('20');
    liquor.value = 'abc';
    liquor.dispatchEvent(new Event('input'));
    await render();
    expect(plans()[0].answers['existingLiquorLicences']).toBe(0);
    expect(liquor.value).toBe('0');
  });

  it('renames a plan', async () => {
    await setup();
    button('Start a new plan')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');
    button('Rename')!.click();
    await render();
    const input = el.querySelector('input[aria-label="Plan name"]') as HTMLInputElement;
    input.value = 'Blue Anchor';
    input.dispatchEvent(new Event('input'));
    await render();
    button('Save')!.click();
    await render();
    expect(plans()[0].title).toBe('Blue Anchor');
    expect(el.querySelector('h1')!.textContent).toBe('Blue Anchor');
  });

  it('lists saved plans, reopens one, goes back and deletes it', async () => {
    let id = '';
    await setup({}, () => {
      id = createPlan('Saved plan').id;
    });
    expect(el.textContent).toContain('Your saved plans');
    expect(el.textContent).toContain('Saved plan');
    expect(el.textContent).toContain('Not started');
    button('Open')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');
    button('All plans')!.click();
    await render();
    expect(el.querySelector('planner-interview')).toBeNull();
    button('Delete Saved plan')!.click();
    await render();
    expect(getPlan(id)).toBeUndefined();
    expect(el.textContent).not.toContain('Your saved plans');
  });

  it('streams an advisor reply with the plan summary as context', async () => {
    const { api } = await setup({ '/api/planner-advisor': replyWithChips('Start with zoning.', ['Next?']) });
    button('Start a new plan')!.click();
    await until(() => !!el.querySelector('planner-interview'), 'detail');
    button('What should I do first?')!.click();
    await until(() => el.textContent!.includes('Start with zoning.'), 'reply');
    const body = api.callsTo('/api/planner-advisor')[0].body as { context: { planSummary: string } };
    expect(body.context.planSummary).toContain('has not chosen an establishment type yet');
    expect(button('Next?')).toBeTruthy();
    button('Clear')!.click();
    await render();
    expect(el.textContent).toContain('I can explain your feasibility results');
  });
});
