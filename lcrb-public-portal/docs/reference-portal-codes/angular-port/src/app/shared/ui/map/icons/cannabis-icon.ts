/** The cannabis-leaf badge, drawn on its own <svg> host. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'svg[cannabisIcon]',
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

    <svg:g transform="translate(20, 20)">
      <svg:path d="M0 -10C-2 -6 -2 -2 0 2C2 -2 2 -6 0 -10Z" [attr.fill]="fillColor()" />
      <svg:path d="M-3 -8C-7 -6 -9 -3 -7 1C-4 -1 -3 -4 -3 -8Z" [attr.fill]="fillColor()" />
      <svg:path d="M-6 -5C-10 -2 -11 2 -8 5C-5 2 -5 -1 -6 -5Z" [attr.fill]="fillColor()" />
      <svg:path d="M-8 -1C-11 3 -10 7 -6 8C-5 4 -6 1 -8 -1Z" [attr.fill]="fillColor()" />
      <svg:path d="M3 -8C7 -6 9 -3 7 1C4 -1 3 -4 3 -8Z" [attr.fill]="fillColor()" />
      <svg:path d="M6 -5C10 -2 11 2 8 5C5 2 5 -1 6 -5Z" [attr.fill]="fillColor()" />
      <svg:path d="M8 -1C11 3 10 7 6 8C5 4 6 1 8 -1Z" [attr.fill]="fillColor()" />
      <svg:path d="M0 2V10" [attr.stroke]="fillColor()" stroke-width="2" stroke-linecap="round" />
    </svg:g>

    @if (hasContravention()) {
      <svg:circle cx="30" cy="10" r="6" fill="#DC2626" stroke="white" stroke-width="2" />
    }

    @if (isApplication()) {
      <svg:circle cx="30" cy="10" r="6" fill="#F59E0B" stroke="white" stroke-width="2" />
    }
  `,
})
export class CannabisIcon {
  readonly size = input(32);
  readonly hasContravention = input<boolean | undefined>(undefined);
  readonly isApplication = input<boolean | undefined>(undefined);
  protected readonly fillColor = computed(() => (this.isApplication() ? '#C4A35A' : this.hasContravention() ? '#C4534A' : '#4A9C6D'));
  protected readonly bgColor = computed(() => (this.isApplication() ? '#FEF7E0' : this.hasContravention() ? '#FEE2E2' : '#E8F5E9'));
}
