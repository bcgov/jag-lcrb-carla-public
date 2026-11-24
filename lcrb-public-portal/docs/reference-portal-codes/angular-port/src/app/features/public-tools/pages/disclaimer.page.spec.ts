import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { DisclaimerPage } from './disclaimer.page';

describe('DisclaimerPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the title inside the legal layout', async () => {
    const fixture = TestBed.createComponent(DisclaimerPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main h1')!.textContent).toBe('Disclaimer');
    expect(el.querySelector('main h1')!.className).toBe('text-3xl md:text-4xl font-semibold text-foreground text-balance');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelectorAll('main h2').length).toBeGreaterThan(0);
  });

  it('keeps the curly quotes and punctuation after links', async () => {
    const fixture = TestBed.createComponent(DisclaimerPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('\u201cas is\u201d');
    const link = el.querySelector('main a[target="_blank"]')!;
    expect(link.nextSibling!.textContent!.startsWith(', the ')).toBe(true);
  });
});
