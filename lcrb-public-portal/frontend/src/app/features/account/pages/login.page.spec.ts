import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ROLE_LIST } from '@domain/roles';
import { LoginPage } from './login.page';

async function render() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(LoginPage);
  await fixture.whenStable();
  const el = fixture.nativeElement as HTMLElement;
  const settle = async () => {
    fixture.detectChanges();
    await fixture.whenStable();
  };
  return { fixture, el, settle };
}

const cards = (el: HTMLElement) => Array.from(el.querySelectorAll<HTMLButtonElement>('main .grid > button'));

describe('LoginPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('renders the role chooser with a card per role', async () => {
    const { el } = await render();
    expect(el.querySelector('h1')!.textContent).toContain('Sign in to Liquor and Cannabis Licensing');
    expect(cards(el).length).toBe(ROLE_LIST.length);
    expect(cards(el)[0].className).toBe(
      'group text-left bg-white rounded-lg border border-[#D9D9D9] p-6 transition-all hover:border-[#013366] hover:shadow-md focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90]',
    );
    expect(cards(el)[0].textContent).toContain('Sign in as Licensee');
    expect(el.querySelector('footer')!.className).toBe('bg-white text-[#474543] text-sm border-t-2 border-[#fcba19]');
    expect(el.querySelector('referral-banner [role="status"]')).toBeNull();
  });

  it('shows the pre-filled form for the chosen role and goes back', async () => {
    const { el, settle } = await render();
    cards(el)[1].click();
    await settle();
    expect(el.querySelector('h1')!.textContent).toBe('Special Event Permit Applicant');
    expect(el.querySelector('label[for="username"]')!.textContent).toBe('BCeID or BC Services Card');
    expect((el.querySelector('#username') as HTMLInputElement).value).toBe('demo.user');
    expect((el.querySelector('#password') as HTMLInputElement).value).toBe('demo');
    expect(el.textContent).toContain('Secured by BCeID or BC Services Card');
    el.querySelector<HTMLButtonElement>('main button[type="button"]')!.click();
    await settle();
    expect(cards(el).length).toBe(ROLE_LIST.length);
  });

  for (const role of ROLE_LIST) {
    it(`signs in as ${role.id} and navigates to the dashboard`, async () => {
      const { el, settle } = await render();
      const router = TestBed.inject(Router);
      const navigate = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
      cards(el)[ROLE_LIST.indexOf(role)].click();
      await settle();
      el.querySelector('form')!.dispatchEvent(new Event('submit', { cancelable: true }));
      await settle();
      expect(navigate).toHaveBeenCalledWith('/dashboard');
      const stored = JSON.parse(localStorage.getItem('lcrb.mock.session')!);
      expect(stored.role).toBe(role.id);
      expect(stored.firstName).toBeTruthy();
      expect(stored.email).toContain('@');
    });
  }

  it('labels the username field per role', async () => {
    const { el, settle } = await render();
    cards(el)[0].click();
    await settle();
    expect(el.querySelector('label[for="username"]')!.textContent).toBe('BCeID');
    el.querySelector<HTMLButtonElement>('main button[type="button"]')!.click();
    await settle();
    cards(el)[2].click();
    await settle();
    expect(el.querySelector('label[for="username"]')!.textContent).toBe('Username');
  });
});
