import { InjectionToken } from '@angular/core';
import type { uploadDocument, completeScan, recordDownload, removeDocument } from './document-service';
import type { validateFile } from '@domain/documents/document-service';
import type { injectDocuments } from './documents.signals';
import { MockDocumentsRepository } from './documents.repository.mock';

/** Documents repository (backend area: 6.11 Documents). */
export interface DocumentsRepository {
  readonly documents: typeof injectDocuments;
  readonly upload: typeof uploadDocument;
  readonly completeScan: typeof completeScan;
  readonly recordDownload: typeof recordDownload;
  readonly remove: typeof removeDocument;
  readonly validateFile: typeof validateFile;
  /** The document API routes (`/api/upload`, `/api/document-check`). */
  fetchApi(path: `/api/${string}`, init?: RequestInit): Promise<Response>;
}

export const DOCUMENTS_REPOSITORY = new InjectionToken<DocumentsRepository>('DocumentsRepository', {
  providedIn: 'root',
  factory: () => new MockDocumentsRepository(),
});
