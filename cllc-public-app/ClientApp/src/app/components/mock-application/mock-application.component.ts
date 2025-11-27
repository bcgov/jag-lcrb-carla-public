import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

type UploadStatus = 'ok' | 'missing';

@Component({
  selector: 'app-mock-application',
  templateUrl: './mock-application.component.html',
  styleUrls: ['./mock-application.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MockApplicationComponent {
  @Input() schema: any;
  @Input() values: Record<string, any> = {};
  @Input() uploadStatuses: Record<string, UploadStatus> = {};

  @Input() activeFieldId? : string;

  @Output() valueChange = new EventEmitter<{ id: string; value: any }>();
  @Output() uploadRequested = new EventEmitter<string>();
  @Output() submitRequested = new EventEmitter<void>();

  readonly days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  // --- helpers for layouts
  isUploads(sec: any) { return (sec?.layout || '').toLowerCase() === 'uploads'; }
  isHours(sec: any) { return (sec?.layout || '').toLowerCase() === 'hours'; }
  isGrid(sec: any) { return !this.isUploads(sec) && !this.isHours(sec); }

  // --- values api
  getVal(id: string, fallback: any = '') { return this.values?.[id] ?? fallback; }

  setVal(id: string, value: any) {
    // emit change (parent persists and mirrors back)
    this.valueChange.emit({ id, value });
  }

  // --- hours mapping (serviceHours<Day><Open|Close>)
  private hourId(day: string, kind: 'open' | 'close') {
    return `serviceHours${day}${kind === 'open' ? 'Open' : 'Close'}`;
    // e.g. Sunday + open => serviceHoursSundayOpen
  }

  getHour(day: string, kind: 'open' | 'close') {
    return this.values?.[this.hourId(day, kind)] || '';
  }

  setHours(day: string, kind: 'open' | 'close', raw: any) {
    const id = this.hourId(day, kind);
    const value = String(raw ?? '').trim();
    this.valueChange.emit({ id, value });
  }

  // --- styling helpers
  statusBadgeClass(id: string) {
    const ok = this.uploadStatuses?.[id] === 'ok';
    return `badge ${ok ? 'ok' : 'missing'}`;
  }

  // flexible column width based on schema span (12 default)
  colClass(span?: number) {
    const s = span ?? 12;
    return {
      'col': true,
      'col-12': s === 12,
      'col-6': s === 6,
      'col-4': s === 4,
      'col-3': s === 3
    };
  }

  trackById = (_: number, f: any) => f?.id || f?.label || _;
}
