/** Behaviour primitives and custom pieces: Radix-equivalent state attributes, keyboard and pointer behaviour, and storage effects. */
import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Checkbox } from '@shared/ui/primitives/checkbox';
import { Switch } from '@shared/ui/primitives/switch';
import { Progress } from '@shared/ui/primitives/progress';
import { RADIO_GROUP } from '@shared/ui/primitives/radio-group';
import { TABS } from '@shared/ui/primitives/tabs';
import { SELECT } from '@shared/ui/primitives/select';
import { COMMAND } from '@shared/ui/primitives/command';
import { Slider } from '@shared/ui/primitives/slider';
import { SHEET } from '@shared/ui/primitives/sheet';
import { ALERT_DIALOG } from '@shared/ui/primitives/alert-dialog';
import { POPOVER } from '@shared/ui/primitives/popover';
import { DROPDOWN_MENU } from '@shared/ui/primitives/dropdown-menu';
import { StatTile } from '@shared/ui/primitives/stat-tile';
import { DismissibleNotice } from '@shared/ui/primitives/dismissible-notice';
import { LucideIcon, lucideClassFor } from '@shared/ui/primitives/lucide-icon';
import { ICONS } from '@shared/ui/primitives/icons';
import { injectToast } from '@shared/ui/primitives/use-toast';

const tick = () => new Promise((r) => setTimeout(r, 0));
async function render<T>(cmp: new () => T) {
  const fixture = TestBed.createComponent(cmp);
  document.body.appendChild(fixture.nativeElement);
  fixture.detectChanges();
  await fixture.whenStable();
  await tick();
  fixture.detectChanges();
  const settle = async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    await tick();
    fixture.detectChanges();
  };
  return { fixture, el: fixture.nativeElement as HTMLElement, settle };
}

afterEach(() => {
  document.body.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  localStorage.clear();
});

describe('lucide icon', () => {
  @Component({ imports: [LucideIcon], template: `<svg lucide="Loader2" class="h-4 w-4"></svg><svg lucide="X" aria-label="Close"></svg>` })
  class Host {}

  it('renders the svg the way lucide-react 0.564 does', async () => {
    const { el } = await render(Host);
    const [svg, labelled] = Array.from(el.querySelectorAll('svg'));
    expect(svg.getAttribute('class')).toBe(lucideClassFor('loader-circle', 'h-4 w-4'));
    expect(svg.getAttribute('class')).toBe('lucide lucide-loader-circle h-4 w-4');
    expect(svg.getAttribute('viewBox')).toBe('0 0 24 24');
    expect(svg.getAttribute('stroke-width')).toBe('2');
    expect(svg.getAttribute('aria-hidden')).toBe('true');
    expect(svg.children.length).toBe(ICONS.Loader2[1].length);
    expect(labelled.hasAttribute('aria-hidden')).toBe(false);
  });

  it('keeps lucide-react class quirks for names with digits', () => {
    expect(lucideClassFor('building-2')).toBe('lucide lucide-building2 lucide-building-2');
  });
});

