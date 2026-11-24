/** Controlled primitives behave like Radix: a bound value only emits the change, an unbound one keeps its own state, and `[(x)]` updates the parent. */
import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { Checkbox } from './checkbox';
import { RADIO_GROUP } from './radio-group';
import { SELECT } from './select';
import { Switch } from './switch';

@Component({
  imports: [Switch, Checkbox, ...RADIO_GROUP],
  template: `
    <button uiSwitch id="sw-fixed" [checked]="false" (checkedChange)="swEvents.push($event)"></button>
    <button uiSwitch id="sw-free" defaultChecked (checkedChange)="swEvents.push($event)"></button>
    <button uiSwitch id="sw-two" [(checked)]="on"></button>
    <button uiCheckbox id="cb-fixed" [checked]="true" (checkedChange)="cbEvents.push($event)"></button>
    <button uiCheckbox id="cb-two" [(checked)]="ticked"></button>
    <div uiRadioGroup id="rg-fixed" value="a" (valueChange)="rgEvents.push($event)">
      <button uiRadioGroupItem value="a"></button>
      <button uiRadioGroupItem value="b"></button>
    </div>
    <div uiRadioGroup id="rg-free" defaultValue="a">
      <button uiRadioGroupItem value="a"></button>
      <button uiRadioGroupItem value="b"></button>
    </div>
  `,
})
class Host {
  on = signal(false);
  ticked = signal(false);
  swEvents: boolean[] = [];
  cbEvents: boolean[] = [];
  rgEvents: (string | undefined)[] = [];
}

async function setup() {
  const fixture = TestBed.createComponent(Host);
  await fixture.whenStable();
  const el = fixture.nativeElement as HTMLElement;
  const click = async (sel: string) => {
    (el.querySelector(sel) as HTMLElement).click();
    fixture.detectChanges();
    await fixture.whenStable();
  };
  return { fixture, el, click, host: fixture.componentInstance };
}

describe('controlled primitives', () => {
  it('a bound switch only emits; the parent decides', async () => {
    const { el, click, host } = await setup();
    await click('#sw-fixed');
    expect(host.swEvents).toEqual([true]);
    expect(el.querySelector('#sw-fixed')!.getAttribute('aria-checked')).toBe('false');
  });

  it('an unbound switch keeps its own state from defaultChecked', async () => {
    const { el, click, host } = await setup();
    expect(el.querySelector('#sw-free')!.getAttribute('data-state')).toBe('checked');
    await click('#sw-free');
    expect(el.querySelector('#sw-free')!.getAttribute('data-state')).toBe('unchecked');
    expect(host.swEvents).toEqual([false]);
  });

  it('two-way binding updates the parent signal (switch and checkbox)', async () => {
    const { el, click, host } = await setup();
    await click('#sw-two');
    expect(host.on()).toBe(true);
    expect(el.querySelector('#sw-two')!.getAttribute('aria-checked')).toBe('true');
    await click('#cb-two');
    expect(host.ticked()).toBe(true);
    expect(el.querySelector('#cb-two')!.getAttribute('data-state')).toBe('checked');
  });

  it('a bound checkbox stays as bound and emits the requested value', async () => {
    const { el, click, host } = await setup();
    await click('#cb-fixed');
    expect(host.cbEvents).toEqual([false]);
    expect(el.querySelector('#cb-fixed')!.getAttribute('aria-checked')).toBe('true');
  });

  it('updates the DOM before the click returns, like React (two-way binding)', async () => {
    const { el } = await setup();
    const cb = el.querySelector('#cb-two') as HTMLButtonElement;
    cb.click();
    // No detectChanges / whenStable here: the state must already show.
    expect(cb.getAttribute('aria-checked')).toBe('true');
    const sw = el.querySelector('#sw-two') as HTMLButtonElement;
    sw.click();
    expect(sw.getAttribute('aria-checked')).toBe('true');
  });

  it('radio groups: bound emits only, unbound moves itself', async () => {
    const { el, click, host } = await setup();
    await click('#rg-fixed button[value="b"]');
    expect(host.rgEvents).toEqual(['b']);
    expect(el.querySelector('#rg-fixed button[value="a"]')!.getAttribute('aria-checked')).toBe('true');
    await click('#rg-free button[value="b"]');
    expect(el.querySelector('#rg-free button[value="b"]')!.getAttribute('aria-checked')).toBe('true');
    // Choosing the current value again emits nothing (Radix only reports changes).
    await click('#rg-fixed button[value="a"]');
    expect(host.rgEvents).toEqual(['b']);
  });
});

@Component({
  imports: [...SELECT],
  template: `
    <ui-select [value]="'1'" (valueChange)="events.push($event)">
      <button uiSelectTrigger id="sel"><span uiSelectValue placeholder="Pick"></span></button>
      <ng-template uiSelectContent>
        <ui-select-item value="1">One</ui-select-item>
        <ui-select-item value="2">Two</ui-select-item>
      </ng-template>
    </ui-select>
  `,
})
class SelectHost {
  events: (string | undefined)[] = [];
}

describe('controlled select', () => {
  it('a bound select emits the choice and keeps showing the bound value', async () => {
    const fixture = TestBed.createComponent(SelectHost);
    await fixture.whenStable();
    fixture.detectChanges();
    const trigger = fixture.nativeElement.querySelector('#sel') as HTMLButtonElement;
    expect(trigger.textContent).toContain('One');
    trigger.click();
    fixture.detectChanges();
    await fixture.whenStable();
    const two = [...document.querySelectorAll('[role="option"]')].find((o) => o.textContent?.includes('Two')) as HTMLElement;
    two.click();
    fixture.detectChanges();
    await fixture.whenStable();
    expect(fixture.componentInstance.events).toEqual(['2']);
    expect(trigger.textContent).toContain('One');
  });
});
