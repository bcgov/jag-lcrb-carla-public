import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { listReceipts } from '@data-access/repositories/payments/receipt-model';
import { ReceiptsPage } from './receipts.page';

describe('ReceiptsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    Element.prototype.scrollIntoView = vi.fn();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the hero, breadcrumbs, receipts list and footer', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const fixture = TestBed.createComponent(ReceiptsPage);
    for (let i = 0; i < 3; i++) {
      fixture.detectChanges();
      await fixture.whenStable();
    }
    const el = fixture.nativeElement as HTMLElement;
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col');
    expect(el.querySelector('main#main-content')!.className).toBe('flex-1 bg-muted/20');
    expect(el.querySelector('h1')!.textContent).toContain('Receipts');
    expect(el.textContent).toContain('Official receipts for every fee you have paid.');
    expect(el.textContent).toContain('My Applications');
    expect(el.querySelector('main > div.container.mx-auto.max-w-4xl.px-4.py-8')).toBeTruthy();
    expect(el.querySelectorAll('main ul > li').length).toBe(listReceipts().length);
    expect(el.querySelector('footer')).toBeTruthy();
  });
});