describe('form controls', () => {
  @Component({
    imports: [Checkbox, Switch, Progress, ...RADIO_GROUP],
    template: `
      <button uiCheckbox id="cb" [(checked)]="checked"></button>
      <button uiSwitch id="sw" [(checked)]="on"></button>
      <div uiProgress id="pg" [value]="40"></div>
      <div uiRadioGroup [(value)]="choice">
        <button uiRadioGroupItem id="r-yes" value="yes"></button>
        <button uiRadioGroupItem id="r-no" value="no"></button>
      </div>`,
  })
  class Host {
    checked = signal(false);
    on = signal(true);
    choice = signal<string | undefined>('yes');
  }

  it('checkbox and switch toggle state attributes', async () => {
    const { el, fixture, settle } = await render(Host);
    const cb = el.querySelector('#cb') as HTMLButtonElement;
    expect(cb.getAttribute('data-state')).toBe('unchecked');
    cb.click();
    await settle();
    expect(fixture.componentInstance.checked()).toBe(true);
    expect(cb.getAttribute('aria-checked')).toBe('true');
    expect(cb.querySelector('[data-slot="checkbox-indicator"] svg')).not.toBeNull();
    const sw = el.querySelector('#sw') as HTMLButtonElement;
    expect(sw.getAttribute('role')).toBe('switch');
    expect(sw.querySelector('[data-slot="switch-thumb"]')!.getAttribute('data-state')).toBe('checked');
  });

  it('progress translates the indicator', async () => {
    const { el } = await render(Host);
    const ind = el.querySelector('#pg [data-slot="progress-indicator"]') as HTMLElement;
    expect(ind.style.transform).toBe('translateX(-60%)');
  });

  it('radio group selects by click and arrow keys', async () => {
    const { el, fixture, settle } = await render(Host);
    const yes = el.querySelector('#r-yes') as HTMLButtonElement;
    const no = el.querySelector('#r-no') as HTMLButtonElement;
    expect(yes.getAttribute('aria-checked')).toBe('true');
    yes.focus();
    yes.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    await settle();
    expect(fixture.componentInstance.choice()).toBe('no');
    expect(no.getAttribute('data-state')).toBe('checked');
  });
});

describe('tabs', () => {
  @Component({
    imports: [...TABS],
    template: `<div uiTabs [(value)]="tab">
      <div uiTabsList><button uiTabsTrigger value="a" id="ta">A</button><button uiTabsTrigger value="b" id="tb">B</button></div>
      <div uiTabsContent value="a" id="ca">Panel A</div>
      <div uiTabsContent value="b" id="cb">Panel B</div>
    </div>`,
  })
  class Host {
    tab = signal<string | undefined>('a');
  }

  it('shows only the active panel and switches on click', async () => {
    const { el, settle } = await render(Host);
    expect(el.querySelector('#ta')!.getAttribute('data-state')).toBe('active');
    expect(el.querySelector('#ca')!.textContent).toContain('Panel A');
    expect(el.querySelector('#cb')!.textContent).toBe('');
    (el.querySelector('#tb') as HTMLButtonElement).click();
    await settle();
    expect(el.querySelector('#tb')!.getAttribute('aria-selected')).toBe('true');
    expect(el.querySelector('#cb')!.textContent).toContain('Panel B');
  });
});

describe('select', () => {
  @Component({
    imports: [...SELECT],
    template: `<ui-select [(value)]="value">
      <button uiSelectTrigger id="trigger" class="w-full"><span uiSelectValue placeholder="Choose a licence"></span></button>
      <ng-template uiSelectContent>
        <ui-select-item value="1">Licence one</ui-select-item>
        <ui-select-item value="2">Licence two</ui-select-item>
      </ng-template>
    </ui-select>`,
  })
  class Host {
    value = signal<string | undefined>(undefined);
  }

  it('shows the placeholder, opens a listbox, and shows the chosen label', async () => {
    const { el, fixture, settle } = await render(Host);
    const trigger = el.querySelector('#trigger') as HTMLButtonElement;
    expect(trigger.textContent).toContain('Choose a licence');
    expect(trigger.hasAttribute('data-placeholder')).toBe(true);
    trigger.click();
    await settle();
    const options = Array.from(document.querySelectorAll('.cdk-overlay-container [role="option"]')) as HTMLElement[];
    expect(options.map((o) => o.textContent?.trim())).toEqual(['Licence one', 'Licence two']);
    options[1].click();
    await settle();
    expect(fixture.componentInstance.value()).toBe('2');
    expect(trigger.textContent).toContain('Licence two');
    expect(trigger.hasAttribute('data-placeholder')).toBe(false);
    expect(trigger.getAttribute('aria-expanded')).toBe('false');
  });
});

