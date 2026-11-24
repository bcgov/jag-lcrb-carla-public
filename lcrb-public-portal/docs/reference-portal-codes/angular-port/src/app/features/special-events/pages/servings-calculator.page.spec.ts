import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SERVINGS_PREFILL_KEY } from '@core/navigation/servings-prefill';
import { ServingsCalculatorPage } from './servings-calculator.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  f.detectChanges();
  await f.whenStable();
  f.detectChanges();
};

async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(ServingsCalculatorPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const total = (el: HTMLElement) => el.querySelector('p.text-3xl')!.textContent!.trim();
const setNumber = async (f: Parameters<typeof settle>[0], el: HTMLElement, id: string, value: string) => {
  const input = el.querySelector(`#${id}`) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
  await settle(f);
};

describe('ServingsCalculatorPage', () => {
  beforeEach(() => sessionStorage.clear());
  afterEach(() => {
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders the defaults: 80 guests, 4 hours, moderate, 50/35/15 mix and the computed list', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toBe('Servings Calculator');
    expect((el.querySelector('#guests') as HTMLInputElement).value).toBe('80');
    expect((el.querySelector('#hours') as HTMLInputElement).value).toBe('4');
    // drinkers 60, perDrinker 2 + 3 = 5, buffer 10%
    const drinks = Math.ceil(60 * 5 * 1.1);
    expect(total(el)).toBe(drinks.toLocaleString());
    expect(el.textContent).toContain('About 60 of 80 guests are expected to drink.');
    expect(el.textContent).toContain('≈ 5 drinks per drinking guest over 4h');
    expect(el.textContent).toContain(`${Math.ceil((drinks * 50) / 100)} cans`);
    expect(el.textContent).toContain(`${Math.ceil(((drinks * 35) / 100) / 5)} bottles`);
    expect(el.textContent).toContain(`${Math.ceil(((drinks * 15) / 100) / 16)} bottles`);
    expect(el.textContent).toContain(`~${(Math.ceil((drinks * 50) / 100) / 24).toFixed(1)} cases of 24`);
    const buttons = [...el.querySelectorAll('main button[aria-pressed]')];
    expect(buttons.map((b) => b.getAttribute('aria-pressed'))).toEqual(['false', 'true', 'false']);
  });

  it('recomputes when guests, hours and the consumption level change, and clamps the inputs', async () => {
    const { fixture, el } = await mount();
    await setNumber(fixture, el, 'guests', '100');
    await setNumber(fixture, el, 'hours', '1');
    // 75 drinkers * 2 drinks, buffer 10%
    expect(total(el)).toBe(Math.ceil(75 * 2 * 1.1).toLocaleString());
    (el.querySelectorAll('main button[aria-pressed]')[0] as HTMLButtonElement).click();
    await settle(fixture);
    expect(total(el)).toBe(Math.ceil(75 * 1 * 1.1).toLocaleString());
    await setNumber(fixture, el, 'guests', '99999');
    expect((el.querySelector('#guests') as HTMLInputElement).value).toBe('10000');
    await setNumber(fixture, el, 'hours', '0');
    expect(el.textContent).toContain('over 1h');
  });

  it('drops the buffer and resets to the defaults', async () => {
    const { fixture, el } = await mount();
    const box = el.querySelector('input[type="checkbox"]') as HTMLInputElement;
    box.checked = false;
    box.dispatchEvent(new Event('change'));
    await settle(fixture);
    expect(total(el)).toBe('300');
    await setNumber(fixture, el, 'guests', '10');
    const reset = [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Reset')!;
    reset.click();
    await settle(fixture);
    expect((el.querySelector('#guests') as HTMLInputElement).value).toBe('80');
    expect((el.querySelector('input[type="checkbox"]') as HTMLInputElement).checked).toBe(true);
  });

  it('writes the prefill and navigates to the application on Apply', async () => {
    const { fixture, el } = await mount();
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigateByUrl').mockResolvedValue(true);
    const apply = [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Apply for a Special Event Permit'))!;
    apply.click();
    await settle(fixture);
    const drinks = Math.ceil(60 * 5 * 1.1);
    expect(JSON.parse(sessionStorage.getItem(SERVINGS_PREFILL_KEY)!)).toEqual({
      estimatedDrinks: String(drinks),
      maxGuests: '80',
      liquorTypes: ['beer', 'wine', 'spirits'],
    });
    expect(navigate).toHaveBeenCalledWith('/special-events/apply');
  });
});
