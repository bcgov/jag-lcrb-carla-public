import { TestBed } from '@angular/core/testing';
import { beforeEach, describe, expect, it } from 'vitest';
import { ANNOUNCEMENTS, latestAnnouncementId } from '@data-access/repositories/notifications/mock/announcements';
import { CATEGORY_LABELS } from '@domain/notifications/announcements';
import { AnnouncementsList } from './announcements-list';

describe('AnnouncementsList', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('marks all announcements seen after mount', async () => {
    expect(localStorage.getItem('lcrb.announcements.lastSeen')).toBeNull();
    const fixture = TestBed.createComponent(AnnouncementsList);
    await fixture.whenStable();
    const stored = Object.entries(localStorage).find(([, v]) => v === latestAnnouncementId());
    expect(stored).toBeTruthy();
  });

  it('renders a card per announcement with category, version and the Latest badge on the first only', async () => {
    const fixture = TestBed.createComponent(AnnouncementsList);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    const items = el.querySelectorAll('ol > li');
    expect(items.length).toBe(ANNOUNCEMENTS.length);
    expect(el.querySelector('article')!.className).toBe('overflow-hidden rounded-xl border border-border bg-card shadow-sm');
    const first = items[0];
    const chip = first.querySelector('article span')!;
    expect(chip.textContent!.trim()).toBe(CATEGORY_LABELS[ANNOUNCEMENTS[0].category]);
    expect(chip.className).toContain('rounded-full px-2.5 py-0.5 text-xs font-semibold ');
    expect(first.textContent).toContain('Latest');
    expect(el.querySelectorAll('.bg-\\[\\#D8292F\\]').length).toBe(1);
    expect(first.querySelector('h2')!.textContent).toBe(ANNOUNCEMENTS[0].title);
    expect(first.querySelector('time')!.getAttribute('datetime')).toBe(ANNOUNCEMENTS[0].date);
  });

  it('renders the changelog entries with labelled icons', async () => {
    const fixture = TestBed.createComponent(AnnouncementsList);
    await fixture.whenStable();
    const el = fixture.nativeElement as HTMLElement;
    const withLog = ANNOUNCEMENTS.find((a) => a.changelog?.length)!;
    expect(el.querySelectorAll('h3').length).toBe(ANNOUNCEMENTS.filter((a) => a.changelog?.length).length);
    expect(el.textContent).toContain(withLog.changelog![0].text);
    expect(el.querySelector('h3 + ul li svg.lucide')).not.toBeNull();
  });
});
