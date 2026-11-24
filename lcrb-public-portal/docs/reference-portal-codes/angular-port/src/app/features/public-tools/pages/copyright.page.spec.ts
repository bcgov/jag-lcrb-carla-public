import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { CopyrightPage } from './copyright.page';

describe('CopyrightPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the title inside the legal layout', async () => {
    const fixture = TestBed.createComponent(CopyrightPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main h1')!.textContent).toBe('Copyright');
    expect(el.querySelector('main h1')!.className).toBe('text-3xl md:text-4xl font-semibold text-foreground text-balance');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelectorAll('main h2').length).toBeGreaterThan(0);
  });

  it('shows the current year and the Last updated line', async () => {
    const fixture = TestBed.createComponent(CopyrightPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain(`\u00a9 ${new Date().getFullYear()} Province of British Columbia. All rights reserved.`);
    expect(el.textContent).toContain('Last updated: June 1, 2026');
  });
});
