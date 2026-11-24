import { Injectable } from '@angular/core';
import { uploadDocument, completeScan, recordDownload, removeDocument } from './document-service';
import { validateFile } from '@domain/documents/document-service';
import { injectDocuments } from './documents.signals';
import type { DocumentsRepository } from './documents.repository';

/** Mock DocumentsRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockDocumentsRepository implements DocumentsRepository {
  readonly documents = injectDocuments;

  readonly upload = uploadDocument;

  readonly completeScan = completeScan;

  readonly recordDownload = recordDownload;

  readonly remove = removeDocument;

  readonly validateFile = validateFile;

  fetchApi(path: `/api/${string}`, init?: RequestInit): Promise<Response> {
    return fetch(path, init);
  }
}