describe('command', () => {
  @Component({
    imports: [...COMMAND],
    template: `<div uiCommand>
      <ui-command-input placeholder="Search" />
      <div uiCommandList>
        <div uiCommandEmpty>No results</div>
        <div uiCommandGroup heading="Licences">
          @for (i of items(); track i) { <div uiCommandItem (selected)="picked.set(i)">{{ i }}</div> }
        </div>
      </div>
    </div>`,
  })
  class Host {
    items = signal(['One', 'Two']);
    picked = signal('');
  }

  it('selects the first item, moves with arrows, picks with Enter, and shows Empty when no items', async () => {
    const { el, fixture, settle } = await render(Host);
    await settle();
    const items = () => Array.from(el.querySelectorAll('[cmdk-item]')) as HTMLElement[];
    expect(items()[0].getAttribute('data-selected')).toBe('true');
    expect(el.querySelector('[cmdk-group-heading]')!.textContent).toBe('Licences');
    const root = el.querySelector('[cmdk-root]') as HTMLElement;
    root.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
    await settle();
    expect(items()[1].getAttribute('data-selected')).toBe('true');
    root.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', bubbles: true }));
    expect(fixture.componentInstance.picked()).toBe('Two');
    expect((el.querySelector('[cmdk-empty]') as HTMLElement).hidden).toBe(true);
    fixture.componentInstance.items.set([]);
    await settle();
    expect(el.querySelector('[cmdk-empty]')!.textContent).toContain('No results');
  });
});

describe('slider', () => {
  @Component({ imports: [Slider], template: `<span uiSlider [(value)]="v" [step]="5"></span>` })
  class Host {
    v = signal<number[] | undefined>([50]);
  }

  it('steps with arrow keys and keeps the thumb in bounds', async () => {
    const { el, fixture, settle } = await render(Host);
    const thumb = el.querySelector('[role="slider"]') as HTMLElement;
    thumb.dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight', bubbles: true }));
    await settle();
    expect(fixture.componentInstance.v()).toEqual([55]);
    thumb.dispatchEvent(new KeyboardEvent('keydown', { key: 'Home', bubbles: true }));
    await settle();
    expect((thumb.parentElement as HTMLElement).style.left).toBe('calc(0% + 8px)');
    expect((el.querySelector('[data-slot="slider-range"]') as HTMLElement).style.right).toBe('100%');
  });
});

