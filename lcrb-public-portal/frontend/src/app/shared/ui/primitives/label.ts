/** Label styles (shadcn new-york). */
import { Directive } from '@angular/core';
import { ClassMerge } from '@shared/util/class-merge';

@Directive({ selector: 'label[uiLabel]', host: { 'data-slot': 'label' } })
export class Label extends ClassMerge {
  protected baseClass() {
    return 'flex items-center gap-2 text-sm leading-none font-medium select-none group-data-[disabled=true]:pointer-events-none group-data-[disabled=true]:opacity-50 peer-disabled:cursor-not-allowed peer-disabled:opacity-50';
  }
}
