import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { ContactPage } from './contact.page';

describe('ContactPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the title inside the legal layout', async () => {
    const fixture = TestBed.createComponent(ContactPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main h1')!.textContent).toBe('Contact Us');
    expect(el.querySelector('main h1')!.className).toBe('text-3xl md:text-4xl font-semibold text-foreground text-balance');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelectorAll('main h2').length).toBeGreaterThan(0);
  });

  it('has no "Last updated" line and keeps the mailto links and line breaks', async () => {
    const fixture = TestBed.createComponent(ContactPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).not.toContain('Last updated');
    expect(el.querySelector('a[href="mailto:LCRBLiquor@gov.bc.ca"]')!.textContent).toBe('LCRBLiquor@gov.bc.ca');
    expect(el.querySelectorAll('a[href^="mailto:"]').length).toBe(4);
    expect(el.querySelectorAll('main br').length).toBe(5);
  });
});
