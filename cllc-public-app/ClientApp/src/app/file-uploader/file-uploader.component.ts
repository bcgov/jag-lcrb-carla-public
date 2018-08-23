import { Component, OnInit, Input } from '@angular/core';
import { UploadFile, UploadEvent, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Http, Headers, Response } from '@angular/http';
import { FileSystemItem } from '../models/file-system-item.model';
import { Observable, Subject } from 'rxjs';
import { throttleTime, catchError } from 'rxjs/operators';
import { debug } from 'util';
import { forEach } from '@angular/router/src/utils/collection';
import { Subscription } from 'rxjs';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { saveAs } from "file-saver";

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
  @Input() multipleFiles: boolean = true;
  @Input() extensions: string[] = ['pdf'];
  busy: Subscription;
  attachmentURL: string;
  Math = Math;
  public files: FileSystemItem[] = [];

  //TODO: move http call to a service
  constructor(private http: Http, private adoxioApplicationDataService: AdoxioApplicationDataService) {
  }

  ngOnInit(): void {
    this.attachmentURL =`api/file/${this.entityId}/attachments/${this.entityName}`;

    this.getUploadedFileData();
  }

  public dropped(event: UploadEvent) {
    let files = event.files;
    if (files.length > 1 && !this.multipleFiles) {
      alert('Only one file can be uploaded here');
      return;
    }
    for (var droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        let fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          this.uploadFile(file);
        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        let fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        //console.log(droppedFile.relativePath, fileEntry);
      }
    }
  }

  onBrowserFileSelect(event: any) {
    let uploadedFiles = event.target.files;
    for (const file of uploadedFiles) {
      this.uploadFile(file);
    }
  }

  private uploadFile(file) {
    let validExt = this.extensions.filter(ex => file.name.toLowerCase().endsWith(ex)).length > 0;
    if (!validExt) {
      alert("File type not supported.")
      return;
    }
    let formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('documentType', this.documentType);
    let headers = new Headers();
    //let url = "";
    //url = this.attachmentURL + this.applicationId + "/attachments";
    this.busy = this.http.post(this.attachmentURL, formData, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    },
      err => alert('Failed to upload file'));
  }

  getUploadedFileData() {
    const headers = new Headers({
      // 'Content-Type': 'multipart/form-data'
    });
    const getFileURL = this.attachmentURL + '/' + this.documentType;
    this.busy = this.http.get(getFileURL, { headers: headers })
      .map((data: Response) => { return <FileSystemItem[]>data.json() })
      .subscribe((data) => {
        // convert bytes to KB
        data.forEach((entry) => {
          entry.size = Math.ceil(entry.size / 1024)
        });
        this.files = data;
      },
        err => alert('Failed to get files'));
  }

  deleteFile(relativeUrl: string) {
    const headers = new Headers({
      'Content-Type': 'application/json'
    });
    const queryParams = `?serverRelativeUrl=${encodeURIComponent(relativeUrl)}`;
    this.busy = this.http.delete(this.attachmentURL + queryParams, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    },
      err => alert('Failed to delete file'));
  }

  disableFileUpload(): boolean {
    return !this.multipleFiles && (this.files && this.files.length > 0);
  }

  public fileOver(event) {
    //console.log(event);
  }

  public fileLeave(event) {
    //console.log(event);
  }

  downloadApplicationPDF(url: string, fileName: string) {
    if (this.entityId) {
      this.adoxioApplicationDataService.downloadFile(url, this.entityId)
        .subscribe((res: Blob) => {
          saveAs(res, fileName);
        },
          err => alert('Failed to download file'));
    }
  }

  browseFiles(browserMultiple, browserSingle) {
    if (!this.disableFileUpload()) {
      if (this.multipleFiles) {
        browserMultiple.click()
      } else {
        browserSingle.click();
      }
    }
  }
}

