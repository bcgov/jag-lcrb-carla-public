import { Component } from '@angular/core';
import { type ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { TopProgressBar } from './top-progress-bar';

@Component({ template: '' })
class Blank {}

let fixture: ComponentFixture<TopProgressBar>;
const bar = () => (fixture.nativeElement as HTMLElement).querySelector('[role=progressbar]');

beforeEach(() => {
  vi.useFakeTimers();
  TestBed.configureTestingModule({
    providers: [
      provideRouter([
        { path: 'ok', component: Blank },
        { path: 'broken', loadComponent: () => Promise.reject(new Error('chunk failed')) },
        { path: 'blocked', component: Blank, canActivate: [() => false] },
      ]),
    ],
  });
  fixture = TestBed.createComponent(TopProgressBar);
  fixture.detectChanges();
});
afterEach(() => vi.useRealTimers());

describe('TopProgressBar', () => {
  for (const url of ['/broken', '/blocked']) {
    it(`hides and stops its timer when navigation to ${url} does not complete`, async () => {
      window.dispatchEvent(new PopStateEvent('popstate'));
      await Promise.resolve();
      fixture.detectChanges();
      expect(bar()).toBeTruthy();

      await TestBed.inject(Router).navigateByUrl(url).catch(() => {});
      await Promise.resolve();
      vi.advanceTimersByTime(300);
      fixture.detectChanges();
      expect(bar()).toBeNull();
      expect(vi.getTimerCount()).toBe(0);
    });
  }
});
