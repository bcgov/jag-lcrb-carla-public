/** the public complaint dialog submits and closes itself after the confirmation. */
import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it } from 'vitest';
import { ComplaintDialog } from './complaint-dialog';
import { allLicenses } from '@data-access/repositories/map/mock/license-data';

const tick = () => new Promise((r) => setTimeout(r, 0));

@Component({
  imports: [ComplaintDialog],
  template: `<complaint-dialog [open]="open()" (openChange)="open.set($event)" [license]="license" />`,
})
class Host {
  open = signal(true);
  license = allLicenses.find((l) => l.status === 'licensed')!;
}

afterEach(() => document.body.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = '')));

const type = (input: HTMLInputElement | HTMLTextAreaElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input', { bubbles: true }));
};

describe('ComplaintDialog', () => {
  it('shows the form, blocks submit without a type, then submits, confirms and closes', async () => {
    const fixture = TestBed.createComponent(Host);
    document.body.appendChild(fixture.nativeElement);
    const settle = async () => {
      fixture.detectChanges();
      await fixture.whenStable();
      await tick();
      fixture.detectChanges();
    };
    await settle();
    const overlay = document.querySelector('.cdk-overlay-container')!;
    expect(overlay.textContent).toContain('File a Complaint');
    expect(overlay.textContent).toContain(`Submit a complaint about ${fixture.componentInstance.license.businessName}`);

    type(overlay.querySelector('#name') as HTMLInputElement, 'Pat Lee');
    type(overlay.querySelector('#email') as HTMLInputElement, 'pat@example.com');
    type(overlay.querySelector('#description') as HTMLTextAreaElement, 'Loud music after hours');
    const submit = overlay.querySelector('button[type="submit"]') as HTMLButtonElement;
    submit.click();
    await settle();
    expect(overlay.textContent).not.toContain('Complaint Submitted');

    (overlay.querySelector('#type') as HTMLButtonElement).click();
    await settle();
    const options = Array.from(document.querySelectorAll('.cdk-overlay-container [role="option"]')) as HTMLElement[];
    expect(options.map((o) => o.textContent?.trim())).toContain('Noise Disturbance');
    options[0].click();
    await settle();
    submit.click();
    await settle();
    expect(overlay.textContent).toContain('Complaint Submitted');
    expect(overlay.textContent).toContain('A reference number will be sent to your email.');

    await new Promise((r) => setTimeout(r, 2100));
    await settle();
    expect(fixture.componentInstance.open()).toBe(false);
  });

  it('Cancel closes without submitting', async () => {
    const fixture = TestBed.createComponent(Host);
    document.body.appendChild(fixture.nativeElement);
    fixture.detectChanges();
    await fixture.whenStable();
    await tick();
    fixture.detectChanges();
    const cancel = Array.from(document.querySelectorAll('.cdk-overlay-container button')).find((b) => b.textContent?.trim() === 'Cancel') as HTMLButtonElement;
    cancel.click();
    expect(fixture.componentInstance.open()).toBe(false);
  });
});
