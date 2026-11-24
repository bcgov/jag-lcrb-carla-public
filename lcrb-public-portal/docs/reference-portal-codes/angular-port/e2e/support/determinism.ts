/** Helpers that make both apps render identically: seeded storage, a frozen clock and stubbed map tiles. */
import type { Page } from '@playwright/test';

/** Fixed "now" used by the parity suites unless a test chooses another. */
export const FIXED_NOW = '2026-11-02T10:00:00-08:00';

/** Seed localStorage (and optionally sessionStorage) before any page script runs. */
export async function seedStorage(
  page: Page,
  local: Record<string, string>,
  session: Record<string, string> = {},
): Promise<void> {
  await page.addInitScript(
    ([l, s]) => {
      for (const [k, v] of Object.entries(l)) window.localStorage.setItem(k, v);
      for (const [k, v] of Object.entries(s)) window.sessionStorage.setItem(k, v);
    },
    [local, session] as const,
  );
}

/** Freeze Date at a fixed time. Timers keep running, so app animations and polling still work. */
export async function freezeClock(page: Page, at: string = FIXED_NOW): Promise<void> {
  await page.clock.setFixedTime(new Date(at));
}

/** Fixed seed for Math.random in the parity suites. */
export const DEFAULT_SEED = 20261102;

/** Replace Math.random with a seeded mulberry32 generator, restarted on every page load; `burn` draws are discarded first to line up framework-internal draws (see FRAMEWORK_DRAWS in parity.ts). */
export async function seedRandom(page: Page, seed: number = DEFAULT_SEED, burn = 0): Promise<void> {
  await page.addInitScript(
    ([s, n]: [number, number]) => {
      let a = s >>> 0;
      Math.random = () => {
        a = (a + 0x6d2b79f5) >>> 0;
        let t = a;
        t = Math.imul(t ^ (t >>> 15), t | 1);
        t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
        return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
      };
      for (let i = 0; i < n; i++) Math.random();
    },
    [seed, burn] as [number, number],
  );
}

/** Wait until the page is visually stable: network idle, web fonts loaded, top progress bar gone, then two animation frames. */
export async function settle(page: Page): Promise<void> {
  await page.waitForLoadState('networkidle');
  await page.evaluate(async () => {
    await document.fonts.ready;
    await new Promise<void>((r) => requestAnimationFrame(() => requestAnimationFrame(() => r())));
  });
  await page.waitForLoadState('networkidle');
}

/** Make requestAnimationFrame animations finish almost at once (each frame jumps 10 s); Recharts restarts its animation when a full-page screenshot resizes the viewport. */
export async function fastForwardAnimations(page: Page): Promise<void> {
  await page.addInitScript(() => {
    const raf = window.requestAnimationFrame.bind(window);
    let offset = 0;
    window.requestAnimationFrame = (cb: FrameRequestCallback) =>
      raf((t) => {
        offset += 10_000;
        cb(t + offset);
      });
  });
}

/** Wait until chart shapes (matched by `selector`) are drawn and stop changing; Recharts animates in JavaScript, which disabling CSS animation does not stop. */
export async function waitForCharts(page: Page, selector: string): Promise<void> {
  await page.waitForFunction(
    async (sel) => {
      const snap = () =>
        Array.from(document.querySelectorAll(sel))
          .map((el) => {
            const r = el.getBoundingClientRect();
            return `${el.getAttribute('d') ?? ''}|${r.width.toFixed(1)}x${r.height.toFixed(1)}`;
          })
          .join(';');
      const first = snap();
      await new Promise((r) => setTimeout(r, 400));
      const shapes = document.querySelectorAll(sel);
      const drawn = Array.from(shapes).some((el) => el.getBoundingClientRect().height > 0);
      return shapes.length > 0 && drawn && snap() === first;
    },
    selector,
    { polling: 100, timeout: 15_000 },
  );
}

// A 1x1 light-grey PNG. Leaflet stretches each tile image to 256x256.
const TILE_PNG = Buffer.from(
  'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mN4/frNfwAJYwPZyALe4wAAAABJRU5ErkJggg==',
  'base64',
);

/** Replace OpenStreetMap tiles with one fixed image so maps render identically. */
export async function stubMapTiles(page: Page): Promise<void> {
  await page.route(/tile\.openstreetmap\.org\//, (route) =>
    route.fulfill({ status: 200, contentType: 'image/png', body: TILE_PNG }),
  );
}
