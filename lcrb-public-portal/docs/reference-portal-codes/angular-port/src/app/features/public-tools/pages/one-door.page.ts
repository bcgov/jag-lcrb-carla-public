/** Standalone simulation, intentionally not linked from anywhere in the LCRB portal. */
import { ChangeDetectionStrategy, Component, inject, OnDestroy } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { OneDoorExperience } from '../components/one-door-experience';

@Component({
  selector: 'one-door-page',
  imports: [OneDoorExperience],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<one-door-experience />`,
})
export class OneDoorPage implements OnDestroy {
  private readonly meta = inject(Meta);

  constructor() {
    // `metadata.robots` in the prototype.
    this.meta.updateTag({ name: 'robots', content: 'noindex, nofollow' });
  }

  ngOnDestroy(): void {
    this.meta.removeTag('name="robots"');
  }
}
