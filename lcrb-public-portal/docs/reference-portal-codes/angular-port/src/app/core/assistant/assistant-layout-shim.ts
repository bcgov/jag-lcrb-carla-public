/** With the assistant open on a large screen, the portal content shifts left by the panel width (28rem). */
import { ChangeDetectionStrategy, Component, computed } from '@angular/core';
import { cn } from '@shared/util/utils';
import { injectAssistant } from './assistant-provider';

@Component({
  selector: 'assistant-layout-shim',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<div [class]="shimClass()"><ng-content /></div>`,
})
export class AssistantLayoutShim {
  private readonly assistant = injectAssistant();
  protected readonly shimClass = computed(() =>
    cn('min-h-screen transition-[padding] duration-300 ease-out', this.assistant.open() && 'lg:pr-[28rem]'),
  );
}
