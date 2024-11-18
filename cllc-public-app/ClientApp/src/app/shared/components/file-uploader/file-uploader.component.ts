import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from "@angular/core";
import { NgxFileDropEntry, FileSystemFileEntry, FileSystemDirectoryEntry } from "ngx-file-drop";
import { FileSystemItem } from "@models/file-system-item.model";
import { Subscription } from "rxjs";
import { ApplicationDataService } from "@services/application-data.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
export interface DropdownOption {
  id: string;
  name: string;
}

@Component({
  selector: "app-file-uploader",
  templateUrl: "./file-uploader.component.html",
  styleUrls: ["./file-uploader.component.scss"]
})
export class FileUploaderComponent implements OnInit, OnDestroy {
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
  disableUploads = false; // force uploads to be disabled
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
  @Input()
  isFloorPlanUploader = false;
  @Output()
  occupantLoadUpdated = new EventEmitter<number>();
  @Output()
  numberOfUploadedFiles = new EventEmitter<number>();
  busy: Subscription;
  attachmentURL: string;
  aiScanServiceUrl: string;
  actionPrefix: string;
  Math = Math;
  files: FileSystemItem[] = [];
  dataLoaded: boolean;
  fileReqOngoing: boolean;
  subscriptionList: Subscription[] = [];
  isLoading: boolean = false; // Loading spinner state
  apiValidationStatus: 'valid' | 'invalid' | 'expired' | null = null; // API result status
  validationMessage: string | null = null; // Error messages

