import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { AccessibilityPage } from './accessibility.page';

describe('AccessibilityPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the title inside the legal layout', async () => {
    const fixture = TestBed.createComponent(AccessibilityPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main h1')!.textContent).toBe('Accessibility');
    expect(el.querySelector('main h1')!.className).toBe('text-3xl md:text-4xl font-semibold text-foreground text-balance');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelectorAll('main h2').length).toBeGreaterThan(0);
  });

  it('lists the accessibility features and links to the Act', async () => {
    const fixture = TestBed.createComponent(AccessibilityPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelectorAll('main li').length).toBe(5);
    expect(el.querySelector('main a[target="_blank"]')!.textContent).toBe('Accessible British Columbia Act');
    expect(el.textContent).toContain('Last updated: June 1, 2026');
  });
});
