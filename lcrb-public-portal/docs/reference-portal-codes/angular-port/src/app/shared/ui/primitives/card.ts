import { Directive } from '@angular/core';
import { ClassMerge } from '@shared/util/class-merge';

@Directive({ selector: 'div[uiCard]', host: { 'data-slot': 'card' } })
export class Card extends ClassMerge {
  protected baseClass() {
    return 'bg-card text-card-foreground flex flex-col gap-6 rounded-xl border py-6 shadow-sm';
  }
}

@Directive({ selector: 'div[uiCardHeader]', host: { 'data-slot': 'card-header' } })
export class CardHeader extends ClassMerge {
  protected baseClass() {
    return '@container/card-header grid auto-rows-min grid-rows-[auto_auto] items-start gap-2 px-6 has-data-[slot=card-action]:grid-cols-[1fr_auto] [.border-b]:pb-6';
  }
}

@Directive({ selector: 'div[uiCardTitle]', host: { 'data-slot': 'card-title' } })
export class CardTitle extends ClassMerge {
  protected baseClass() {
    return 'leading-none font-semibold';
  }
}

@Directive({ selector: 'div[uiCardDescription]', host: { 'data-slot': 'card-description' } })
export class CardDescription extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground text-sm';
  }
}

@Directive({ selector: 'div[uiCardAction]', host: { 'data-slot': 'card-action' } })
export class CardAction extends ClassMerge {
  protected baseClass() {
    return 'col-start-2 row-span-2 row-start-1 self-start justify-self-end';
  }
}

@Directive({ selector: 'div[uiCardContent]', host: { 'data-slot': 'card-content' } })
export class CardContent extends ClassMerge {
  protected baseClass() {
    return 'px-6';
  }
}

@Directive({ selector: 'div[uiCardFooter]', host: { 'data-slot': 'card-footer' } })
export class CardFooter extends ClassMerge {
  protected baseClass() {
    return 'flex items-center px-6 [.border-t]:pt-6';
  }
}

export const CARD = [Card, CardHeader, CardTitle, CardDescription, CardAction, CardContent, CardFooter] as const;
