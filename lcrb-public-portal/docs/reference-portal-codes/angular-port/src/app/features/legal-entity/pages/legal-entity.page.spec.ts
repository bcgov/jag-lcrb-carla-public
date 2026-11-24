import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { SessionProvider } from '@core/session/session-provider';
import type { DirectoryResult } from '@server-lib/data/entity-directory';
import type { LegalEntityBuild } from '@domain/models/legal-entity';
import { LegalEntityPage } from './legal-entity.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }, n = 4) => {
  for (let i = 0; i < n; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 5));
  }
  f.detectChanges();
};

const RESULTS: DirectoryResult[] = [
  {
    kind: 'business',
    id: 'dir-zed',
    sources: ['bc-registry'],
    legalName: 'Zed Brewing Ltd.',
    entityType: 'private-corporation',
    registrationNumber: 'BC9999999',
    status: 'Active',
    jurisdiction: 'British Columbia',
    licenceNumbers: [],
  },
  { kind: 'person', id: 'dir-pat', sources: ['dynamics'], name: 'Pat Lee', descriptor: 'Director of a licensed business' },
];

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

async function mount(role = 'licensee') {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role, firstName: 'Alex', lastName: 'Rivera', organization: 'Harding Hospitality Group Ltd.' } as never);
  const fixture = TestBed.createComponent(LegalEntityPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  const el = fixture.nativeElement as HTMLElement;
  const next = () => settle(fixture);
  return { fixture, el, next };
}

const buttons = (root: ParentNode) => [...root.querySelectorAll('button')] as HTMLButtonElement[];
const button = (root: ParentNode, text: string) => buttons(root).find((b) => b.textContent!.trim().includes(text));
const builds = () => JSON.parse(localStorage.getItem('lcrb.v2.legal-entity-builds') ?? '{}') as Record<string, LegalEntityBuild>;
const build = () => builds()['build_BC0987654'];
const nodeCard = (el: HTMLElement, name: string) =>
  [...el.querySelectorAll('le-entity-node > div, le-person-node > div')].find((d) => d.querySelector('p.truncate')?.getAttribute('title') === name) as HTMLElement;
const setValue = (input: HTMLInputElement | HTMLTextAreaElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input'));
};

