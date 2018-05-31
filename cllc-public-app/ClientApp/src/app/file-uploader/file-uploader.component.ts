import { Component, OnInit } from '@angular/core';
import { UploadFile, UploadEvent, FileSystemFileEntry, FileSystemDirectoryEntry } from 'ngx-file-drop';
import { Http, Headers } from '@angular/http';

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent implements OnInit {

  //TODO: move http call to a service
  constructor(private http: Http) {
  }

  ngOnInit(): void {
  }

  public files: UploadFile[] = [];

  public dropped(event: UploadEvent) {
    let files = event.files;
    this.files = files;
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

  onBrowserFileSelect(event: any){
    let files = event.target.files;
    this.files = files;
    for (const file of files) {
      console.log(file.path, file);
      
    }
  }


  public fileOver(event) {
    console.log(event);
  }

  public fileLeave(event) {
    console.log(event);
  }

}
