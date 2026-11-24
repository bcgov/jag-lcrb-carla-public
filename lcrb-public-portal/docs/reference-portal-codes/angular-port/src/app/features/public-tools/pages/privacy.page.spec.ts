import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { PrivacyPage } from './privacy.page';

describe('PrivacyPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the title inside the legal layout', async () => {
    const fixture = TestBed.createComponent(PrivacyPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main h1')!.textContent).toBe('Privacy');
    expect(el.querySelector('main h1')!.className).toBe('text-3xl md:text-4xl font-semibold text-foreground text-balance');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelectorAll('main h2').length).toBeGreaterThan(0);
  });

  it('keeps the external link attributes and the plain /contact link', async () => {
    const fixture = TestBed.createComponent(PrivacyPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    const ext = el.querySelector('main a[target="_blank"]')!;
    expect(ext.getAttribute('rel')).toBe('noopener noreferrer');
    expect(ext.textContent).toBe('Freedom of Information and Protection of Privacy Act');
    expect(ext.nextSibling!.textContent).toBe(' (FOIPPA).');
    expect(el.querySelector('main a[href="/contact"]')!.textContent).toBe('contact the LCRB');
  });
});
