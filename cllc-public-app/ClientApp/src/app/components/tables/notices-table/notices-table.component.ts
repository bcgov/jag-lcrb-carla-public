import { Component, OnInit, Input } from '@angular/core';
import { FileSystemItem } from '@models/file-system-item.model';

@Component({
  selector: 'app-notices-table',
  templateUrl: './notices-table.component.html',
  styleUrls: ['./notices-table.component.scss']
})
export class NoticesTableComponent implements OnInit {
  @Input() rows: FileSystemItem[];

  constructor() { }

  ngOnInit() {
  }

  // TODO: Fix to proper link
  downloadLink(notice: any): string {
    return `api/licenceevents/${notice.id}/${notice.filename}.pdf`;
  }
}
