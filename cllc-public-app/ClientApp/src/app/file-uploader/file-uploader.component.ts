import { Component, OnInit, Input } from '@angular/core';
import { UploadFile, UploadEvent, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Http, Headers, Response } from '@angular/http';
import { FileSystemItem } from '../models/file-system-item.model';


export interface DropdownOption{
  id: string,
  name: string
}

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit {
  selectedDocumentType: DropdownOption;
  @Input() accountId: string;
  @Input() uploadUrl: string;

  _documentTypes: DropdownOption[];
  get documentTypes(){
    return this._documentTypes;
  }
  @Input() set documentTypes(types: DropdownOption[]){
    this._documentTypes = types;
    if(types && types.length  == 1){
      this.selectedDocumentType  = types[0];
    }
  };
  
  //TODO: move http call to a service
  constructor(private http: Http) {
  }

  ngOnInit(): void {
  }

  public files: FileSystemItem[] = [];

  public dropped(event: UploadEvent) {
    let files = event.files;
    for (const droppedFile of files) {

      // Is it a file?
      if (droppedFile.fileEntry.isFile) {
        const fileEntry = droppedFile.fileEntry as FileSystemFileEntry;
        fileEntry.file((file: File) => {

          // Here you can access the real file
          console.log(droppedFile.relativePath, file);

          /**
          // You could upload it like this:
          **/
          const formData = new FormData()
          formData.append('file', file, droppedFile.relativePath)
          formData.append('file', file, droppedFile.relativePath)

          // Headers
          const headers = new Headers({
            //'Content-Type': 'multipart/form-data'
          })

          this.http.post('api/AdoxioLegalEntity/id/attachments', formData, { headers: headers })
            .subscribe(data => {
              // Sanitized logo returned from backend
            })

        });
      } else {
        // It was a directory (empty directories are added, otherwise only files)
        const fileEntry = droppedFile.fileEntry as FileSystemDirectoryEntry;
        console.log(droppedFile.relativePath, fileEntry);
      }
    }
  }

  onBrowserFileSelect(event: any) {
    let uploadedFiles = event.target.files;
    for (const file of uploadedFiles) {
      console.log(file.path, file);

    }
  }

  getUploadedFileData() {
    const headers = new Headers({
      //'Content-Type': 'multipart/form-data'
    })
    let id = "some-id";
    this.http.get(`api/AdoxioLegalEntity/${id}/attachments`, { headers: headers })
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
