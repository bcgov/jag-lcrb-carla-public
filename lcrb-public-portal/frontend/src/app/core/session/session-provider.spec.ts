import { TestBed } from '@angular/core/testing';
import { beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider, type Session } from './session-provider';

const KEY = 'lcrb.mock.session';
const alice: Session = { role: 'licensee', firstName: 'Alice', lastName: 'Ng', organization: 'Alice Ltd' };

function create() {
  const provider = TestBed.inject(SessionProvider);
  TestBed.tick();
  return provider;
}

describe('SessionProvider', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.resetTestingModule();
  });

  it('starts empty and finishes loading when storage is empty', () => {
    const provider = create();
    expect(provider.session()).toBeNull();
    expect(provider.loading()).toBe(false);
  });

  it('restores a stored session', () => {
    localStorage.setItem(KEY, JSON.stringify(alice));
    expect(create().session()).toEqual(alice);
  });

  it('ignores malformed storage', () => {
    localStorage.setItem(KEY, '{not json');
    const provider = create();
    expect(provider.session()).toBeNull();
    expect(provider.loading()).toBe(false);
  });

  it('login stores the session and logout clears it', () => {
    const provider = create();
    provider.login(alice);
    expect(provider.session()).toEqual(alice);
    expect(JSON.parse(localStorage.getItem(KEY)!)).toEqual(alice);
    provider.logout();
    expect(provider.session()).toBeNull();
    expect(localStorage.getItem(KEY)).toBeNull();
  });
});
