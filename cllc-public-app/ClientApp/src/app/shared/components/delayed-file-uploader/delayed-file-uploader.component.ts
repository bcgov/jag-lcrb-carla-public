import { Component, Input, Output, OnDestroy, OnInit, EventEmitter } from "@angular/core";
import { Store } from "@ngrx/store";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import { NgxFileDropEntry, FileSystemFileEntry, FileSystemDirectoryEntry } from "ngx-file-drop";

import * as FileUploadsActions from "../../../app-state/actions/file-uploads.action";
import { AppState } from "../../../app-state/models/app-state";

import { FileItem } from "../../../models/file-item.model";

export interface DropdownOption {
  id: string;
  name: string;
}

@Component({
  selector: "app-delayed-file-uploader",
  templateUrl: "./delayed-file-uploader.component.html",
  styleUrls: ["./delayed-file-uploader.component.scss"]
})
export class DelayedFileUploaderComponent implements OnInit, OnDestroy {
  unsubscribe: Subject<void> = new Subject();


  @Input()
  id: string;
  @Input()
  uploadUrl: string;
  @Input()
  fileTypes = "";
  @Input()
  documentType: string;
  @Input()
  entityName: string;
  @Input()
  entityId: string;
  @Input()
  multipleFiles = true;
  @Input()
  extensions: string[] = ["pdf"];
  @Input()
  uploadHeader = "TO UPLOAD DOCUMENTS, DRAG FILES HERE OR";
  @Input()
  enableFileDeletion = true;
  @Input()
  maxNumberOfFiles = 10;
  @Input()
  useDocumentTypeForName = false;
  @Input()
  publicAccess = false;

  @Output()
  numberOfUploadedFiles = new EventEmitter<number>();

  fileSizeLimit = 1048576 * 25; // 25 MB
  fileSizeLimitReadable = "25 MB";

  validationErrors: string[] = [];

  files: FileItem[] = [];

  constructor(private store: Store<AppState>) {}

  ngOnInit() {


    // subscribe to files from store
    this.store.select(state => state.fileUploadsState.fileUploads)
      .pipe(
        takeUntil(this.unsubscribe),
      ).subscribe(fileUploads => {
        const fileUploadSet = fileUploads.find(f => f.id === this.id && f.documentType === this.documentType);
        this.files = fileUploadSet ? fileUploadSet.files : [];
      });
  }

  ngOnDestroy() {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

  dropped(event: NgxFileDropEntry[]) {
    const droppedFiles = event;
    let newDroppedFileCount = 0;
    for (const droppedFile of droppedFiles) {
      newDroppedFileCount += 1;
    }
    if (droppedFiles.length > 1 && !this.multipleFiles) {
      alert("Only one file can be uploaded here");
      return;
    }
    if (this.maxNumberOfFiles < (this.files.length + newDroppedFileCount)) {
      alert(`Only ${this.maxNumberOfFiles} files can be uploaded here`);
      return;
    }
    for (const droppedFile of droppedFiles) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          this.addFile(file);
        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        const fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        // console.log(droppedFile.relativePath, fileEntry);
      }
    }


  }

  getCurrentLastFileCounter(): number {
    let lastCount = 0;
    if (this.files.length) {
      const counts = this.files.map(file => {
          const match = file.name.match(/_(\d+)\.([^\.]+)$/);
          if (match) {
            return parseInt(match[1], 10);
          } else {
            return 0;
          }

        }).sort()
        .reverse();
      lastCount = counts[0];
    }
    return lastCount;
  }


  onBrowserFileSelect(event: any, input: any) {
    this.validationErrors = [];

    const uploadedFiles = event.target.files;
    for (const file of uploadedFiles) {
      if (this.validateFile(file)) {
        this.addFile(file);
      }
    }

    input.value = "";
  }

  validateFile(file: File): boolean {
    const validExt = this.extensions.filter(ex => file.name.toLowerCase().endsWith(`.${ex}`)).length > 0;
    if (!validExt) {
      this.validationErrors.push(`File type not supported. <em>[${file.name}]</em>`);
      return false;
    }

    if (file && file.name && file.name.length > 128) {
      this.validationErrors.push(`File name must be 128 characters or less. <em>[${file.name}]</em>`);
      return false;
    }

    if (file && file.size && file.size > this.fileSizeLimit) {
      const limit = this.fileSizeLimitReadable;
      this.validationErrors.push(
        `The specified file exceeds the maximum file size of ${limit}. <em>[${file.name}]</em>`);
      return false;
    }

    if (this.maxNumberOfFiles && this.files.length >= this.maxNumberOfFiles) {
      this.validationErrors.push(
        `File limit has been reached. The specified file has not been added. <em>[${file.name}]</em>`);
      return false;
    }

    return true;
  }

  disableFileUpload(): boolean {
    return (!this.multipleFiles && (this.files && this.files.length > 0)) ||
      (this.multipleFiles && this.maxNumberOfFiles <= (this.files.length));
  }

  addFile(file: File) {
    const fileSystemEntry = { id: this.files.length, name: file.name, size: Math.trunc(file.size / 1024), file: file };
    const fileSet = {
      id: this.id,
      documentType: this.documentType,
      files: [...this.files, fileSystemEntry]
    };
    this.store.dispatch(new FileUploadsActions.SetFileUploadsAction(fileSet));
  }

  removeFile(file: FileItem) {
    this.store.dispatch(new FileUploadsActions.SetFileUploadsAction({
      id: this.id,
      documentType: this.documentType,
      files: this.files.filter(f => f.id !== file.id)
    }));
  }

  browseFiles(browserMultiple: HTMLInputElement) {
    browserMultiple.click();
  }
}
