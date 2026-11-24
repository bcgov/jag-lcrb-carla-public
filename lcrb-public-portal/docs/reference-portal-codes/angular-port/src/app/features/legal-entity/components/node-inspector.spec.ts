import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { createSampleBuild } from '@data-access/repositories/legal-entity/mock/legal-entity';
import { type LegalEntityBuild, type LENode } from '@domain/models/legal-entity';
import { NodeInspector } from './node-inspector';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

@Component({
  imports: [NodeInspector],
  template: `<node-inspector
    [build]="build()"
    [node]="node()"
    [readOnly]="readOnly()"
    actor="Alex Rivera"
    (closed)="closes = closes + 1"
    (addRelationship)="relationships.push($event)"
  />`,
})
class Host {
  build = signal<LegalEntityBuild>(createSampleBuild('Harding Hospitality Group Ltd.', 'BC0987654'));
  node = signal<LENode>(this.build().nodes[0]);
  readOnly = signal(false);
  closes = 0;
  relationships: string[] = [];
}

async function mount(pick: (b: LegalEntityBuild) => LENode, readOnly = false) {
  const fixture = TestBed.createComponent(Host);
  const host = fixture.componentInstance;
  host.node.set(pick(host.build()));
  host.readOnly.set(readOnly);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, host, el: fixture.nativeElement as HTMLElement, next: () => settle(fixture) };
}

describe('NodeInspector', () => {
  beforeEach(() => localStorage.clear());
  afterEach(() => {
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
    document.body.innerHTML = '';
  });

  it('shows a record card read-only with locks, linked licences and relationships', async () => {
    const { el } = await mount((b) => b.nodes[0]);
    expect(el.querySelector('h2')!.textContent).toContain('Harding Hospitality Group Ltd.');
    expect(el.textContent).toContain('Applicant Legal Entity');
    expect(el.textContent).toContain('From LCRB records');
    expect(el.querySelector('#insp-name')).toBeNull();
    expect(el.textContent).toContain('Business number');
    expect(el.textContent).toContain('Linked licences');
    expect(el.textContent).toContain('This card comes from LCRB systems of record.');
    expect(el.textContent).toContain('Shareholding');
    expect(el.textContent).toContain('from Avery Holdings Ltd.');
    expect(el.textContent).not.toContain('Remove this entity');
  });

  it('lets a portal-added card be renamed and removed, and requests a relationship', async () => {
    const { host, el, next } = await mount((b) => b.nodes.find((n) => n.legalName === 'Blake Family Trust')!);
    expect(el.textContent).toContain('Suggested');
    expect(el.querySelector('#insp-name')).not.toBeNull();
    expect(el.textContent).toContain('Morgan Blake');
    const add = [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Add')!;
    add.click();
    expect(host.relationships).toEqual([host.node().id]);
    expect(el.textContent).toContain('Remove this entity');
    [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Close inspector'))!.click();
    await next();
    expect(host.closes).toBe(1);
  });

  it('shows a Tied House mirror card as informational only', async () => {
    const { el } = await mount((b) => ({
      ...b.nodes[1],
      id: 'th_1',
      external: 'tied-house' as const,
      source: 'portal' as const,
      locked: false,
    }));
    expect(el.textContent).toContain('From Tied House');
    expect(el.textContent).toContain('mirrored from your Tied House declarations');
    expect(el.querySelector('input')).toBeNull();
    expect(el.querySelector('textarea')).toBeNull();
    expect(el.textContent).not.toContain('Add person');
    expect(el.textContent).not.toContain('Remove this entity');
  });

  it('hides every edit control when the build is locked', async () => {
    const { el } = await mount((b) => b.nodes.find((n) => n.legalName === 'Blake Family Trust')!, true);
    expect(el.querySelector('input')).toBeNull();
    expect(el.querySelector('textarea')).toBeNull();
    expect(el.textContent).not.toContain('Add person');
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.trim() === 'Add')).toBe(false);
    expect(el.textContent).toContain('Note for LCRB');
  });
});
