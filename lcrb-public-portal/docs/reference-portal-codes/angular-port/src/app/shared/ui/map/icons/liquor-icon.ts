/** The wine-glass badge, drawn on its own <svg> host. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'svg[liquorIcon]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[attr.width]': 'size()',
    '[attr.height]': 'size()',
    viewBox: '0 0 40 40',
    fill: 'none',
    xmlns: 'http://www.w3.org/2000/svg',
  },
  template: `
    <svg:circle cx="20" cy="20" r="18" [attr.fill]="bgColor()" [attr.stroke]="fillColor()" stroke-width="2" />

    <svg:path
      d="M20 28V24M20 24C17 24 14 21 14 17V12H26V17C26 21 23 24 20 24Z"
      [attr.stroke]="fillColor()"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
    <svg:path d="M15 28H25" [attr.stroke]="fillColor()" stroke-width="2" stroke-linecap="round" />
    <svg:path d="M14 15H26" [attr.stroke]="fillColor()" stroke-width="1.5" stroke-linecap="round" />
    <svg:path d="M15 17C15 20 17 22 20 22C23 22 25 20 25 17V15H15V17Z" [attr.fill]="fillColor()" opacity="0.3" />

    @if (hasContravention()) {
      <svg:circle cx="30" cy="10" r="6" fill="#DC2626" stroke="white" stroke-width="2" />
    }

    @if (isApplication()) {
      <svg:circle cx="30" cy="10" r="6" fill="#F59E0B" stroke="white" stroke-width="2" />
    }
  `,
})
export class LiquorIcon {
  readonly size = input(32);
  readonly hasContravention = input<boolean | undefined>(undefined);
  readonly isApplication = input<boolean | undefined>(undefined);
  protected readonly fillColor = computed(() => (this.isApplication() ? '#C4A35A' : this.hasContravention() ? '#C4534A' : '#C4784A'));
  protected readonly bgColor = computed(() => (this.isApplication() ? '#FEF7E0' : this.hasContravention() ? '#FEE2E2' : '#FFF4ED'));
}
