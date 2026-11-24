import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { AnnouncementsPage } from './announcements.page';

describe('AnnouncementsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the hero with breadcrumbs and the list', async () => {
    const fixture = TestBed.createComponent(AnnouncementsPage);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('main')!.className).toBe('flex-1 bg-muted/20');
    expect(el.querySelector('main')!.id).toBe('main-content');
    expect(el.querySelector('main h1')!.textContent).toBe('Announcements');
    expect(el.querySelector('nav[aria-label="Breadcrumb"] a')!.getAttribute('href')).toBe('/dashboard');
    expect(el.querySelector('main p')!.textContent).toContain('Policy updates, regulatory changes');
    expect(el.querySelectorAll('announcements-list article').length).toBeGreaterThan(0);
  });
});
