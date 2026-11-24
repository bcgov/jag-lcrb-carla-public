import { TestBed } from '@angular/core/testing';
import { Meta } from '@angular/platform-browser';
import { describe, expect, it } from 'vitest';
import { GatePage } from './gate.page';

async function render(inputs: { error?: string; next?: string } = {}) {
  const fixture = TestBed.createComponent(GatePage);
  for (const [k, v] of Object.entries(inputs)) fixture.componentRef.setInput(k, v);
  await fixture.whenStable();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('GatePage', () => {
  it('posts a native form to /gate with next defaulting to /login', async () => {
    const { el } = await render();
    const form = el.querySelector('form')!;
    expect(form.getAttribute('action')).toBe('/gate');
    expect(form.getAttribute('method')).toBe('post');
    expect((el.querySelector('input[name="next"]') as HTMLInputElement).value).toBe('/login');
    const code = el.querySelector('input[name="code"]') as HTMLInputElement;
    expect(code.type).toBe('password');
    expect(code.required).toBe(true);
    expect(code.hasAttribute('autofocus')).toBe(false);
    expect(el.querySelector('[role="alert"]')).toBeNull();
  });

  it('keeps next and shows the error when ?error is set', async () => {
    const { el } = await render({ error: '1', next: '/licences' });
    expect((el.querySelector('input[name="next"]') as HTMLInputElement).value).toBe('/licences');
    expect(el.querySelector('[role="alert"]')?.textContent).toContain('That access code is not correct');
  });

  it('focuses the code field and sets robots noindex while shown', async () => {
    const { fixture, el } = await render();
    expect(document.activeElement).toBe(el.querySelector('#code'));
    const meta = TestBed.inject(Meta);
    expect(meta.getTag('name="robots"')?.content).toBe('noindex, nofollow');
    fixture.destroy();
    expect(meta.getTag('name="robots"')).toBeNull();
  });
});
