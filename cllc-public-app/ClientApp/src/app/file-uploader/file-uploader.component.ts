import { Component, OnInit, Input } from '@angular/core';
import { UploadFile, UploadEvent, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Http, Headers, Response } from '@angular/http';
import { FileSystemItem } from '../models/file-system-item.model';
import { Observable, Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators';
import { debug } from 'util';
import { forEach } from '@angular/router/src/utils/collection';
import { Subscription } from 'rxjs';

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
  @Input() accountId: string;
  @Input() uploadUrl: string;
  @Input() fileTypes = '';
  @Input() documentType: string;
  @Input() applicationId: string;
  @Input() multipleFiles: boolean = true;
  busy: Subscription;
  attachmentURL: string;
  Math = Math;

  //TODO: move http call to a service
  constructor(private http: Http) {
  }

  ngOnInit(): void {
    //use application controller if application ID is passed, otherwise legal entity controller
    if (this.applicationId) {
      this.attachmentURL = 'api/adoxioapplication/' + this.applicationId + '/attachments';
    } else {
      this.attachmentURL = 'api/AdoxioLegalEntity/' + this.accountId + '/attachments';
    }
    this.getUploadedFileData();
  }

  public files: FileSystemItem[] = [];


  public dropped(event: UploadEvent) {
    let files = event.files;

    for (var droppedFile of files) {
      if (droppedFile.fileEntry.isFile) {
        let fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {
          this.uploadFile(file);
        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        let fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        console.log(droppedFile.relativePath, fileEntry);
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
    let formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('documentType', this.documentType);
    let headers = new Headers();
    //let url = "";
    //url = this.attachmentURL + this.applicationId + "/attachments";
    this.busy = this.http.post(this.attachmentURL, formData, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    });
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
          entry.size = entry.size / 1024
        });
        this.files = data;
      });
  }

  deleteFile(relativeUrl: string) {
    const headers = new Headers({
      'Content-Type': 'application/json'
    });
    const queryParams = `?serverRelativeUrl=${encodeURIComponent(relativeUrl)}`;
    this.busy = this.http.delete(this.attachmentURL + queryParams, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    });
  }

  disableFileUpload(): boolean {
    return !this.multipleFiles && (this.files && this.files.length > 0);
  }

  public fileOver(event) {
    console.log(event);
  }

  public fileLeave(event) {
    console.log(event);
  }

}
