import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { CHECK_OUTCOME_META, QUEUE_META } from '@domain/staff/agent-log';
import { OutcomeBadge, QueueBadge } from './queue-badge';

const hasClasses = (el: Element, classes: string) => classes.split(' ').every((c) => el.classList.contains(c));

@Component({
  imports: [QueueBadge, OutcomeBadge],
  template: `
    <queue-badge queue="beyond-licence-cap" className="ml-2" />
    <queue-badge queue="section-119-review" />
    <queue-badge queue="enhanced-review" />
    <queue-badge queue="standard-processing" />
    <outcome-badge outcome="clear" />
    <outcome-badge outcome="attention" />
    <outcome-badge outcome="flag" />
  `,
})
class Host {}

describe('QueueBadge and OutcomeBadge', () => {
  it('render the meta label with the tone classes and an extra className', () => {
    const fixture = TestBed.createComponent(Host);
    fixture.detectChanges();
    const spans = [...(fixture.nativeElement as HTMLElement).querySelectorAll('span')];
    expect(spans.map((s) => s.textContent)).toEqual([
      QUEUE_META['beyond-licence-cap'].label,
      QUEUE_META['section-119-review'].label,
      QUEUE_META['enhanced-review'].label,
      QUEUE_META['standard-processing'].label,
      CHECK_OUTCOME_META.clear.label,
      CHECK_OUTCOME_META.attention.label,
      CHECK_OUTCOME_META.flag.label,
    ]);
    const tone = QUEUE_META['beyond-licence-cap'].tone;
    expect(hasClasses(spans[0], 'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium')).toBe(true);
    expect(hasClasses(spans[0], tone === 'danger' ? 'bg-red-100 text-red-800 border-red-200' : 'border')).toBe(true);
    expect(spans[0].classList.contains('ml-2')).toBe(true);
    expect(hasClasses(spans[4], 'bg-green-100 text-green-800 border-green-200')).toBe(true);
    expect(hasClasses(spans[5], 'bg-orange-100 text-orange-800 border-orange-200')).toBe(true);
    expect(hasClasses(spans[6], 'bg-red-100 text-red-800 border-red-200')).toBe(true);
    expect(hasClasses(spans[4], 'px-2 py-0.5')).toBe(true);
  });
});
