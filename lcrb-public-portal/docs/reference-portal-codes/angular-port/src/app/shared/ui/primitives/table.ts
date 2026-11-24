import { ChangeDetectionStrategy, Component, computed, Directive, input } from '@angular/core';
import { cn } from '@shared/util/utils';
import { ClassMerge } from '@shared/util/class-merge';

/** Renders the prototype's container div and table; the host uses `display: contents` so layout matches. */
@Component({
  selector: 'ui-table',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<div data-slot="table-container" class="relative w-full overflow-x-auto">
    <table data-slot="table" [class]="tableClass()"><ng-content /></table>
  </div>`,
})
export class Table {
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly tableClass = computed(() => cn('w-full caption-bottom text-sm', this.userClass()));
}

@Directive({ selector: 'thead[uiTableHeader]', host: { 'data-slot': 'table-header' } })
export class TableHeader extends ClassMerge {
  protected baseClass() {
    return '[&_tr]:border-b';
  }
}

@Directive({ selector: 'tbody[uiTableBody]', host: { 'data-slot': 'table-body' } })
export class TableBody extends ClassMerge {
  protected baseClass() {
    return '[&_tr:last-child]:border-0';
  }
}

@Directive({ selector: 'tfoot[uiTableFooter]', host: { 'data-slot': 'table-footer' } })
export class TableFooter extends ClassMerge {
  protected baseClass() {
    return 'bg-muted/50 border-t font-medium [&>tr]:last:border-b-0';
  }
}

@Directive({ selector: 'tr[uiTableRow]', host: { 'data-slot': 'table-row' } })
export class TableRow extends ClassMerge {
  protected baseClass() {
    return 'hover:bg-muted/50 data-[state=selected]:bg-muted border-b transition-colors';
  }
}

@Directive({ selector: 'th[uiTableHead]', host: { 'data-slot': 'table-head' } })
export class TableHead extends ClassMerge {
  protected baseClass() {
    return 'text-foreground h-10 px-2 text-left align-middle font-medium whitespace-nowrap [&:has([role=checkbox])]:pr-0 [&>[role=checkbox]]:translate-y-[2px]';
  }
}

@Directive({ selector: 'td[uiTableCell]', host: { 'data-slot': 'table-cell' } })
export class TableCell extends ClassMerge {
  protected baseClass() {
    return 'p-2 align-middle whitespace-nowrap [&:has([role=checkbox])]:pr-0 [&>[role=checkbox]]:translate-y-[2px]';
  }
}

@Directive({ selector: 'caption[uiTableCaption]', host: { 'data-slot': 'table-caption' } })
export class TableCaption extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground mt-4 text-sm';
  }
}

export const TABLE = [Table, TableHeader, TableBody, TableFooter, TableRow, TableHead, TableCell, TableCaption] as const;
