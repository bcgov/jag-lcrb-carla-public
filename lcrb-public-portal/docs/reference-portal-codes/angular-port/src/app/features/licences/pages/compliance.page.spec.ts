import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { CompliancePage } from './compliance.page';

@Component({ template: '' })
class Blank {}

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
});

describe('CompliancePage', () => {
  it('renders the hero, breadcrumbs and the enforcement hub', async () => {
    const fixture = TestBed.createComponent(CompliancePage);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('h1')!.textContent).toContain('Compliance & Enforcement');
    expect(el.textContent).toContain('Responding on time protects your licence');
    expect([...el.querySelectorAll('nav a')].some((a) => a.getAttribute('href') === '/dashboard')).toBe(true);
    expect(el.querySelector('enforcement-hub')).not.toBeNull();
    expect(el.querySelector('main > div.max-w-7xl')).not.toBeNull();
  });
});
