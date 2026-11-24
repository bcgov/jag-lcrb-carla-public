/** Shows on hover/focus after `delayDuration` (Radix default 700 ms), on top with sideOffset 0 and a rotated arrow. */
import { Overlay, type OverlayRef } from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';
import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, DestroyRef, Directive, ElementRef, inject, input, numberAttribute, signal, TemplateRef, ViewContainerRef, ChangeDetectionStrategy, viewChild } from '@angular/core';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'ui-tooltip-content',
  imports: [NgTemplateOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ng-template #panel>
      <div data-slot="tooltip-content" data-side="top" data-state="delayed-open" role="tooltip" [class]="contentClass()">
        <ng-container [ngTemplateOutlet]="content()" />
        <span style="position: absolute; bottom: 0; left: 50%; transform: translate(-50%, 100%)">
          <svg width="10" height="5" viewBox="0 0 30 10" preserveAspectRatio="none" style="display: block" class="bg-foreground fill-foreground z-50 size-2.5 translate-y-[calc(-50%_-_2px)] rotate-45 rounded-[2px]">
            <polygon points="0,0 30,0 15,10"></polygon>
          </svg>
        </span>
      </div>
    </ng-template>
  `,
})
export class TooltipContent {
  readonly content = input.required<TemplateRef<unknown>>();
  readonly extraClass = input<string | undefined>(undefined);
  readonly panel = viewChild.required<TemplateRef<unknown>>('panel');
  protected readonly contentClass = computed(() =>
    cn(
      'bg-foreground text-background animate-in fade-in-0 zoom-in-95 data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 z-50 w-fit origin-(--radix-tooltip-content-transform-origin) rounded-md px-3 py-1.5 text-xs text-balance',
      this.extraClass(),
    ),
  );
}

@Directive({
  selector: '[uiTooltip]',
  host: {
    'data-slot': 'tooltip-trigger',
    '[attr.data-state]': 'open() ? "delayed-open" : "closed"',
    '(pointerenter)': 'scheduleOpen()',
    '(pointerleave)': 'close()',
    '(focus)': 'show()',
    '(blur)': 'close()',
  },
})
export class Tooltip {
  readonly uiTooltip = input.required<TemplateRef<unknown>>();
  readonly delayDuration = input(700, { transform: numberAttribute });
  readonly tooltipClass = input<string | undefined>(undefined);
  protected readonly open = signal(false);
  private readonly overlay = inject(Overlay);
  private readonly vcr = inject(ViewContainerRef);
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  private overlayRef: OverlayRef | null = null;
  private timer: ReturnType<typeof setTimeout> | null = null;

  constructor() {
    inject(DestroyRef).onDestroy(() => {
      if (this.timer) clearTimeout(this.timer);
      this.overlayRef?.dispose();
    });
  }

  protected scheduleOpen(): void {
    if (this.timer) clearTimeout(this.timer);
    this.timer = setTimeout(() => this.show(), this.delayDuration());
  }

  protected show(): void {
    if (this.open()) return;
    this.overlayRef ??= this.overlay.create({
      positionStrategy: this.overlay
        .position()
        .flexibleConnectedTo(this.el)
        .withPositions([{ originX: 'center', originY: 'top', overlayX: 'center', overlayY: 'bottom' }]),
    });
    const ref = this.vcr.createComponent(TooltipContent);
    ref.setInput('content', this.uiTooltip());
    ref.setInput('extraClass', this.tooltipClass());
    ref.changeDetectorRef.detectChanges();
    this.overlayRef.attach(new TemplatePortal(ref.instance.panel(), this.vcr));
    this.open.set(true);
  }

  protected close(): void {
    if (this.timer) clearTimeout(this.timer);
    this.timer = null;
    this.overlayRef?.detach();
    this.open.set(false);
  }
}
