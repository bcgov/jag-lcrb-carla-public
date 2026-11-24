import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computeProcessingTimes } from '@domain/applications/processing-times';
import { ProcessingTimesBanner } from './processing-times-banner';

async function mount(focusType?: string) {
  const fixture = TestBed.createComponent(ProcessingTimesBanner);
  if (focusType) fixture.componentRef.setInput('focusType', focusType);
  fixture.detectChanges();
  await fixture.whenStable();
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('ProcessingTimesBanner', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.useRealTimers();
    vi.unstubAllGlobals();
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('shows the portal-wide median and up to four representative types when not focused', async () => {
    const { el } = await mount();
    const result = computeProcessingTimes();
    const section = el.querySelector('section')!;
    expect(section.getAttribute('aria-label')).toBe('Current application processing times');
    expect(section.className).toBe('rounded-lg border border-border bg-muted/40 p-4');
    expect(el.querySelector('h2')!.textContent).toContain('Current processing times');
    expect(el.textContent).toContain('From the LCRB analytics engine · updated');
    expect(el.textContent).toContain(`Portal-wide median ${result.overallMedianDays} days`);
    const items = el.querySelectorAll('ul > li');
    expect(items.length).toBe(Math.min(4, result.byLicenceType.filter((d) => d.sampleSize >= 10).length));
    expect(el.querySelector('ul')!.className).toBe('mt-3 grid grid-cols-2 gap-x-4 gap-y-2 sm:grid-cols-4');
    const medians = [...items].map((li) => Number(/~(\d+) days/.exec(li.textContent!)![1]));
    expect(medians).toEqual([...medians].sort((a, b) => a - b));
  });

  it('highlights the focus licence type with its typical and p90 days and a trend pill', async () => {
    const datum = computeProcessingTimes().byLicenceType[0];
    const { el } = await mount(datum.licenceType);
    expect(el.textContent).toContain(datum.label);
    expect(el.textContent).toContain(`Typically ~${datum.medianDays} days · most within ${datum.p90Days} days`);
    expect(el.querySelector('ul')).toBeNull();
    expect(el.textContent).not.toContain('Portal-wide median');
    const pill = el.querySelector('span.inline-flex.items-center.gap-1.text-xs.font-medium')!;
    expect(pill.textContent!.trim()).toMatch(/Faster than usual|Slower than usual|Steady/);
    expect(pill.querySelector('svg')!.getAttribute('class')).toContain('h-3.5 w-3.5');
  });

  it('shows the explanation tooltip after 150 ms on hover', async () => {
    const { fixture, el } = await mount();
    const trigger = el.querySelector('button[aria-label="How processing times are calculated"]') as HTMLButtonElement;
    expect(trigger.className).toBe('text-muted-foreground hover:text-foreground');
    trigger.dispatchEvent(new Event('pointerenter'));
    await new Promise((r) => setTimeout(r, 250));
    fixture.detectChanges();
    const tip = document.querySelector('[data-slot="tooltip-content"]')!;
    expect(tip.textContent).toContain('Live estimates from applications decided in the last');
    expect(tip.className).toContain('max-w-xs');
  });
});
