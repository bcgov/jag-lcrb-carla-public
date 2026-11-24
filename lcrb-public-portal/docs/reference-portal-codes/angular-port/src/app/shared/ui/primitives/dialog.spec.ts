import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Button } from './button';
import { DIALOG } from './dialog';

@Component({
  imports: [...DIALOG, Button],
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="open.set($event === 'open')">
      <button uiButton uiDialogTrigger id="trigger">Open</button>
      <ui-dialog-content *uiDialogPortal class="sm:max-w-md">
        <div uiDialogHeader><h2 uiDialogTitle>Approve permit</h2><p uiDialogDescription>Desc</p></div>
        <div uiDialogFooter><button uiDialogClose id="cancel">Cancel</button></div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
class Host {
  open = signal(false);
}

const content = () => document.querySelector('ui-dialog-content') as HTMLElement | null;
const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  f.detectChanges();
  await f.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  f.detectChanges();
};

describe('dialog', () => {
  afterEach(() => document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

  it('opens from the trigger with the prototype classes, slots and state', async () => {
    const fixture = TestBed.createComponent(Host);
    await settle(fixture);
    expect(content()).toBeNull();
    (fixture.nativeElement.querySelector('#trigger') as HTMLButtonElement).click();
    await settle(fixture);
    const el = content()!;
    expect(el).not.toBeNull();
    expect(fixture.componentInstance.open()).toBe(true);
    expect(el.getAttribute('data-slot')).toBe('dialog-content');
    expect(el.getAttribute('data-state')).toBe('open');
    expect(el.closest('[role="dialog"]')?.getAttribute('aria-modal')).toBe('true');
    expect(document.querySelectorAll('[role="dialog"]').length).toBe(1);
    expect(el.className).toContain('sm:max-w-md');
    expect(el.className).not.toContain('sm:max-w-lg');
    expect(el.querySelector('[data-slot="dialog-title"]')!.textContent).toBe('Approve permit');
    const close = el.querySelector('button[data-slot="dialog-close"] .sr-only')!;
    expect(close.textContent).toBe('Close');
    const backdrop = document.querySelector('.cdk-overlay-backdrop') as HTMLElement;
    expect(backdrop.className).toContain('bg-black/50');
    expect(backdrop.getAttribute('data-state')).toBe('open');
  });

  it('closes from a close button and from controlled state', async () => {
    const fixture = TestBed.createComponent(Host);
    fixture.componentInstance.open.set(true);
    await settle(fixture);
    expect(content()).not.toBeNull();
    (document.querySelector('#cancel') as HTMLButtonElement).click();
    await settle(fixture);
    expect(fixture.componentInstance.open()).toBe(false);

    fixture.componentInstance.open.set(true);
    await settle(fixture);
    expect(content()?.getAttribute('data-state')).toBe('open');
    fixture.componentInstance.open.set(false);
    await settle(fixture);
    expect(content() === null || content()!.getAttribute('data-state') === 'closed').toBe(true);
  });

  it('closes on Escape', async () => {
    const fixture = TestBed.createComponent(Host);
    fixture.componentInstance.open.set(true);
    await settle(fixture);
    content()!.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    await settle(fixture);
    expect(fixture.componentInstance.open()).toBe(false);
  });
});
