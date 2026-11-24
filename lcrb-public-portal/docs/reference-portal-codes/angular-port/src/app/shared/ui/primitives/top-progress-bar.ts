/** Slim progress bar across the top of the page during navigation; starts on same-origin link clicks, pushState, back/forward or startProgress(), and completes when the router finishes. */
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { doneProgress, startProgress, subscribeProgress } from '@core/progress/progress-bus';

@Component({
  selector: 'ui-top-progress-bar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `@if (visible()) {
    <div class="pointer-events-none fixed inset-x-0 top-0 z-[200] h-0.5" role="progressbar" aria-hidden="true">
      <div
        class="h-full bg-[#FCBA19] shadow-[0_0_8px_rgba(252,186,25,0.7)] transition-[width] duration-200 ease-out"
        [style.width.%]="round(progress() * 100)"
      ></div>
    </div>
  }`,
})
export class TopProgressBar {
  protected readonly progress = signal(0);
  protected readonly visible = signal(false);
  protected readonly round = Math.round;
  private trickle: ReturnType<typeof setInterval> | null = null;
  private hide: ReturnType<typeof setTimeout> | null = null;
  private active = false;

  constructor() {
    const clearTimers = () => {
      if (this.trickle) {
        clearInterval(this.trickle);
        this.trickle = null;
      }
      if (this.hide) {
        clearTimeout(this.hide);
        this.hide = null;
      }
    };
    const begin = () => {
      // Restart cleanly if already running.
      clearTimers();
      this.active = true;
      this.visible.set(true);
      this.progress.set(0.08);
      // Trickle upward but never reach 100% until "done".
      this.trickle = setInterval(() => {
        this.progress.update((p) => {
          if (p >= 0.9) return p;
          // Ease-out: smaller steps as it approaches the ceiling.
          const remaining = 0.9 - p;
          return p + Math.max(0.005, remaining * 0.12);
        });
      }, 180);
    };
    const finish = () => {
      if (!this.active) return;
      this.active = false;
      clearTimers();
      this.progress.set(1);
      this.hide = setTimeout(() => {
        this.visible.set(false);
        this.progress.set(0);
      }, 260);
    };

    const unsubscribe = subscribeProgress((event) => {
      if (event === 'start') begin();
      else finish();
    });

    const onClick = (e: MouseEvent) => {
      if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
      const anchor = (e.target as HTMLElement | null)?.closest('a');
      if (!anchor) return;
      const href = anchor.getAttribute('href');
      const target = anchor.getAttribute('target');
      if (!href || href.startsWith('#') || target === '_blank' || anchor.hasAttribute('download')) return;
      // Only same-origin, actually-navigating links.
      try {
        const url = new URL(href, window.location.href);
        if (url.origin !== window.location.origin) return;
        if (url.pathname === window.location.pathname && url.search === window.location.search) return;
      } catch {
        return;
      }
      startProgress();
    };

    // Patch History API so programmatic navigation also starts the bar.
    const origPush = history.pushState;
    const origReplace = history.replaceState;
    history.pushState = function (...args) {
      startProgress();
      return origPush.apply(this, args as Parameters<typeof origPush>);
    };
    history.replaceState = function (...args) {
      return origReplace.apply(this, args as Parameters<typeof origReplace>);
    };
    const onPopState = () => startProgress();

    document.addEventListener('click', onClick);
    window.addEventListener('popstate', onPopState);

    // When the route finishes changing, complete the bar.
    const sub = inject(Router).events.subscribe((e) => {
      if (e instanceof NavigationEnd) doneProgress();
    });

    inject(DestroyRef).onDestroy(() => {
      unsubscribe();
      sub.unsubscribe();
      document.removeEventListener('click', onClick);
      window.removeEventListener('popstate', onPopState);
      history.pushState = origPush;
      history.replaceState = origReplace;
      clearTimers();
    });
  }
}
