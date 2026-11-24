import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { afterEach, describe, expect, it } from 'vitest';
import { AssistantProvider } from './assistant-provider';
import { AssistantLauncher } from './assistant-launcher';

const SESSION_KEY = 'lcrb.mock.session';

async function mount(navigate = true) {
  TestBed.configureTestingModule({ providers: [provideRouter([{ path: '**', children: [] }])] });
  const fixture = TestBed.createComponent(AssistantLauncher);
  if (navigate) await TestBed.inject(Router).navigateByUrl('/');
  for (let i = 0; i < 3; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
  }
  return { fixture, assistant: TestBed.inject(AssistantProvider) };
}

afterEach(() => localStorage.clear());

describe('AssistantLauncher', () => {
  it('draws the chat id as soon as a session exists, like the prototype panel mount', async () => {
    localStorage.setItem(SESSION_KEY, JSON.stringify({ role: 'licensee', firstName: 'Matt', lastName: 'Harding' }));
    const { fixture, assistant } = await mount();
    expect(assistant.chatId).toMatch(/^[0-9A-Za-z]{16}$/);
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Ask AI');
  });

  it('waits for the first navigation, as the prototype panel mounts after the first page view', async () => {
    localStorage.setItem(SESSION_KEY, JSON.stringify({ role: 'licensee', firstName: 'Matt', lastName: 'Harding' }));
    const { assistant } = await mount(false);
    expect(assistant.chatId).toBeNull();
  });

  it('draws nothing while signed out', async () => {
    const { assistant } = await mount();
    expect(assistant.chatId).toBeNull();
  });
});
