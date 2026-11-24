import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { NoticesPage } from './notices.page';

async function render() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(NoticesPage);
  await fixture.whenStable();
  const el = fixture.nativeElement as HTMLElement;
  const settle = async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
    fixture.detectChanges();
  };
  const filterBtns = () => Array.from(el.querySelectorAll<HTMLButtonElement>('[role="group"] > button[aria-pressed]'));
  return { fixture, el, settle, filterBtns };
}

describe('NoticesPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });

  it('renders the hero, filters with counts and the default licensee feed', async () => {
    const { el, filterBtns } = await render();
    expect(el.querySelector('main h1')!.textContent).toBe('Notices');
    const labels = filterBtns().map((b) => b.textContent!.trim());
    expect(labels[0]).toMatch(/^All \(\d+\)$/);
    expect(labels[1]).toMatch(/^Unread \(\d+\)$/);
    expect(labels[2]).toMatch(/^Official notices \(\d+\)$/);
    expect(labels[3]).toMatch(/^Alerts \(\d+\)$/);
    expect(filterBtns()[0].getAttribute('aria-pressed')).toBe('true');
    expect(filterBtns()[0].className).toBe('rounded-full border px-3.5 py-1.5 text-sm font-medium transition-colors border-[#013366] bg-[#013366] text-white');
    expect(filterBtns()[1].className).toBe('rounded-full border px-3.5 py-1.5 text-sm font-medium transition-colors bg-white text-foreground hover:bg-muted');
    expect(el.textContent).toContain('Official notice');
    expect(el.textContent).toContain('Notices attached to your account are shown here');
  });

  it('filters to official notices and to alerts', async () => {
    const { el, settle, filterBtns } = await render();
    filterBtns()[2].click();
    await settle();
    expect(filterBtns()[2].getAttribute('aria-pressed')).toBe('true');
    expect(el.querySelectorAll('notice-document-viewer').length).toBeGreaterThan(0);
    expect(el.textContent).not.toContain('Alert ·');
    filterBtns()[3].click();
    await settle();
    expect(el.querySelectorAll('notice-document-viewer').length).toBe(0);
  });

  it('marks all as read, then the Unread filter shows the empty state', async () => {
    const { el, settle, filterBtns } = await render();
    const markAll = Array.from(el.querySelectorAll('button')).find((b) => b.textContent!.includes('Mark all as read'))!;
    expect(el.querySelector('main .bg-bc-gold')!.textContent).toMatch(/\d+ unread/);
    markAll.click();
    await settle();
    expect(el.querySelector('main .bg-bc-gold')).toBeNull();
    expect(Array.from(el.querySelectorAll('button')).some((b) => b.textContent!.includes('Mark all as read'))).toBe(false);
    filterBtns()[1].click();
    await settle();
    expect(el.querySelector('h3')!.textContent).toBe("You're all caught up");
    expect(el.textContent).toContain('Every notice and alert has been read.');
  });

  it('opens a notice document, which marks the notice as read', async () => {
    const { el, settle, filterBtns } = await render();
    filterBtns()[2].click();
    await settle();
    const unreadBefore = el.querySelectorAll('span[aria-label="Unread"]').length;
    expect(unreadBefore).toBeGreaterThan(0);
    const open = Array.from(el.querySelectorAll('notice-document-viewer button')).find((b) => b.textContent!.includes('Read full notice')) as HTMLButtonElement;
    open.click();
    await settle();
    expect(document.querySelector('ui-dialog-content')).not.toBeNull();
    expect(el.querySelectorAll('span[aria-label="Unread"]').length).toBe(unreadBefore - 1);
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });
});
