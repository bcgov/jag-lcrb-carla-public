import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { injectNoticeChime, isNoticeChimeMuted, setNoticeChimeMuted } from './notice-chime';

function blockStorage(storage: Storage): void {
  const fail = () => {
    throw new DOMException('blocked', 'SecurityError');
  };
  vi.spyOn(storage, 'getItem').mockImplementation(fail);
  vi.spyOn(storage, 'setItem').mockImplementation(fail);
  vi.spyOn(storage, 'removeItem').mockImplementation(fail);
}

afterEach(() => {
  vi.restoreAllMocks();
  localStorage.clear();
  sessionStorage.clear();
});

describe('notice chime storage', () => {
  it('keeps the mute preference in memory when localStorage is blocked', () => {
    blockStorage(Storage.prototype);
    expect(() => setNoticeChimeMuted(true)).not.toThrow();
    expect(isNoticeChimeMuted()).toBe(true);
    setNoticeChimeMuted(false);
    expect(isNoticeChimeMuted()).toBe(false);
  });

  it('skips the login chime without throwing when sessionStorage is blocked', async () => {
    blockStorage(Storage.prototype);
    @Component({ template: '' })
    class Host {
      constructor() {
        injectNoticeChime(3, true, true);
      }
    }
    const fixture = TestBed.createComponent(Host);
    fixture.detectChanges();
    await fixture.whenStable();
  });
});
