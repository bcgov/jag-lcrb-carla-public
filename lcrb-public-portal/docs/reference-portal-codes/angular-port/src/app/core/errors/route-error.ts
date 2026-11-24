/** Angular has no per-route error boundary: this global ErrorHandler records the error and the shell shows the error page; "Try again" re-runs the navigation. */
import { ErrorHandler, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class RouteErrorState {
  readonly error = signal<(Error & { digest?: string }) | null>(null);
  private readonly router = inject(Router);

  reset(): void {
    this.error.set(null);
    void this.router.navigateByUrl(this.router.url, { onSameUrlNavigation: 'reload' });
  }
}

@Injectable()
export class RouteErrorHandler implements ErrorHandler {
  private readonly state = inject(RouteErrorState);

  handleError(error: unknown): void {
    const err = (error instanceof Error ? error : new Error(String(error))) as Error & { digest?: string };
    // Surface for debugging / server logs without breaking the UI.
    console.log('[v0] Route error boundary caught:', err?.message, err?.digest ?? '');
    console.error(error);
    this.state.error.set(err);
  }
}