describe('overlays', () => {
  @Component({
    imports: [...SHEET, ...ALERT_DIALOG, ...POPOVER, ...DROPDOWN_MENU],
    template: `
      <ui-sheet [state]="sheet() ? 'open' : 'closed'" (stateChanged)="sheet.set($event === 'open')">
        <ui-sheet-content *uiSheetPortal side="right" class="w-80"><h2 uiSheetTitle>Menu</h2></ui-sheet-content>
      </ui-sheet>
      <ui-alert-dialog [state]="alert() ? 'open' : 'closed'" (stateChanged)="alert.set($event === 'open')">
        <ui-alert-dialog-content *uiAlertDialogPortal>
          <button uiAlertDialogCancel id="ad-cancel">Cancel</button>
        </ui-alert-dialog-content>
      </ui-alert-dialog>
      <ui-popover align="end"><button uiPopoverTrigger id="pop">Bell</button><div uiPopoverContent *uiPopoverPortal class="w-80 p-0">Popover body</div></ui-popover>
      <button [uiDropdownMenuTrigger]="menu" align="end" id="dd">User</button>
      <ng-template #menu><div uiDropdownMenuContent class="w-56">
        <div uiDropdownMenuLabel>Signed in</div><div uiDropdownMenuSeparator></div>
        <button uiDropdownMenuItem variant="destructive" (triggered)="signedOut.set(true)" id="dd-out">Sign Out</button>
      </div></ng-template>`,
  })
  class Host {
    sheet = signal(false);
    alert = signal(false);
    signedOut = signal(false);
  }

  it('sheet opens with the side classes and a close button', async () => {
    const { fixture, settle } = await render(Host);
    fixture.componentInstance.sheet.set(true);
    await settle();
    const content = document.querySelector('ui-sheet-content') as HTMLElement;
    expect(content.getAttribute('data-state')).toBe('open');
    expect(content.className).toContain('slide-in-from-right');
    expect(content.className).toContain('w-80');
    expect(content.querySelector('.sr-only')!.textContent).toBe('Close');
  });

  it('alert dialog uses role alertdialog and closes from Cancel', async () => {
    const { fixture, settle } = await render(Host);
    fixture.componentInstance.alert.set(true);
    await settle();
    expect(document.querySelector('ui-alert-dialog-content')!.closest('[role="alertdialog"]')).not.toBeNull();
    expect(document.querySelectorAll('[role="alertdialog"], [role="dialog"]').length).toBe(1);
    (document.querySelector('#ad-cancel') as HTMLButtonElement).click();
    await settle();
    expect(fixture.componentInstance.alert()).toBe(false);
  });

  it('popover opens from its trigger', async () => {
    const { el, settle } = await render(Host);
    (el.querySelector('#pop') as HTMLButtonElement).click();
    await settle();
    const content = document.querySelector('[data-slot="popover-content"]') as HTMLElement;
    expect(content.textContent).toContain('Popover body');
    expect(content.className).toContain('w-80');
  });

  it('dropdown menu opens, and an item fires triggered', async () => {
    const { el, fixture, settle } = await render(Host);
    (el.querySelector('#dd') as HTMLButtonElement).click();
    await settle();
    const content = document.querySelector('[data-slot="dropdown-menu-content"]') as HTMLElement;
    expect(content.getAttribute('role')).toBe('menu');
    // Pointer open focuses the content, not the first item (Radix behaviour).
    expect(document.activeElement).toBe(content);
    expect(content.querySelector('[data-slot="dropdown-menu-label"]')!.textContent).toBe('Signed in');
    (document.querySelector('#dd-out') as HTMLButtonElement).click();
    await settle();
    expect(fixture.componentInstance.signedOut()).toBe(true);
  });
});

describe('custom pieces', () => {
  @Component({
    imports: [StatTile, DismissibleNotice],
    template: `
      <ui-stat-tile label="Drafts" [value]="3" icon="Clock" href="/applications" />
      <ui-stat-tile label="Open" [value]="1" icon="Bell" [active]="true" (tileClick)="clicked.set(true)" />
      <ui-dismissible-notice noticeId="t1" title="Start here">Body</ui-dismissible-notice>`,
  })
  class Host {
    clicked = signal(false);
  }

  it('stat tile is a link with href, otherwise a pressed-state button', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const { el, fixture } = await render(Host);
    const link = el.querySelector('a') as HTMLAnchorElement;
    expect(link.getAttribute('href')).toBe('/applications');
    const button = el.querySelector('button[aria-pressed]') as HTMLButtonElement;
    expect(button.getAttribute('aria-pressed')).toBe('true');
    button.click();
    expect(fixture.componentInstance.clicked()).toBe(true);
  });

  it('dismissible notice shows after first paint and remembers dismissal', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
    const { el, settle } = await render(Host);
    const aside = el.querySelector('aside[aria-label="Start here"]') as HTMLElement;
    expect(aside).not.toBeNull();
    (aside.querySelector('button[aria-label="Dismiss: Start here"]') as HTMLButtonElement).click();
    await settle();
    expect(el.querySelector('aside[aria-label="Start here"]')).toBeNull();
    expect(localStorage.getItem('lcrb.notice.t1')).toBe('1');
  });

  it('use-toast keeps a store with no outlet', () => {
    const { toasts, toast } = TestBed.runInInjectionContext(() => injectToast());
    toast({ title: 'Receipt emailed' });
    expect(toasts()[0].title).toBe('Receipt emailed');
    expect(document.body.textContent).not.toContain('Receipt emailed');
  });
});