describe('LegalEntityPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    vi.stubGlobal('ResizeObserver', ResizeObserverStub);
    stubSvgGeometry();
    stubFetch({ '/api/entity-directory': { json: { results: RESULTS } } });
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
    document.body.innerHTML = '';
  });

  it('shows the licensee-only notice to another role', async () => {
    const { el } = await mount('applicant');
    expect(el.textContent).toContain('Licensee tool');
    expect(el.textContent).toContain('available to licensee (business) accounts');
    expect(el.querySelector('a[href="/login"]')!.textContent).toContain('Switch account');
    expect(el.querySelector('le-canvas')).toBeNull();
  });

  it('renders the hero, toolbar, status cards and the demo staff actions', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Legal Entity Builder');
    expect(el.textContent).toContain('Harding Hospitality Group Ltd.');
    expect(el.textContent).toContain('(BN BC0987654)');
    expect(el.querySelector('page-hero span[uiBadge]')!.textContent).toContain('Draft');
    expect(button(el, 'Add entity')).toBeTruthy();
    expect(button(el, 'Add relationship')).toBeTruthy();
    expect(button(el, 'Submit to LCRB')).toBeTruthy();
    expect(el.querySelector('a[href="/applications/new/pcl-change"]')!.textContent).toContain('Report a change');
    expect(el.textContent).toContain('6 entities · 5 relationships');
    expect(el.textContent).toContain('Not yet synced');
    expect(el.textContent).toContain('No review notes yet.');
    expect(el.textContent).toContain('Changes you make are tracked here and flagged for LCRB confirmation.');
    expect(el.textContent).toContain('Demo: LCRB staff actions (Dynamics side)');
    expect(button(el, 'Lock & take over')).toBeTruthy();
    expect(el.querySelector('node-inspector')).toBeNull();
  });

  it('adds a new entity from the dialog, selects it and opens the inspector', async () => {
    const { el, next } = await mount();
    button(el, 'Add entity')!.click();
    await next();
    const dialog = document.querySelector('[role="dialog"]')!;
    expect(dialog.textContent).toContain('Add to your structure');
    button(dialog, 'New entity')!.click();
    await next();
    const add = button(dialog, 'Add entity')!;
    expect(add.disabled).toBe(true);
    setValue(dialog.querySelector('#ent-name')!, 'Maple Holdings Ltd.');
    await next();
    expect(add.disabled).toBe(false);
    add.click();
    await next();
    expect(document.querySelector('[role="dialog"]')).toBeNull();
    expect(build().nodes.some((n) => n.legalName === 'Maple Holdings Ltd.')).toBe(true);
    expect(el.textContent).toContain('7 entities');
    const inspector = el.querySelector('node-inspector')!;
    expect(inspector.querySelector('h2')!.textContent).toContain('Maple Holdings Ltd.');
    expect(inspector.textContent).toContain('Suggested');
    expect(nodeCard(el, 'Maple Holdings Ltd.').classList.contains('ring-[#013366]')).toBe(true);
    expect(el.textContent).toContain('Your recent changes');
  });

  it('searches records in the add dialog and adds a result without closing it', async () => {
    const { el, next } = await mount();
    button(el, 'Add entity')!.click();
    await next();
    await new Promise((r) => setTimeout(r, 300));
    await next();
    const dialog = document.querySelector('[role="dialog"]')!;
    const rows = [...dialog.querySelectorAll('.max-h-72 > div')];
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Zed Brewing Ltd.');
    expect(rows[0].textContent).toContain('Private Corp · BC9999999 · Active');
    expect(rows[1].textContent).toContain('Pat Lee');
    const first = rows[0].querySelector('button')!;
    expect(first.textContent!.trim()).toBe('Add');
    first.click();
    await next();
    expect(rows[0].querySelector('button')!.textContent).toContain('Added');
    expect(rows[0].querySelector('button')!.disabled).toBe(true);
    expect(button(dialog, 'Done · 1 added')).toBeTruthy();
    expect(build().nodes.some((n) => n.legalName === 'Zed Brewing Ltd.')).toBe(true);
    // A person is added by name; the dialog stays open for more.
    rows[1].querySelector('button')!.click();
    await next();
    expect(button(dialog, 'Done · 2 added')).toBeTruthy();
    expect(build().nodes.some((n) => n.legalName === 'Pat Lee')).toBe(true);
    expect(document.querySelector('[role="dialog"]')).not.toBeNull();
    button(dialog, 'Done · 2 added')!.click();
    await next();
    expect(document.querySelector('[role="dialog"]')).toBeNull();
  });

  it('selects a card, shows the inspector and closes it again', async () => {
    const { el, next } = await mount();
    nodeCard(el, 'Avery Holdings Ltd.').click();
    await next();
    const inspector = el.querySelector('node-inspector')!;
    expect(inspector.querySelector('h2')!.textContent).toContain('Avery Holdings Ltd.');
    expect(inspector.textContent).toContain('From LCRB records');
    expect(inspector.textContent).toContain('This card comes from LCRB systems of record.');
    expect(nodeCard(el, 'Avery Holdings Ltd.').classList.contains('ring-2')).toBe(true);
    (inspector.querySelector('button .sr-only')!.parentElement as HTMLElement).click();
    await next();
    expect(el.querySelector('node-inspector')).toBeNull();
    expect(nodeCard(el, 'Avery Holdings Ltd.').classList.contains('ring-2')).toBe(false);

    nodeCard(el, 'Casey Blake').click();
    await next();
    expect(el.querySelector('node-inspector')).not.toBeNull();
    (el.querySelector('[data-testid="rf__wrapper"]') as HTMLElement).click();
    await next();
    expect(el.querySelector('node-inspector')).toBeNull();
  });

  it('edits a portal-added card in the inspector: rename, note, people and removal', async () => {
    const { el, next } = await mount();
    button(el, 'Add entity')!.click();
    await next();
    let dialog = document.querySelector('[role="dialog"]')!;
    button(dialog, 'New entity')!.click();
    await next();
    setValue(dialog.querySelector('#ent-name')!, 'Maple Holdings Ltd.');
    await next();
    button(dialog, 'Add entity')!.click();
    await next();

    const inspector = () => el.querySelector('node-inspector')!;
    const name = inspector().querySelector('#insp-name') as HTMLInputElement;
    setValue(name, 'Maple Holdings Inc.');
    name.dispatchEvent(new Event('blur'));
    await next();
    expect(build().nodes.some((n) => n.legalName === 'Maple Holdings Inc.')).toBe(true);
    expect(inspector().querySelector('h2')!.textContent).toContain('Maple Holdings Inc.');

    const note = inspector().querySelector('textarea') as HTMLTextAreaElement;
    setValue(note, 'Holding company');
    note.dispatchEvent(new Event('blur'));
    await next();
    expect(build().nodes.find((n) => n.legalName === 'Maple Holdings Inc.')!.note).toBe('Holding company');

    const addPerson = button(inspector(), 'Add person')!;
    expect(addPerson.disabled).toBe(true);
    setValue(inspector().querySelector('input[placeholder="Add a person"]')!, 'Drew Park');
    await next();
    addPerson.click();
    await next();
    expect(build().nodes.find((n) => n.legalName === 'Maple Holdings Inc.')!.people.map((p) => p.name)).toEqual(['Drew Park']);
    expect(inspector().textContent).toContain('Drew Park');
    expect(inspector().textContent).toContain('Director');
    (button(inspector(), 'Remove Drew Park') as HTMLButtonElement).click();
    await next();
    expect(inspector().textContent).toContain('No people recorded yet.');

    button(inspector(), 'Remove this entity')!.click();
    await next();
    expect(build().nodes.some((n) => n.legalName === 'Maple Holdings Inc.')).toBe(false);
    expect(el.querySelector('node-inspector')).toBeNull();
    dialog = document.body;
    expect(el.textContent).toContain('6 entities');
  });

  it('opens the relationship dialog from the toolbar and adds a relationship', async () => {
    const { el, next } = await mount();
    button(el, 'Add relationship')!.click();
    await next();
    const dialog = document.querySelector('[role="dialog"]')!;
    expect(dialog.textContent).toContain('Add a relationship');
    const submit = button(dialog, 'Add relationship')!;
    expect(submit.disabled).toBe(true);
    const pick = async (id: string, text: string) => {
      (dialog.querySelector(`#${id}`) as HTMLButtonElement).click();
      await next();
      const option = [...document.querySelectorAll('.cdk-overlay-container [role="option"]')].filter((o) => o.textContent!.trim() === text).pop() as HTMLElement;
      option.click();
      await next();
    };
    await pick('rel-source', 'Avery Holdings Ltd.');
    await pick('rel-target', 'Casey Blake');
    expect(dialog.querySelector('#rel-source')!.textContent).toContain('Avery Holdings Ltd.');
    expect(dialog.querySelector('#rel-target')!.textContent).toContain('Casey Blake');
    expect(submit.disabled).toBe(false);
    // Shareholding shows the ownership percentage field.
    setValue(dialog.querySelector('#rel-share')!, '25');
    await next();
    submit.click();
    await next();
    const added = build().edges.find((e) => e.sharePercent === 25)!;
    expect(added.type).toBe('shareholding');
    expect(el.textContent).toContain('6 entities · 6 relationships');
    expect(document.querySelector('[role="dialog"]')).toBeNull();
  });

  it('is read-only once the LCRB locks the build, and writable again on unlock', async () => {
    const { el, next } = await mount();
    button(el, 'Lock & take over')!.click();
    await next();
    expect(el.textContent).toContain('This build is locked by the LCRB');
    expect(button(el, 'Add entity')!.disabled).toBe(true);
    expect(button(el, 'Add relationship')!.disabled).toBe(true);
    expect(button(el, 'Submit to LCRB')).toBeUndefined();
    expect(build().lockedByLCRB).toBe(true);
    // The canvas cards are not draggable and their handles take no pointer events.
    const nodes = [...el.querySelectorAll('g.vflow-node')];
    expect(nodes.length).toBe(6);
    expect(nodes.every((n) => n.classList.contains('vflow-node--undraggable'))).toBe(true);
    const handles = [...el.querySelectorAll('g.vflow-node circle[stroke="#ffffff"]')] as SVGElement[];
    expect(handles.length).toBe(24);
    expect(handles.every((c) => c.style.pointerEvents === 'none')).toBe(true);
    // The inspector hides every edit control.
    nodeCard(el, 'Casey Blake').click();
    await next();
    expect(el.querySelector('node-inspector input')).toBeNull();
    expect(el.querySelector('node-inspector textarea')).toBeNull();
    expect(el.querySelector('node-inspector')!.textContent).toContain('Note for LCRB');

    button(el, 'Unlock')!.click();
    await next();
    expect(el.textContent).not.toContain('This build is locked by the LCRB');
    expect(button(el, 'Add entity')!.disabled).toBe(false);
    const open = [...el.querySelectorAll('g.vflow-node circle[stroke="#ffffff"]')] as SVGElement[];
    expect(open.every((c) => c.style.pointerEvents === 'all')).toBe(true);
    expect(el.querySelectorAll('g.vflow-node--undraggable').length).toBe(0);
  });

  it('submits to the LCRB (version 2, synced to Dynamics) and reopens the draft', async () => {
    vi.spyOn(Math, 'random').mockReturnValue(0.5);
    const { el, next } = await mount();
    button(el, 'Submit to LCRB')!.click();
    await new Promise((r) => setTimeout(r, 1200));
    await next();
    expect(build().status).toBe('submitted');
    expect(build().version).toBe(2);
    expect(el.querySelector('page-hero span[uiBadge]')!.textContent).toContain('Submitted to LCRB');
    expect(button(el, 'Submit to LCRB')).toBeUndefined();
    const sync = JSON.parse(localStorage.getItem('lcrb.v2.dynamics-sync') ?? '{}') as Record<string, { caseType: string; reference: string }>;
    expect(Object.values(sync).some((r) => r.caseType === 'Legal Entity Build' && r.reference === 'build_BC0987654')).toBe(true);

    button(el, 'Reopen draft')!.click();
    await next();
    expect(build().status).toBe('draft');
    expect(button(el, 'Submit to LCRB')).toBeTruthy();
  });

  it('shows the changes-requested banner and the review notes after the demo request', async () => {
    const { el, next } = await mount();
    button(el, 'Request changes')!.click();
    await next();
    expect(el.textContent).toContain('The LCRB has requested changes.');
    expect(el.querySelector('page-hero span[uiBadge]')!.textContent).toContain('Changes requested');
    expect(el.textContent).toContain("Please confirm the shareholding percentages");
    button(el, 'Approve')!.click();
    await next();
    expect(el.querySelector('page-hero span[uiBadge]')!.textContent).toContain('Approved by LCRB');
  });
});
