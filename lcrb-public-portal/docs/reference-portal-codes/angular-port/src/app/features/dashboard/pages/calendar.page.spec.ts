import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { CalendarPage } from './calendar.page';

@Component({ template: '' })
class Blank {}

beforeEach(() => {
  localStorage.clear();
  sessionStorage.clear();
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', component: Blank }])] });
});

describe('CalendarPage', () => {
  it('renders the hero, the calendar once hydrated', async () => {
    const fixture = TestBed.createComponent(CalendarPage);
    fixture.detectChanges();
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('h1')!.textContent).toContain('Renewals & Compliance Calendar');
    await fixture.whenStable();
    fixture.detectChanges();
    expect(el.querySelector('[aria-label="Loading calendar"]')).toBeNull();
    expect(el.textContent).toContain('Total tracked');
  });
});
