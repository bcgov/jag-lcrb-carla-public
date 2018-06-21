import { Component, OnInit, Input } from '@angular/core';
import { UploadFile, UploadEvent, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Http, Headers, Response } from '@angular/http';
import { FileSystemItem } from '../models/file-system-item.model';
import { Observable, Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators';
import { debug } from 'util';


export interface DropdownOption {
  id: string,
  name: string
}

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit {
  @Input() accountId: string;
  @Input() uploadUrl: string;
  @Input() documentType: string;

  //TODO: move http call to a service
  constructor(private http: Http) {
  }

  ngOnInit(): void {
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

  private uploadFile(file){
    let formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('documentType', this.documentType);
    let headers = new Headers();
    let url = `api/AdoxioLegalEntity/${this.accountId}/attachments`;
    this.http.post(url, formData, { headers: headers }).subscribe(result => {
    });
  }

  getUploadedFileData() {
    const headers = new Headers({
      //'Content-Type': 'multipart/form-data'
    })
    this.http.get(`api/AdoxioLegalEntity/${this.accountId}/attachments/${this.documentType}`, { headers: headers })
      .map((data: Response) => { return <FileSystemItem[]>data.json() })
      .subscribe((data) => {
        this.files = data;
      })
  }

  public fileOver(event) {
    console.log(event);
  }

  public fileLeave(event) {
    console.log(event);
  }

}
