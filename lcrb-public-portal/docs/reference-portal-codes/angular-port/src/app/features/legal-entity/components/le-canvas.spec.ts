import { Component, signal } from '@angular/core';
import { By } from '@angular/platform-browser';
import { TestBed } from '@angular/core/testing';
import { type ConnectionSettings, VflowComponent } from 'ngx-vflow';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { createSampleBuild } from '@data-access/repositories/legal-entity/mock/legal-entity';
import { type LegalEntityBuild } from '@domain/models/legal-entity';
import { LECanvas } from './le-canvas';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }, n = 4) => {
  for (let i = 0; i < n; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 5));
  }
  f.detectChanges();
};

class ResizeObserverStub {
  observe() {}
  unobserve() {}
  disconnect() {}
}

// jsdom has no SVG geometry: ngx-vflow measures handles with getBBox and d3-zoom reads the svg size.
function stubSvgGeometry() {
  (SVGElement.prototype as unknown as { getBBox: () => unknown }).getBBox = () => ({ x: 0, y: 0, width: 10, height: 10 });
  Object.defineProperty(SVGSVGElement.prototype, 'width', { configurable: true, get: () => ({ baseVal: { value: 900 } }) });
  Object.defineProperty(SVGSVGElement.prototype, 'height', { configurable: true, get: () => ({ baseVal: { value: 600 } }) });
}

@Component({
  imports: [LECanvas],
  template: `<le-canvas
    [build]="build()"
    [readOnly]="readOnly()"
    [selectedId]="selectedId()"
    (selectNode)="selected.push($event)"
    (connectNodes)="connected.push($event)"
    (positionChange)="moved.push($event)"
  />`,
})
class Host {
  build = signal<LegalEntityBuild>(createSampleBuild('Harding Hospitality Group Ltd.', 'BC0987654'));
  readOnly = signal(false);
  selectedId = signal<string | null>(null);
  selected: (string | null)[] = [];
  connected: { source: string; target: string }[] = [];
  moved: { nodeId: string; position: { x: number; y: number } }[] = [];
}

async function mount() {
  const fixture = TestBed.createComponent(Host);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  const el = fixture.nativeElement as HTMLElement;
  return { fixture, host: fixture.componentInstance, el, next: () => settle(fixture) };
}

const card = (el: HTMLElement, name: string) =>
  [...el.querySelectorAll('le-entity-node > div, le-person-node > div')].find((d) => d.querySelector('p.truncate')?.getAttribute('title') === name) as HTMLElement;
/** The canvas's protected members the vflow events are routed to. */
interface CanvasInternals {
  connection: ConnectionSettings;
  handleConnect(c: { source: string; target: string }): void;
  handleDragEnd(e: { node: unknown }): void;
}
const canvas = (fixture: { debugElement: { query: (p: never) => { componentInstance: unknown } } }) =>
  fixture.debugElement.query(By.directive(LECanvas) as never).componentInstance as CanvasInternals;
const classes = (el: Element, tokens: string) => tokens.split(' ').every((t) => el.classList.contains(t));
const edgePaths = (el: HTMLElement) => [...el.querySelectorAll('path.react-flow__edge-path')] as SVGPathElement[];

