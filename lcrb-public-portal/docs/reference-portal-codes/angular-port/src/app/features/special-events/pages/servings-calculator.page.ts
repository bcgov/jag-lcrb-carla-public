/** SEP Servings Calculator: a planning aid, not a requirement, for how much alcohol to buy. Uses Canadian standard drinks (beer 341 mL, wine and spirits 750 mL). */
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Separator } from '@shared/ui/primitives/separator';
import { Slider } from '@shared/ui/primitives/slider';
import { writeServingsPrefill } from '@core/navigation/servings-prefill';
import { path } from '@/app/route-registry';

type Level = 'light' | 'moderate' | 'heavy';

// Drinks consumed by a drinking guest: first hour, then each additional hour.
const LEVELS: Record<Level, { label: string; first: number; rest: number; hint: string }> = {
  light: { label: 'Light', first: 1, rest: 0.5, hint: 'Daytime / family event' },
  moderate: { label: 'Moderate', first: 2, rest: 1, hint: 'Typical reception' },
  heavy: { label: 'Lively', first: 2, rest: 1.5, hint: 'Evening party / celebration' },
};

// Servings yielded per purchasing unit.
const WINE_SERVINGS_PER_BOTTLE = 5;
const SPIRIT_SERVINGS_PER_BOTTLE = 16;
const BEERS_PER_CASE = 24;

function clampNumber(value: number, min: number, max: number) {
  if (Number.isNaN(value)) return min;
  return Math.min(max, Math.max(min, value));
}