  // TODO: move http call to a service
  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private adoxioApplicationDataService: ApplicationDataService) {
  }

  ngOnInit(): void {
    if (this.publicAccess) {
      this.actionPrefix = "public-";
    } else {
      this.actionPrefix = "";
    }
    this.attachmentURL = `api/file/${this.entityId}/${this.actionPrefix}attachments/${this.entityName}`;
    this.aiScanServiceUrl = "";
    console.log('aiScanServiceUrl', this.aiScanServiceUrl);
    this.getUploadedFileData();
  }

  dropped(event: NgxFileDropEntry[]) {
    // Clear validation state
    this.apiValidationStatus = null;
    this.validationMessage = null;
    const files = event;
    let newFileCount = 0;
    for (const droppedFile of files) {
      newFileCount += 1;
    }
    let count = this.getCurrentLastFileCounter() + 1;
    if (files.length > 1 && !this.multipleFiles) {
      this.snackBar.open("Only one file can be uploaded here",
        "Fail",
        { duration: 3500, panelClass: ["red-snackbar"] });
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

  validateOccupantLoadStamp(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    this.isLoading = true; // Show the existing loading bar

    this.http.post(this.aiScanServiceUrl, formData).subscribe({
      next: (response: any) => {
        this.isLoading = false; // Hide the loading bar
        const { date, occupancy_load, stamp_detected } = response;

        // Check response validity
        const currentDate = new Date();
        const stampDate = new Date(date);
        const isDateValid = currentDate.getTime() - stampDate.getTime() <= 365 * 24 * 60 * 60 * 1000;

        if (!stamp_detected) {
          this.apiValidationStatus = 'invalid';
          this.validationMessage = "The uploaded document did not contain an occupant load stamp.";
        } else if (!isDateValid) {
          this.apiValidationStatus = 'expired';
          this.validationMessage = "The uploaded document's occupancy load stamp was issued more than one year ago. A new stamp is required before you can proceed with your application.";
        } else {
          this.apiValidationStatus = 'valid';
          this.validationMessage = null;
          this.occupantLoadUpdated.emit(occupancy_load); // Emit occupant load to parent
        }
      },
      error: () => {
        this.isLoading = false; // Hide the loading bar
        this.apiValidationStatus = 'invalid';
        this.validationMessage = "Failed to validate the uploaded file.";
      },
    });
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
      this.snackBar.open("Only one file can be uploaded here",
        "Fail",
        { duration: 3500, panelClass: ["red-snackbar"] });
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

    input.value = "";
  }

  uploadFile(file: File, count: number) {
    // Clear validation state
    this.apiValidationStatus = null;
    this.validationMessage = null;
    if (this.isFloorPlanUploader) {
      this.isLoading = true; // Show the loading bar
    }

    const validExt = this.extensions.filter(ex => file.name.toLowerCase().endsWith(ex)).length > 0;
    if (!validExt) {
      this.isLoading = false; // Hide the loading bar if the file type is invalid
      this.snackBar.open("File type not supported.", "Fail", { duration: 3000, panelClass: ["red-snackbar"] });
      return;
    }

    if (file.name.length > 128) {
      this.isLoading = false; // Hide the loading bar if the file name is invalid
      this.snackBar.open("File name must be 128 characters or less.", "Fail", { duration: 3000, panelClass: ["red-snackbar"] });
      return;
    }

    const formData = new FormData();
    let fileName = file.name;
    const extension = file.name.match(/\.([^\.])+$/)[0];
    if (this.useDocumentTypeForName) {
      fileName = `${count}${extension}`;
    }
    formData.append("file", file, fileName);
    formData.append("documentType", this.documentType);

    // Validate occupant load stamp if it's a floor plan
    if (this.isFloorPlanUploader && this.aiScanServiceUrl && this.aiScanServiceUrl !== "") {
      this.validateOccupantLoadStamp(file);
    }

    const headers = new HttpHeaders();
    this.fileReqOngoing = true;

    this.http.post(this.attachmentURL, formData, { headers }).subscribe({
      next: () => {
        this.getUploadedFileData();
      },
      error: () => {
        this.snackBar.open("Failed to upload file.", "Fail", { duration: 3000, panelClass: ["red-snackbar"] });
        this.fileReqOngoing = false;
      },
    });
  }

  getUploadedFileData() {
    this.fileReqOngoing = true;
    const headers = new HttpHeaders({
      // 'Content-Type': 'application/json'

    });
    const getFileURL = this.attachmentURL + "/" + this.documentType;
    const sub = this.http.get<FileSystemItem[]>(getFileURL, { headers: headers })
      .subscribe((data) => {
        data.forEach(file => {
          if (this.useDocumentTypeForName) {
            file.name = this.documentType + "_" + file.name;
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
        this.subscriptionList.push(sub);
        // this.busy = sub;

        // convert bytes to KB
        data.forEach((entry) => {
          entry.size = Math.ceil(entry.size / 1024);
          entry.downloadUrl =
            `api/file/${this.entityId}/${this.actionPrefix}download-file/${this.entityName}/${entry.name}`;
          entry.downloadUrl += `?serverRelativeUrl=${encodeURIComponent(entry.serverrelativeurl)}&documentType=${this
            .documentType}`;
        });
        this.files = data;
        this.numberOfUploadedFiles.emit(this.files.length);
        this.dataLoaded = true;
        this.fileReqOngoing = false;
      },
        err => {
          this.snackBar.open("Failed to get files", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
          this.fileReqOngoing = false;
        });
    this.subscriptionList.push(sub);
  }

  deleteFile(relativeUrl: string) {
    // Clear validation state
    this.apiValidationStatus = null;
    this.validationMessage = null;
    this.fileReqOngoing = true;
    const headers = new HttpHeaders({
      'Content-Type': "application/json"
    });
    const queryParams = `?serverRelativeUrl=${encodeURIComponent(relativeUrl)}&documentType=${this.documentType}`;
    const sub = this.http.delete(this.attachmentURL + queryParams, { headers: headers }).subscribe(result => {
      this.getUploadedFileData();
    },
      err => {
        this.snackBar.open("Failed to delete file", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        this.fileReqOngoing = false;
      });

    this.subscriptionList.push(sub);
    // this.busy = sub;
  }

  disableFileUpload(): boolean {
    return (!this.multipleFiles && (this.files && this.files.length > 0)) ||
      (this.multipleFiles && this.maxNumberOfFiles <= (this.files.length)) ||
      this.disableUploads;
  }

  fileOver(event) {
    // console.log(event);
  }

  fileLeave(event) {
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

  ngOnDestroy() {
    this.subscriptionList.forEach(sub => sub.unsubscribe());
  }
}
