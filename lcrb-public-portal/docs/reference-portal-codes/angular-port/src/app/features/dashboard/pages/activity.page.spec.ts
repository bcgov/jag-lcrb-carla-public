import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ActivityPage } from './activity.page';

async function mount(session?: object) {
  if (session) localStorage.setItem('lcrb.mock.session', JSON.stringify(session));
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const nav = vi.spyOn(TestBed.inject(Router), 'navigateByUrl').mockResolvedValue(true);
  const fixture = TestBed.createComponent(ActivityPage);
  expect(nav).not.toHaveBeenCalled();
  for (let i = 0; i < 4; i++) {
    await new Promise((r) => setTimeout(r, 5));
    fixture.detectChanges();
    await fixture.whenStable();
  }
  return { fixture, el: fixture.nativeElement as HTMLElement, nav };
}

describe('ActivityPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the hero and console for a signed-in user without redirecting', async () => {
    const { el, nav } = await mount({ role: 'police', firstName: 'A', lastName: 'B', organization: 'O' });
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col bg-background');
    expect(el.querySelector('h1')!.textContent).toContain('Activity & Integrations');
    expect(el.textContent).toContain('A live record of actions across the portal');
    expect(el.querySelector('main#main-content')!.className).toBe('flex-1');
    expect(el.querySelector('[role="tablist"]')).toBeTruthy();
    expect(nav).not.toHaveBeenCalled();
  });

  it('redirects to /login with replace when signed out once loading is done', async () => {
    const { nav } = await mount();
    expect(nav).toHaveBeenCalledWith('/login', { replaceUrl: true });
  });
});
