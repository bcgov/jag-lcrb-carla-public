/** the admin settings overlay switches tabs and edits local state. */
import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { SettingsPanel } from './settings-panel';

const tick = () => new Promise((r) => setTimeout(r, 0));

async function mount() {
  const fixture = TestBed.createComponent(SettingsPanel);
  document.body.appendChild(fixture.nativeElement);
  const settle = async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    await tick();
    fixture.detectChanges();
  };
  await settle();
  return { fixture, el: fixture.nativeElement as HTMLElement, settle };
}

const tab = (el: HTMLElement, label: string) =>
  Array.from(el.querySelectorAll('button[role="tab"]')).find((b) => b.textContent?.includes(label)) as HTMLButtonElement;

describe('SettingsPanel', () => {
  it('starts on General and switches tabs', async () => {
    const { el, settle } = await mount();
    expect(el.textContent).toContain('System Settings');
    expect(el.querySelectorAll('button[role="tab"]').length).toBe(7);
    expect(tab(el, 'General').getAttribute('aria-selected')).toBe('true');
    expect(el.textContent).toContain('Map Defaults');
    expect(el.textContent).not.toContain('Risk Score Thresholds');

    tab(el, 'AI').click();
    await settle();
    expect(tab(el, 'AI').getAttribute('aria-selected')).toBe('true');
    expect(el.textContent).toContain('Risk Score Thresholds');
    expect(el.textContent).toContain('Current Ranges: Low (0-39), Medium (40-69), High (70+)');

    tab(el, 'Users').click();
    await settle();
    expect(el.textContent).toContain('Administrator');
    expect(el.textContent).toContain('156 users');
  });

  it('switches enable dependent controls and the slider updates the range text', async () => {
    const { el, settle } = await mount();
    tab(el, 'Notifications').click();
    await settle();
    const switches = () => Array.from(el.querySelectorAll('button[role="switch"]')) as HTMLButtonElement[];
    expect(switches()[1].disabled).toBe(false);
    switches()[0].click();
    await settle();
    expect(switches()[0].getAttribute('aria-checked')).toBe('false');
    expect(switches()[1].disabled).toBe(true);
    expect(el.textContent).not.toContain('Reminder Lead Time');

    tab(el, 'AI').click();
    await settle();
    const thumb = el.querySelector('[role="slider"]') as HTMLElement;
    thumb.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight', bubbles: true }));
    await settle();
    expect(el.textContent).toContain('High (75+)');
  });

  it('adds and removes a custom monitoring website, and closes through Cancel', async () => {
    const { fixture, el, settle } = await mount();
    let closed = 0;
    fixture.componentInstance.closed.subscribe(() => closed++);
    tab(el, 'Integrations').click();
    await settle();
    expect(el.textContent).toContain('https://example-cannabis-review.ca');
    const input = el.querySelector('input[placeholder="https://example.com"]') as HTMLInputElement;
    input.value = 'https://new.example.org';
    input.dispatchEvent(new Event('input', { bubbles: true }));
    await settle();
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', bubbles: true }));
    await settle();
    expect(el.textContent).toContain('https://new.example.org');
    expect(input.value).toBe('');
    const row = Array.from(el.querySelectorAll('span')).find((s) => s.textContent === 'https://new.example.org')!.parentElement!;
    (row.querySelector('button') as HTMLButtonElement).click();
    await settle();
    expect(el.textContent).not.toContain('https://new.example.org');
    (Array.from(el.querySelectorAll('button')).find((b) => b.textContent?.trim() === 'Cancel') as HTMLButtonElement).click();
    expect(closed).toBe(1);
  });
});