@Component({
  selector: 'servings-calculator-page',
  imports: [RouterLink, BCGovHeader, BCGovFooter, ...CARD, Button, Input, Label, LucideIcon, Separator, Slider],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header />

      <main class="flex-1">
        <!-- Hero -->
        <div class="bg-bc-blue text-white">
          <div class="container mx-auto px-4 py-8">
            <a [routerLink]="path('special-events.list')" class="mb-4 inline-flex items-center gap-1.5 text-sm text-white/80 hover:text-white">
              <svg lucide="ArrowLeft" class="h-4 w-4"></svg>
              Back to Special Event Permits
            </a>
            <div class="flex items-center gap-3">
              <div class="rounded-lg bg-bc-gold/20 p-2.5">
                <svg lucide="Calculator" class="h-7 w-7 text-bc-gold"></svg>
              </div>
              <div>
                <h1 class="text-2xl font-bold md:text-3xl">Servings Calculator</h1>
                <p class="mt-1 text-pretty text-white/80">Estimate how much beer, wine, and spirits to buy for your event.</p>
              </div>
            </div>
          </div>
        </div>

        <div class="container mx-auto px-4 py-8">
          <div class="grid grid-cols-1 gap-6 lg:grid-cols-5">
            <!-- Inputs -->
            <div class="space-y-6 lg:col-span-3">
              <div uiCard>
                <div uiCardHeader>
                  <div uiCardTitle class="text-lg">Your event</div>
                </div>
                <div uiCardContent class="space-y-6">
                  <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                    <div class="space-y-2">
                      <label uiLabel for="guests" class="flex items-center gap-2">
                        <svg lucide="Users" class="h-4 w-4 text-bc-blue"></svg>
                        Expected guests
                      </label>
                      <input
                        uiInput
                        id="guests"
                        type="number"
                        min="1"
                        max="10000"
                        [value]="guests()"
                        (input)="guests.set(clamp($any($event.target).valueAsNumber, 1, 10000))"
                      />
                    </div>
                    <div class="space-y-2">
                      <label uiLabel for="hours" class="flex items-center gap-2">
                        <svg lucide="Clock" class="h-4 w-4 text-bc-blue"></svg>
                        Event length (hours)
                      </label>
                      <input
                        uiInput
                        id="hours"
                        type="number"
                        min="1"
                        max="24"
                        [value]="hours()"
                        (input)="hours.set(clamp($any($event.target).valueAsNumber, 1, 24))"
                      />
                    </div>
                  </div>

                  <div class="space-y-3">
                    <div class="flex items-center justify-between">
                      <label uiLabel for="share">Guests who will drink alcohol</label>
                      <span class="text-sm font-semibold text-bc-blue">{{ drinkingShare() }}%</span>
                    </div>
                    <span uiSlider [id]="'share'" [value]="[drinkingShare()]" (valueChange)="drinkingShare.set($event![0])" [min]="0" [max]="100" [step]="5"></span>
                    <p class="text-xs text-muted-foreground">About {{ result().drinkers }} of {{ guests() }} guests are expected to drink.</p>
                  </div>

                  <div class="space-y-2">
                    <label uiLabel>Consumption level</label>
                    <div class="grid grid-cols-3 gap-2">
                      @for (key of levelKeys; track key) {
                        <button
                          type="button"
                          (click)="level.set(key)"
                          [attr.aria-pressed]="level() === key"
                          [attr.class]="
                            'rounded-lg border p-3 text-left transition-colors ' +
                            (level() === key ? 'border-bc-blue bg-bc-blue/5 ring-1 ring-bc-blue' : 'border-border hover:bg-muted')
                          "
                        >
                          <span class="block text-sm font-semibold">{{ levels[key].label }}</span>
                          <span class="block text-xs text-muted-foreground">{{ levels[key].hint }}</span>
                        </button>
                      }
                    </div>
                  </div>
                </div>
              </div>

              <div uiCard>
                <div uiCardHeader>
                  <div uiCardTitle class="text-lg">Drink mix</div>
                </div>
                <div uiCardContent class="space-y-6">
                  <div class="space-y-3">
                    <div class="flex items-center justify-between">
                      <label uiLabel for="beer" class="flex items-center gap-2">
                        <svg lucide="Beer" class="h-4 w-4 text-bc-blue"></svg> Beer / cider / coolers
                      </label>
                      <span class="text-sm font-semibold text-bc-blue">{{ beerPct() }}%</span>
                    </div>
                    <span uiSlider [id]="'beer'" [value]="[beerPct()]" (valueChange)="beerPct.set($event![0])" [min]="0" [max]="100" [step]="5"></span>
                  </div>

                  <div class="space-y-3">
                    <div class="flex items-center justify-between">
                      <label uiLabel for="wine" class="flex items-center gap-2">
                        <svg lucide="Wine" class="h-4 w-4 text-bc-blue"></svg> Wine
                      </label>
                      <span class="text-sm font-semibold text-bc-blue">{{ winePct() }}%</span>
                    </div>
                    <span uiSlider [id]="'wine'" [value]="[winePct()]" (valueChange)="winePct.set($event![0])" [min]="0" [max]="100" [step]="5"></span>
                  </div>

                  <div class="flex items-center justify-between rounded-lg bg-muted px-3 py-2">
                    <span class="flex items-center gap-2 text-sm font-medium">
                      <svg lucide="Martini" class="h-4 w-4 text-bc-blue"></svg> Spirits / cocktails
                    </span>
                    <span class="text-sm font-semibold text-bc-blue">{{ spiritPct() }}%</span>
                  </div>

                  @if (!mixValid()) {
                    <p class="text-sm text-bc-danger">Beer and wine together can't exceed 100%. Lower one of the sliders.</p>
                  }

                  <div uiSeparator></div>

                  <label class="flex cursor-pointer items-start gap-3">
                    <input
                      type="checkbox"
                      [checked]="buffer()"
                      (change)="buffer.set($any($event.target).checked)"
                      class="mt-1 h-4 w-4 accent-[var(--bc-blue)]"
                    />
                    <span class="text-sm">
                      <span class="font-medium">Add a 10% buffer</span>
                      <span class="block text-muted-foreground">A small safety margin so you don't run short.</span>
                    </span>
                  </label>

                  <button uiButton variant="outline" (click)="reset()" class="gap-2">
                    <svg lucide="RotateCcw" class="h-4 w-4"></svg>
                    Reset
                  </button>
                </div>
              </div>
            </div>

            <!-- Results -->
            <div class="space-y-4 lg:col-span-2">
              <div uiCard class="border-bc-blue/30">
                <div uiCardHeader class="pb-3">
                  <div uiCardTitle class="text-lg">Estimated shopping list</div>
                </div>
                <div uiCardContent class="space-y-4">
                  <div class="rounded-lg bg-bc-blue px-4 py-4 text-white">
                    <p class="text-sm text-white/80">Total standard drinks</p>
                    <p class="text-3xl font-bold">{{ result().totalDrinks.toLocaleString() }}</p>
                    <p class="mt-1 text-xs text-white/70">≈ {{ result().perDrinker }} drinks per drinking guest over {{ hours() }}h</p>
                  </div>

                  <ul class="space-y-3">
                    <li class="flex items-center justify-between gap-3 rounded-lg border p-3">
                      <span class="flex items-center gap-2.5">
                        <svg lucide="Beer" class="h-5 w-5 text-bc-blue"></svg>
                        <span class="text-sm font-medium">Beer / coolers</span>
                      </span>
                      <span class="text-right">
                        <span class="block font-semibold">{{ result().beerCans.toLocaleString() }} cans</span>
                        <span class="block text-xs text-muted-foreground">~{{ result().beerCases }} cases of 24</span>
                      </span>
                    </li>
                    <li class="flex items-center justify-between gap-3 rounded-lg border p-3">
                      <span class="flex items-center gap-2.5">
                        <svg lucide="Wine" class="h-5 w-5 text-bc-blue"></svg>
                        <span class="text-sm font-medium">Wine</span>
                      </span>
                      <span class="text-right">
                        <span class="block font-semibold">{{ result().wineBottles.toLocaleString() }} bottles</span>
                        <span class="block text-xs text-muted-foreground">750 mL · 5 glasses each</span>
                      </span>
                    </li>
                    <li class="flex items-center justify-between gap-3 rounded-lg border p-3">
                      <span class="flex items-center gap-2.5">
                        <svg lucide="Martini" class="h-5 w-5 text-bc-blue"></svg>
                        <span class="text-sm font-medium">Spirits</span>
                      </span>
                      <span class="text-right">
                        <span class="block font-semibold">{{ result().spiritBottles.toLocaleString() }} bottles</span>
                        <span class="block text-xs text-muted-foreground">750 mL · ~16 drinks each</span>
                      </span>
                    </li>
                  </ul>

                  <div class="flex gap-3 rounded-lg border border-bc-info-border/30 bg-bc-info p-3">
                    <svg lucide="Info" class="h-4 w-4 shrink-0 text-bc-info-border"></svg>
                    <p class="text-xs leading-relaxed text-bc-text-secondary">
                      This is a planning estimate only. Actual needs vary with your guests, weather, and
                      menu. Always serve responsibly and offer water and non-alcoholic options.
                    </p>
                  </div>

                  <button uiButton (click)="handleApply()" class="w-full gap-2 bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90">
                    Apply for a Special Event Permit
                    <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                  </button>
                  <p class="text-center text-xs text-muted-foreground">
                    Your estimate ({{ result().totalDrinks.toLocaleString() }} drinks, {{ guests().toLocaleString() }} guests) will
                    be carried into the application.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class ServingsCalculatorPage {
  protected readonly path = path;
  private readonly router = inject(Router);
  protected readonly levels = LEVELS;
  protected readonly levelKeys = Object.keys(LEVELS) as Level[];
  protected readonly clamp = clampNumber;

  protected readonly guests = signal(80);
  protected readonly hours = signal(4);
  protected readonly drinkingShare = signal(75); // % of guests who drink
  protected readonly level = signal<Level>('moderate');
  protected readonly buffer = signal(true); // add a 10% safety margin

  // Drink mix (percent). Beer + wine + spirits should total 100.
  protected readonly beerPct = signal(50);
  protected readonly winePct = signal(35);
  protected readonly spiritPct = computed(() => Math.max(0, 100 - this.beerPct() - this.winePct()));

  protected readonly result = computed(() => {
    const guests = this.guests();
    const hours = this.hours();
    const level = this.level();
    const beerPct = this.beerPct();
    const winePct = this.winePct();
    const spiritPct = this.spiritPct();
    const drinkers = (guests * this.drinkingShare()) / 100;
    const perDrinker = LEVELS[level].first + LEVELS[level].rest * Math.max(0, hours - 1);
    let totalDrinks = drinkers * perDrinker;
    if (this.buffer()) totalDrinks *= 1.1;
    totalDrinks = Math.ceil(totalDrinks);

    const beerDrinks = (totalDrinks * beerPct) / 100;
    const wineDrinks = (totalDrinks * winePct) / 100;
    const spiritDrinks = (totalDrinks * spiritPct) / 100;

    const beerCans = Math.ceil(beerDrinks);
    const wineBottles = Math.ceil(wineDrinks / WINE_SERVINGS_PER_BOTTLE);
    const spiritBottles = Math.ceil(spiritDrinks / SPIRIT_SERVINGS_PER_BOTTLE);
    const beerCases = (beerCans / BEERS_PER_CASE).toFixed(1);

    return {
      drinkers: Math.round(drinkers),
      perDrinker: Math.round(perDrinker * 10) / 10,
      totalDrinks,
      beerCans,
      beerCases,
      wineBottles,
      spiritBottles,
    };
  });

  protected readonly mixValid = computed(() => this.beerPct() + this.winePct() <= 100);

  protected reset(): void {
    this.guests.set(80);
    this.hours.set(4);
    this.drinkingShare.set(75);
    this.level.set('moderate');
    this.buffer.set(true);
    this.beerPct.set(50);
    this.winePct.set(35);
  }

  // Carry this estimate into the SEP application. Only fires from this button, so the numbers never appear in a permit the applicant started elsewhere.
  protected handleApply(): void {
    const liquorTypes: string[] = [];
    if (this.beerPct() > 0) liquorTypes.push('beer');
    if (this.winePct() > 0) liquorTypes.push('wine');
    if (this.spiritPct() > 0) liquorTypes.push('spirits');
    writeServingsPrefill({
      estimatedDrinks: String(this.result().totalDrinks),
      maxGuests: String(this.guests()),
      liquorTypes,
    });
    void this.router.navigateByUrl(path('special-events.apply'));
  }
}