describe('LECanvas', () => {
  beforeEach(() => {
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    stubSvgGeometry();
  });
  afterEach(() => {
    vi.restoreAllMocks();
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders a card per node with its header, identity facts and people grouped by role', async () => {
    const { el } = await mount();
    expect(el.querySelectorAll('g.vflow-node').length).toBe(6);
    expect(el.querySelectorAll('le-entity-node').length).toBe(4);
    expect(el.querySelectorAll('le-person-node').length).toBe(2);
    const applicant = card(el, 'Harding Hospitality Group Ltd.');
    expect(classes(applicant, 'w-[300px] overflow-hidden rounded-lg border bg-card shadow-sm')).toBe(true);
    expect(applicant.textContent).toContain('Applicant Legal Entity');
    expect(applicant.textContent).toContain('BN BC0987654');
    expect(applicant.querySelector('svg[aria-label="Locked identity"]')).not.toBeNull();
    // Roles are grouped in the prototype's display order (Directors before Shareholders ...).
    const roles = [...card(el, 'Blake Family Trust').querySelectorAll('p.text-\\[9px\\]')].map((p) => p.textContent!.trim());
    expect(roles).toEqual(['Trustee', 'Beneficiary']);
    const person = card(el, 'Casey Blake');
    expect(person.classList.contains('w-[210px]')).toBe(true);
    expect(person.querySelector('[aria-hidden="true"]')!.textContent!.trim()).toBe('CB');
  });

  it('draws one smooth-step edge per relationship with its colour, dash, marker and label', async () => {
    const { el } = await mount();
    const paths = edgePaths(el);
    expect(paths.length).toBe(5);
    const byColour = (c: string) => paths.filter((p) => p.getAttribute('stroke') === c);
    // Confirmed (record) shareholdings are solid; suggested ones are dashed and animated.
    const confirmed = byColour('#38598A').filter((p) => !p.style.strokeDasharray);
    expect(confirmed.length).toBe(2);
    expect(confirmed.every((p) => !p.classList.contains('animated'))).toBe(true);
    const suggested = paths.filter((p) => p.style.strokeDasharray === '6 4');
    expect(suggested.length).toBe(3);
    expect(suggested.every((p) => p.classList.contains('animated'))).toBe(true);
    expect(paths.every((p) => p.getAttribute('stroke-width') === '2' && p.getAttribute('marker-end')!.startsWith('url(#'))).toBe(true);
    // One marker per distinct edge colour.
    expect(el.querySelectorAll('marker polyline').length).toBe(3);
    const labels = [...el.querySelectorAll('.react-flow__edge-text')].map((l) => l.textContent!.trim());
    expect(labels).toContain('Doing Business As');
    expect(labels).toContain('Family Connection');
    expect(labels.filter((l) => l.endsWith('% Shares')).length).toBe(3);
    expect((el.querySelector('.react-flow__edge-text') as HTMLElement).style.fontWeight).toBe('600');
  });

  it('shows a dotted teal style and a caption for Tied House mirror cards and edges', async () => {
    const { host, el, next } = await mount();
    const b = host.build();
    host.build.set({
      ...b,
      updatedAt: 'later',
      nodes: [
        ...b.nodes,
        {
          id: 'th_1',
          type: 'private-corporation',
          legalName: 'Mirror Brewing Ltd.',
          linkedLicenceIds: [],
          people: [],
          position: { x: 900, y: 0 },
          source: 'record',
          locked: true,
          external: 'tied-house',
        },
      ],
      edges: [...b.edges, { id: 'th_e', source: b.nodes[0].id, target: 'th_1', type: 'connection', locked: true, external: 'tied-house' }],
    });
    await next();
    const mirror = card(el, 'Mirror Brewing Ltd.');
    expect(classes(mirror, 'border-dashed border-[#0F766E]/60')).toBe(true);
    expect(mirror.textContent).toContain('Tied House association');
    expect(mirror.querySelector('svg[aria-label="From Tied House"]')).not.toBeNull();
    const teal = edgePaths(el).find((p) => p.getAttribute('stroke') === '#0F766E')!;
    expect(teal.style.strokeDasharray).toBe('2 4');
    expect(teal.classList.contains('animated')).toBe(false);
    // A mirror card can never be dragged or connected, even in an editable build.
    const handles = [...mirror.closest('g.vflow-node')!.querySelectorAll('circle[stroke="#ffffff"]')] as SVGElement[];
    expect(handles.every((c) => c.style.pointerEvents === 'none')).toBe(true);
    expect(mirror.closest('g.vflow-node')!.classList.contains('vflow-node--undraggable')).toBe(true);
  });

  it('highlights the selected card and reports clicks on a card or on the empty pane', async () => {
    const { host, el, next } = await mount();
    host.selectedId.set(host.build().nodes[1].id);
    await next();
    expect(classes(card(el, host.build().nodes[1].legalName), 'ring-2 ring-[#013366] ring-offset-2')).toBe(true);
    expect(card(el, host.build().nodes[2].legalName).classList.contains('hover:shadow-md')).toBe(true);

    card(el, 'Casey Blake').click();
    await next();
    expect(host.selected).toEqual([host.build().nodes.find((n) => n.legalName === 'Casey Blake')!.id]);

    (el.querySelector('[data-testid="rf__wrapper"]') as HTMLElement).click();
    expect(host.selected.at(-1)).toBeNull();

    // Panels and edges are not the pane.
    host.selected.length = 0;
    (el.querySelector('.react-flow__controls-fitview') as HTMLElement).click();
    edgePaths(el)[0].dispatchEvent(new MouseEvent('click', { bubbles: true }));
    expect(host.selected).toEqual([]);
  });

  it('proposes a relationship when a connection is drawn, but not in a locked build', async () => {
    const { fixture, host, el, next } = await mount();
    const c = canvas(fixture);
    c.handleConnect({ source: 'a', target: 'b' });
    expect(host.connected).toEqual([{ source: 'a', target: 'b' }]);
    // ngx-vflow only emits a connection the validator accepts: both cards must be connectable.
    const [a, b] = host.build().nodes;
    const valid = (source: string, target: string) => c.connection.validator!({ source, target } as never);
    expect(valid(a.id, b.id)).toBe(true);
    expect(c.connection.allowSelfConnections).toBe(true);
    host.readOnly.set(true);
    await next();
    expect(valid(a.id, b.id)).toBe(false);
    c.handleConnect({ source: 'c', target: 'd' });
    expect(host.connected.length).toBe(1);
    expect(el.querySelectorAll('g.vflow-node--undraggable').length).toBe(6);
  });

  it('persists a position only when a card was moved', async () => {
    const { fixture, host } = await mount();
    const flow = fixture.debugElement.query(By.directive(VflowComponent)).componentInstance as VflowComponent;
    const vnode = flow.getNode(host.build().nodes[1].id)!;
    canvas(fixture).handleDragEnd({ node: vnode });
    expect(host.moved).toEqual([]);
    (vnode.point as { set(p: { x: number; y: number }): void }).set({ x: 11, y: 22 });
    canvas(fixture).handleDragEnd({ node: vnode });
    expect(host.moved).toEqual([{ nodeId: host.build().nodes[1].id, position: { x: 11, y: 22 } }]);
  });

  it('resyncs positions from the build when it is updated', async () => {
    const { fixture, host, next } = await mount();
    const flow = fixture.debugElement.query(By.directive(VflowComponent)).componentInstance as VflowComponent;
    const b = host.build();
    host.build.set({ ...b, updatedAt: 'later', nodes: b.nodes.map((n, i) => (i === 1 ? { ...n, position: { x: 5, y: 6 } } : n)) });
    await next();
    expect(flow.getNode(b.nodes[1].id)!.point!()).toEqual({ x: 5, y: 6 });
  });

  it('has Controls with zoom in, zoom out and fit view (no interactive toggle)', async () => {
    const { fixture, el } = await mount();
    const flow = fixture.debugElement.query(By.directive(VflowComponent)).componentInstance as VflowComponent;
    const panel = el.querySelector('.react-flow__controls')!;
    expect(panel.className).toBe('react-flow__panel react-flow__controls vertical bottom left');
    expect(panel.getAttribute('aria-label')).toBe('Control Panel');
    const buttons = [...panel.querySelectorAll('button')];
    expect(buttons.map((b) => b.getAttribute('aria-label'))).toEqual(['Zoom In', 'Zoom Out', 'Fit View']);
    expect(buttons.map((b) => b.className)).toEqual([
      'react-flow__controls-button react-flow__controls-zoomin',
      'react-flow__controls-button react-flow__controls-zoomout',
      'react-flow__controls-button react-flow__controls-fitview',
    ]);
    const viewportTo = vi.spyOn(flow, 'viewportTo').mockImplementation(() => undefined);
    const fitView = vi.spyOn(flow, 'fitView').mockImplementation(() => undefined);
    buttons[0].click();
    expect(viewportTo.mock.calls[0][0].zoom).toBeCloseTo(1.2);
    buttons[1].click();
    expect(viewportTo.mock.calls[1][0].zoom).toBeCloseTo(1 / 1.2);
    buttons[2].click();
    expect(fitView).toHaveBeenCalledWith({ padding: 0.2 });
  });

  it('has the entity-type legend panel', async () => {
    const { el } = await mount();
    const panel = el.querySelector('.react-flow__panel.top.left')!;
    expect(panel.className).toBe('react-flow__panel rounded-md border bg-card/95 p-2 shadow-sm top left');
    expect(panel.textContent).toContain('Entity types');
    for (const t of ['Applicant', 'Private Corp', 'Public Corp', 'Partnership', 'LP', 'Trust', 'Society', 'Sole Prop', 'Individual']) {
      expect(panel.textContent).toContain(t);
    }
    expect(panel.textContent).toContain('Solid line = confirmed · dashed = suggested');
    expect(panel.textContent).toContain('Tied House association (read-only)');
  });

  it('has a MiniMap coloured by entity type', async () => {
    const { el } = await mount();
    const mini = el.querySelector('.react-flow__minimap')!;
    expect(mini.className).toBe('react-flow__panel react-flow__minimap !bg-card bottom right');
    const svg = mini.querySelector('svg.react-flow__minimap-svg')!;
    expect(svg.getAttribute('width')).toBe('200');
    expect(svg.getAttribute('height')).toBe('150');
    expect(svg.querySelector('title')!.textContent).toBe('Mini Map');
    const fills = [...svg.querySelectorAll('rect.react-flow__minimap-node')].map((r) => (r as SVGElement).style.fill);
    expect(fills.length).toBe(6);
    expect(fills).toContain('rgb(46, 125, 87)');
    expect(svg.querySelector('path.react-flow__minimap-mask')!.getAttribute('fill-rule')).toBe('evenodd');
  });

  it('draws the dots background (gap 20, size 1, #cbd5e1)', async () => {
    const { el } = await mount();
    const pattern = el.querySelector('pattern')!;
    expect(pattern.getAttribute('width')).toBe('20');
    expect(pattern.querySelector('circle')!.getAttribute('fill')).toBe('#cbd5e1');
    expect(pattern.querySelector('circle')!.getAttribute('r')).toBe('0.5');
  });
});
