/** PDF upload to /api/upload, one file at a time. */
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { DOCUMENTS_REPOSITORY, type DocumentsRepository } from '@data-access/repositories/documents/documents.repository';

interface UploadedFile {
  pathname: string;
  filename: string;
  size: number;
}

async function uploadFile(file: File, documents: DocumentsRepository) {
  const formData = new FormData();
  formData.append('file', file);

  const response = await documents.fetchApi('/api/upload', {
    method: 'POST',
    body: formData,
  });

  if (!response.ok) {
    throw new Error(`Failed to upload ${file.name}`);
  }

  return response.json();
}

@Component({
  selector: 'upload-page',
  imports: [Card, CardContent, CardDescription, CardHeader, CardTitle, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen bg-background p-8">
      <div class="mx-auto max-w-2xl space-y-6">
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Upload Background Documents</div>
            <div uiCardDescription>
              Upload your PDF documents for the LCRB Portal Redesign project. These will be stored securely and used
              as reference material.
            </div>
          </div>
          <div uiCardContent class="space-y-4">
            <div
              (drop)="handleDrop($event)"
              (dragover)="handleDragOver($event)"
              (dragleave)="handleDragLeave($event)"
              [attr.class]="dropClass()"
            >
              <input
                type="file"
                accept=".pdf"
                multiple
                (change)="onChange($event)"
                class="absolute inset-0 cursor-pointer opacity-0"
                [disabled]="uploading()"
              />
              @if (uploading()) {
                <svg lucide="Loader2" class="mb-4 h-10 w-10 animate-spin text-primary"></svg>
                <p class="text-sm text-muted-foreground">Uploading...</p>
              } @else {
                <svg lucide="Upload" class="mb-4 h-10 w-10 text-muted-foreground"></svg>
                <p class="mb-1 text-sm font-medium">Drop PDF files here or click to browse</p>
                <p class="text-xs text-muted-foreground">PDF files only</p>
              }
            </div>

            @if (error()) {
              <div class="flex items-center gap-2 rounded-md bg-destructive/10 p-3 text-sm text-destructive">
                <svg lucide="X" class="h-4 w-4"></svg>
                {{ error() }}
              </div>
            }
          </div>
        </div>

        @if (files().length > 0) {
          <div uiCard>
            <div uiCardHeader>
              <div uiCardTitle class="flex items-center gap-2">
                <svg lucide="Check" class="h-5 w-5 text-green-600"></svg>
                Uploaded Documents ({{ files().length }})
              </div>
            </div>
            <div uiCardContent>
              <ul class="space-y-2">
                @for (file of files(); track $index) {
                  <li class="flex items-center gap-3 rounded-md border p-3">
                    <svg lucide="FileText" class="h-5 w-5 text-red-600"></svg>
                    <div class="flex-1 overflow-hidden">
                      <p class="truncate text-sm font-medium">
                        {{ file.filename }}
                      </p>
                      <p class="text-xs text-muted-foreground">
                        {{ formatFileSize(file.size) }}
                      </p>
                    </div>
                  </li>
                }
              </ul>
            </div>
          </div>
        }

        <p class="text-center text-sm text-muted-foreground">
          After uploading, let me know in the chat and I will review the documents.
        </p>
      </div>
    </div>
  `,
})
export class UploadPage {
  private readonly documentsRepository = inject(DOCUMENTS_REPOSITORY);
  protected readonly files = signal<UploadedFile[]>([]);
  protected readonly uploading = signal(false);
  protected readonly dragOver = signal(false);
  protected readonly error = signal<string | null>(null);

  /** The prototype's template-literal className, whitespace included. */
  protected dropClass(): string {
    return `
                relative flex min-h-48 cursor-pointer flex-col items-center justify-center
                rounded-lg border-2 border-dashed p-8 transition-colors
                ${this.dragOver() ? 'border-primary bg-primary/5' : 'border-muted-foreground/25 hover:border-primary/50'}
              `;
  }

  protected async handleFiles(fileList: FileList): Promise<void> {
    this.uploading.set(true);
    this.error.set(null);

    const pdfFiles = Array.from(fileList).filter((file) => file.type === 'application/pdf');

    if (pdfFiles.length === 0) {
      this.error.set('Please select PDF files only');
      this.uploading.set(false);
      return;
    }

    try {
      const uploaded: UploadedFile[] = [];
      for (const file of pdfFiles) {
        const result = await uploadFile(file, this.documentsRepository);
        uploaded.push(result);
      }
      this.files.update((prev) => [...prev, ...uploaded]);
    } catch (err) {
      this.error.set(err instanceof Error ? err.message : 'Upload failed');
    } finally {
      this.uploading.set(false);
    }
  }

  protected onChange(e: Event): void {
    const files = (e.target as HTMLInputElement).files;
    if (files) void this.handleFiles(files);
  }

  protected handleDrop(e: DragEvent): void {
    e.preventDefault();
    this.dragOver.set(false);
    if (e.dataTransfer?.files) {
      void this.handleFiles(e.dataTransfer.files);
    }
  }

  protected handleDragOver(e: DragEvent): void {
    e.preventDefault();
    this.dragOver.set(true);
  }

  protected handleDragLeave(e: DragEvent): void {
    e.preventDefault();
    this.dragOver.set(false);
  }

  protected formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }
}
