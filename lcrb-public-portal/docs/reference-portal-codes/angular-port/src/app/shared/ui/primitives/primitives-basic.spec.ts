/** Styling-only primitives: data-slot attributes, variants and the `cn(base, className)` merge (a consumer class overrides a conflicting base class). */
import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Badge } from '@shared/ui/primitives/badge';
import { Button, buttonVariants } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { ALERT } from '@shared/ui/primitives/alert';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { Separator } from '@shared/ui/primitives/separator';
import { TABLE } from '@shared/ui/primitives/table';
import { Textarea } from '@shared/ui/primitives/textarea';

@Component({
  imports: [Button, Badge, ...CARD, ...ALERT, Input, Label, Textarea, Separator, ...TABLE],
  template: `
    <button uiButton id="b1">Default</button>
    <button uiButton id="b2" variant="outline" size="sm" class="px-2 text-red-600">Outline</button>
    <a uiButton id="b3" [variant]="variant()" href="/x" [class]="dynamicClass()">Link</a>
    <span uiBadge id="badge" variant="secondary">Badge</span>
    <div uiCard id="card"><div uiCardHeader><div uiCardTitle>T</div></div><div uiCardContent class="px-4">C</div></div>
    <div uiAlert id="alert" variant="destructive"><div uiAlertDescription>D</div></div>
    <input uiInput id="input" />
    <label uiLabel id="label">L</label>
    <textarea uiTextarea id="textarea"></textarea>
    <div uiSeparator id="sep"></div>
    <div uiSeparator id="vsep" orientation="vertical" [decorative]="false"></div>
    <ui-table class="min-w-full"><thead uiTableHeader><tr uiTableRow><th uiTableHead>H</th></tr></thead>
      <tbody uiTableBody><tr uiTableRow><td uiTableCell id="cell">V</td></tr></tbody></ui-table>
  `,
})
class Host {
  variant = signal<'ghost' | 'link'>('ghost');
  dynamicClass = signal('w-full');
}

function el(id: string): HTMLElement {
  return document.getElementById(id) as HTMLElement;
}

/** Class order in the attribute does not affect CSS; Angular writes bound classes sorted. */
const classSet = (value: string) => [...new Set(value.split(/\s+/).filter(Boolean))].sort();

describe('styling-only primitives', () => {
  let fixture: ReturnType<typeof TestBed.createComponent<Host>>;
  beforeEach(async () => {
    fixture = TestBed.createComponent(Host);
    document.body.appendChild(fixture.nativeElement);
    fixture.detectChanges();
    await fixture.whenStable();
  });
  afterEach(() => fixture.nativeElement.remove());

  it('button renders the default variant classes and data-slot', () => {
    expect(el('b1').getAttribute('data-slot')).toBe('button');
    expect(classSet(el('b1').className)).toEqual(classSet(buttonVariants({ variant: 'default', size: 'default' })));
  });

  it('a consumer class overrides conflicting base classes (tailwind-merge)', () => {
    const classes = el('b2').className.split(' ');
    expect(classes).toContain('px-2');
    expect(classes).not.toContain('px-3');
    expect(classes).toContain('text-red-600');
    expect(classes).toContain('h-8'); // size sm
    expect(classes).toContain('border'); // variant outline
  });

  it('bound variant and class update the host', async () => {
    expect(el('b3').className).toContain('hover:bg-accent');
    fixture.componentInstance.variant.set('link');
    fixture.componentInstance.dynamicClass.set('w-auto');
    fixture.detectChanges();
    await fixture.whenStable();
    expect(el('b3').className).toContain('underline-offset-4');
    expect(el('b3').className).toContain('w-auto');
    expect(el('b3').className).not.toContain('w-full');
  });

  it('badge, card, alert, input, label, textarea carry their slots and classes', () => {
    expect(el('badge').className).toContain('bg-secondary');
    expect(el('card').getAttribute('data-slot')).toBe('card');
    expect(el('card').querySelector('[data-slot="card-content"]')!.className).toBe('px-4');
    expect(el('alert').getAttribute('role')).toBe('alert');
    expect(el('alert').className).toContain('text-destructive');
    expect(el('input').getAttribute('data-slot')).toBe('input');
    expect(el('label').className).toContain('select-none');
    expect(el('textarea').className).toContain('field-sizing-content');
  });

  it('separator follows Radix: decorative role none, orientation attributes', () => {
    expect(el('sep').getAttribute('role')).toBe('none');
    expect(el('sep').getAttribute('data-orientation')).toBe('horizontal');
    expect(el('vsep').getAttribute('role')).toBe('separator');
    expect(el('vsep').getAttribute('aria-orientation')).toBe('vertical');
  });

  it('table wraps the table in the container div and merges the table class', () => {
    const table = fixture.nativeElement.querySelector('table') as HTMLTableElement;
    expect(table.parentElement!.getAttribute('data-slot')).toBe('table-container');
    expect(classSet(table.className)).toEqual(classSet('w-full caption-bottom text-sm min-w-full'));
    expect(el('cell').getAttribute('data-slot')).toBe('table-cell');
  });
});
