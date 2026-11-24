import { TestBed } from '@angular/core/testing';
import { Meta } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { describe, expect, it, vi } from 'vitest';
import { OneDoorPage } from './one-door.page';

describe('OneDoorPage', () => {
  it('renders the standalone experience without the LCRB portal chrome', async () => {
    Element.prototype.scrollTo = vi.fn() as never;
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const fixture = TestBed.createComponent(OneDoorPage);
    fixture.detectChanges();
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('one-door-experience')).toBeTruthy();
    expect(el.querySelector('main#main-content')).toBeTruthy();
    expect(el.textContent).toContain('Government of British Columbia');
    expect(el.textContent).toContain('Not an official government service');
    expect(el.querySelector('bc-gov-header')).toBeNull();
  });

  it('sets robots noindex while shown, as the prototype metadata does', async () => {
    Element.prototype.scrollTo = vi.fn() as never;
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const meta = TestBed.inject(Meta);
    const fixture = TestBed.createComponent(OneDoorPage);
    fixture.detectChanges();
    expect(meta.getTag('name="robots"')?.content).toBe('noindex, nofollow');
    fixture.destroy();
    expect(meta.getTag('name="robots"')).toBeNull();
  });
});
