import { FileItem } from "./file-item.model";

export class FileUploadSet {
  id: string;
  documentType: string;
  files: FileItem[];
}
