/** Renders a lucide icon on the <svg> itself with lucide-react's attributes so `[&_svg]` selectors work; only icons registered in ./icons are available. */
import { Directive, effect, ElementRef, inject, input, numberAttribute, Renderer2 } from '@angular/core';
import { ICONS, type IconName } from './icons';

const SVG_NS = 'http://www.w3.org/2000/svg';

/** lucide-react's toKebabCase(toPascalCase(name)), used for its first icon class. */
function lucideReactClassName(kebab: string): string {
  const pascal = kebab.replace(/^([A-Z])|[\s-_]+(\w)/g, (_m, a: string, b: string) => (b ? b.toUpperCase() : a.toLowerCase()));
  const pascalCased = pascal.charAt(0).toUpperCase() + pascal.slice(1);
  return pascalCased.replace(/([a-z0-9])([A-Z])/g, '$1-$2').toLowerCase();
}

/** lucide-react's mergeClasses: drop empties and duplicates, keep order. */
function mergeClasses(...classes: (string | undefined)[]): string {
  return classes
    .filter((c, i, all) => Boolean(c) && c!.trim() !== '' && all.indexOf(c) === i)
    .join(' ')
    .trim();
}

@Directive({ selector: 'svg[lucide]' })
export class LucideIcon {
  readonly lucide = input.required<IconName>();
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly size = input(24, { transform: numberAttribute });
  readonly strokeWidth = input(2, { transform: numberAttribute });

  private readonly el = inject<ElementRef<SVGSVGElement>>(ElementRef).nativeElement;
  private readonly renderer = inject(Renderer2);

  constructor() {
    const hasA11y = Array.from(this.el.attributes).some(
      (a) => a.name.startsWith('aria-') || a.name === 'role' || a.name === 'title',
    );
    effect(() => {
      const [kebab, node] = ICONS[this.lucide()];
      const svg = this.el;
      const attrs: Record<string, string> = {
        xmlns: SVG_NS,
        width: String(this.size()),
        height: String(this.size()),
        viewBox: '0 0 24 24',
        fill: 'none',
        stroke: 'currentColor',
        'stroke-width': String(this.strokeWidth()),
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        class: mergeClasses('lucide', mergeClasses(`lucide-${lucideReactClassName(kebab)}`, `lucide-${kebab}`, this.userClass())),
      };
      if (!hasA11y) attrs['aria-hidden'] = 'true';
      for (const [k, v] of Object.entries(attrs)) this.renderer.setAttribute(svg, k, v);
      while (svg.firstChild) svg.removeChild(svg.firstChild);
      for (const [tag, childAttrs] of node) {
        const child = document.createElementNS(SVG_NS, tag);
        for (const [k, v] of Object.entries(childAttrs)) child.setAttribute(k, String(v));
        svg.appendChild(child);
      }
    });
  }
}

/** Exposed for tests: the class lucide-react would put on an icon. */
export const lucideClassFor = (kebab: string, className?: string) =>
  mergeClasses('lucide', mergeClasses(`lucide-${lucideReactClassName(kebab)}`, `lucide-${kebab}`, className));

