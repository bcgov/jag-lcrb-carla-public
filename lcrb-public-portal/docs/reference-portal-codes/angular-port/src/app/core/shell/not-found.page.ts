/** Not-found page. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'not-found-page',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <div class="bg-[#013366] border-b-2 border-[#FCBA19]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <a routerLink="/" class="text-white font-semibold hover:underline">Liquor and Cannabis Licensing Portal</a>
        </div>
      </div>
      <main class="flex-1 flex items-center justify-center px-4 py-16">
        <div class="max-w-lg w-full text-center">
          <p class="text-5xl font-bold text-[#013366]">404</p>
          <h1 class="mt-4 text-2xl font-semibold text-foreground text-balance">We couldn't find that page</h1>
          <p class="mt-3 text-muted-foreground text-pretty leading-relaxed">
            The page you're looking for may have moved, or the link may be
            incorrect. Check the address, or return to the home page.
          </p>
          <div class="mt-8">
            <a
              routerLink="/"
              class="inline-block rounded-md bg-[#013366] px-5 py-2.5 text-sm font-medium text-white hover:bg-[#012a52] focus:outline-none focus-visible:ring-2 focus-visible:ring-[#FCBA19] focus-visible:ring-offset-2"
              >Go to home page</a
            >
          </div>
        </div>
      </main>
    </div>
  `,
})
export class NotFoundPage {}
