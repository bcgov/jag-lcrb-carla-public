import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { setAutoApprovalEnabled } from '@data-access/repositories/special-events/sep-auto-approval';
import { MIN_NOTICE_DAYS } from '@domain/special-events/auto-approval';
import { AutoApprovalStatusCard } from './auto-approval-status-card';

async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  const fixture = TestBed.createComponent(AutoApprovalStatusCard);
  document.body.appendChild(fixture.nativeElement);
  fixture.detectChanges();
  await fixture.whenStable();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('AutoApprovalStatusCard', () => {
  beforeEach(() => localStorage.clear());
  afterEach(() => {
    setAutoApprovalEnabled(false, 'Test', 'Agency');
    document.body.innerHTML = '';
  });

  it('shows the Off state by default with the account settings link', async () => {
    const { el } = await mount();
    expect(el.textContent).toContain('SEP auto-approval');
    expect(el.textContent).toContain('Off — every permit is referred to an officer.');
    expect(el.querySelector('[data-slot="badge"]')!.textContent!.trim()).toBe('Off');
    expect(el.querySelector('a')!.getAttribute('href')).toBe('/account');
    expect(el.querySelector('a')!.textContent).toContain('Manage in Account Settings');
    expect(el.querySelector('svg.lucide-shield-off')).not.toBeNull();
  });

  it('switches to the On state when the policy is enabled', async () => {
    const { fixture, el } = await mount();
    setAutoApprovalEnabled(true, 'Cst. Test', 'Agency');
    fixture.detectChanges();
    await fixture.whenStable();
    expect(el.textContent).toContain(`On — eligible permits filed ${MIN_NOTICE_DAYS}+ days ahead are approved automatically.`);
    expect(el.querySelector('[data-slot="badge"]')!.textContent!.trim()).toBe('On');
    expect(el.querySelector('svg.lucide-shield-check')).not.toBeNull();
  });
});
