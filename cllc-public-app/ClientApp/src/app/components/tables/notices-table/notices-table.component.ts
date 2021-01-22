import { DatePipe } from "@angular/common";
import { Component, OnInit, Input } from "@angular/core";
import { FileSystemItem } from "@models/file-system-item.model";

@Component({
  selector: "app-notices-table",
  templateUrl: "./notices-table.component.html",
  styleUrls: ["./notices-table.component.scss"]
})
export class NoticesTableComponent implements OnInit {
  @Input()
  rows: FileSystemItem[];

  constructor() {}

  ngOnInit() {
  }

  getLocale() {
    if (navigator.languages !== undefined) {
      return navigator.languages[0];
    } else {
      return navigator.language;
    }
  }

  // Formats dates based on user locale
  localDate(value: Date): string {
    if (value == null) {
      return "";
    }
    const dp = new DatePipe(this.getLocale());
    const dateFormat = "y-MM-dd"; // YYYY-MM-DD
    return dp.transform(new Date(value), "short");
  }

  // Converts bytes to KB
  bytesToKB(size: number): number {
    return Math.ceil(size / 1024);
  }
}
