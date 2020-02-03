import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NgxFileDropEntry, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { FileSystemItem } from '@models/file-system-item.model';
import { Subscription } from 'rxjs';
import { ApplicationDataService } from '@services/application-data.service';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';

export interface DropdownOption {
  id: string;
  name: string;
}

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit {
  @Input() uploadUrl: string;
  @Input() fileTypes = '';
  @Input() documentType: string;
  @Input() entityName: string;
  @Input() entityId: string;
  @Input() multipleFiles = true;
  @Input() extensions: string[] = ['pdf'];
  @Input() uploadHeader = 'TO UPLOAD DOCUMENTS, DRAG FILES HERE OR';
  @Input() enableFileDeletion = true;
  @Input() maxNumberOfFiles = 10;
  @Input() useDocumentTypeForName = false;
  @Input() publicAccess = false;
  @Output() numberOfUploadedFiles: EventEmitter<number> = new EventEmitter<number>();
  busy: Subscription;
  attachmentURL: string;
  actionPrefix: string;
  Math = Math;
  public files: FileSystemItem[] = [];

  // TODO: move http call to a service
  constructor(private http: HttpClient, private adoxioApplicationDataService: ApplicationDataService) {
  }

  ngOnInit(): void {
    if (this.publicAccess) {
      this.actionPrefix = "public-";
    }
    else {
      this.actionPrefix = "";
    }
    this.attachmentURL = `api/file/${this.entityId}/${this.actionPrefix}attachments/${this.entityName}`;
    this.getUploadedFileData();
  }

  public dropped(event: NgxFileDropEntry[]) {
    const files = event;
    let newFileCount = 0;
    for (const droppedFile of files) {
      newFileCount += 1;
    }
    let count = this.getCurrentLastFileCounter() + 1;
    if (files.length > 1 && !this.multipleFiles) {
      alert('Only one file can be uploaded here');
      return;
    }
    if (this.maxNumberOfFiles < (this.files.length + newFileCount)) {
      alert(`Only ${this.maxNumberOfFiles} files can be uploaded here`);
      return;
    }
    for (const droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          this.uploadFile(file, count);
          count += 1;
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
    const uploadedFiles = event.target.files;
    let newFileCount = 0;
    for (const droppedFile of uploadedFiles) {
      newFileCount += 1;
    }
    let count = this.getCurrentLastFileCounter() + 1;
    if (uploadedFiles.length > 1 && !this.multipleFiles) {
      alert('Only one file can be uploaded here');
      return;
    }
    if (this.maxNumberOfFiles < (this.files.length + newFileCount)) {
      alert(`Only ${this.maxNumberOfFiles} files can be uploaded here`);
      return;
    }
    for (const file of uploadedFiles) {
      this.uploadFile(file, count);
      count += 1;
    }

    input.value = '';
  }

  public uploadFile(file, count) {
    const validExt = this.extensions.filter(ex => file.name.toLowerCase().endsWith(ex)).length > 0;
    if (!validExt) {
      alert('File type not supported.');
      return;
    }

    if (file && file.name && file.name.length > 128) {
      alert('File name must be 128 characters or less.');
      return;
    }

    const formData = new FormData();
    let fileName = file.name;
    const extension = file.name.match(/\.([^\.])+$/)[0];
    if (this.useDocumentTypeForName) {
      fileName = (count) + extension;
    }
    formData.append('file', file, fileName);
    formData.append('documentType', this.documentType);
    const headers: HttpHeaders = new HttpHeaders();

    this.busy = this.http.post(this.attachmentURL, formData, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    },
      err => alert('Failed to upload file'));
  }

  getUploadedFileData() {
    const headers: HttpHeaders = new HttpHeaders({
      // 'Content-Type': 'application/json'
    });
    const getFileURL = this.attachmentURL + '/' + this.documentType;
    this.busy = this.http.get<FileSystemItem[]>(getFileURL, { headers: headers })
      .subscribe((data) => {
        data.forEach(file => {
          if (this.useDocumentTypeForName) {
            file.name = this.documentType + '_' + file.name;
          }
        });
        // sort by filename
        data = data.sort((fileA, fileB) => {
          if (fileA.name > fileB.name) {
            return 1;
          } else {
            return -1;
          }
        });

        // convert bytes to KB
        data.forEach((entry) => {
          entry.size = Math.ceil(entry.size / 1024);
          entry.downloadUrl = `api/file/${this.entityId}/${this.actionPrefix}download-file/${this.entityName}/${entry.name}`;
          entry.downloadUrl += `?serverRelativeUrl=${encodeURIComponent(entry.serverrelativeurl)}&documentType=${this.documentType}`;
        });
        this.files = data;
        this.numberOfUploadedFiles.emit(this.files.length);
      },
        err => alert('Failed to get files'));
  }

  deleteFile(relativeUrl: string) {
    const headers: HttpHeaders = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    const queryParams = `?serverRelativeUrl=${encodeURIComponent(relativeUrl)}&documentType=${this.documentType}`;
    this.busy = this.http.delete(this.attachmentURL + queryParams, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    },
      err => alert('Failed to delete file'));
  }

  disableFileUpload(): boolean {
    return (!this.multipleFiles && (this.files && this.files.length > 0))
      || (this.multipleFiles && this.maxNumberOfFiles <= (this.files.length));
  }

  public fileOver(event) {
    // console.log(event);
  }

  public fileLeave(event) {
    // console.log(event);
  }

  browseFiles(browserMultiple, browserSingle) {
    if (!this.disableFileUpload()) {
      if (this.multipleFiles) {
        browserMultiple.click();
      } else {
        browserSingle.click();
      }
    }
  }
}
