/** Drag/drop or browse upload that validates type and size, stores the file via /api/upload, registers metadata, and simulates a virus scan that resolves clean 1.5 s later. */
import { ChangeDetectionStrategy, Component, computed, ElementRef, inject, input, signal, viewChild } from '@angular/core';
import { Button } from '../primitives/button';
import { LucideIcon } from '../primitives/lucide-icon';
import { type StoredDocument } from '@data-access/repositories/documents/document-service';
import { formatBytes, MAX_SIZE_BYTES, validateFile } from '@domain/documents/document-service';
import { cn } from '@shared/util/utils';
import { DOCUMENTS_REPOSITORY } from '@data-access/repositories/documents/documents.repository';

@Component({
  selector: 'document-manager',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div [attr.class]="rootClass()">
      @if (!readOnly()) {
        <div (dragover)="onDragOver($event)" (dragleave)="dragging.set(false)" (drop)="onDrop($event)" [attr.class]="dropClass()">
          <svg lucide="Upload" class="h-6 w-6 text-muted-foreground" aria-hidden="true"></svg>
          <p class="text-sm text-foreground">Drag and drop files here, or&ngsp;<button type="button" (click)="openPicker()" class="font-medium text-[#013366] underline underline-offset-2">browse</button></p>
          <p class="text-xs text-muted-foreground">PDF, image, or Word document · up to {{ maxSize }}</p>
          <input
            #fileInput
            type="file"
            multiple
            class="sr-only"
            accept=".pdf,.png,.jpg,.jpeg,.doc,.docx"
            (change)="onChange($event)"
          />
        </div>
      }

      @if (error()) {
        <p role="alert" class="text-sm text-destructive">{{ error() }}</p>
      }
      @if (busy()) {
        <p class="flex items-center gap-2 text-sm text-muted-foreground">
          <svg lucide="Loader2" class="h-4 w-4 animate-spin" aria-hidden="true"></svg>&ngsp;Uploading…
        </p>
      }

      @if (docs().length === 0) {
        <p class="text-sm text-muted-foreground">No documents uploaded yet.</p>
      } @else {
        <ul class="divide-y rounded-lg border">
          @for (doc of docs(); track doc.id) {
            <li class="flex items-center gap-3 px-4 py-3">
              <svg lucide="FileText" class="h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
              <div class="min-w-0 flex-1">
                <p class="truncate text-sm font-medium text-foreground">{{ doc.name }}</p>
                <p class="text-xs text-muted-foreground">{{ size(doc.size) }} · {{ date(doc.uploadedAt) }} · {{ doc.uploadedBy }}</p>
              </div>
              @switch (doc.scanState) {
                @case ('pending') {
                  <span class="flex items-center gap-1 rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-800">
                    <svg lucide="Loader2" class="h-3 w-3 animate-spin" aria-hidden="true"></svg>&ngsp;Scanning
                  </span>
                }
                @case ('clean') {
                  <span class="flex items-center gap-1 rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">
                    <svg lucide="ShieldCheck" class="h-3 w-3" aria-hidden="true"></svg>&ngsp;Clean
                  </span>
                }
                @default {
                  <span class="flex items-center gap-1 rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-800">
                    <svg lucide="ShieldAlert" class="h-3 w-3" aria-hidden="true"></svg>&ngsp;{{ doc.scanState === 'infected' ? 'Threat' : 'Error' }}
                  </span>
                }
              }
              <button
                uiButton
                variant="ghost"
                size="sm"
                class="gap-1.5"
                (click)="onDownload(doc)"
                [disabled]="!doc.blobUrl || doc.scanState !== 'clean'"
              >
                <svg lucide="Download" class="h-4 w-4" aria-hidden="true"></svg>
                <span class="sr-only sm:not-sr-only">Download</span>
              </button>
              @if (!readOnly()) {
                <button
                  uiButton
                  variant="ghost"
                  size="sm"
                  class="text-destructive"
                  (click)="remove(doc)"
                  [attr.aria-label]="'Remove ' + doc.name"
                >
                  <svg lucide="Trash2" class="h-4 w-4" aria-hidden="true"></svg>
                </button>
              }
            </li>
          }
        </ul>
      }
    </div>
  `,
})
export class DocumentManager {
  private readonly documentsRepository = inject(DOCUMENTS_REPOSITORY);
  /** The entity these documents belong to (application/job/permit number). */
  readonly subjectId = input.required<string>();
  /** Acting user's display name, recorded on upload/download. */
  readonly actor = input.required<string>();
  /** Optional logical category applied to uploads from this instance. */
  readonly category = input<string | undefined>(undefined);
  /** When true, hides the uploader and only lists documents (reviewer view). */
  readonly readOnly = input<boolean | undefined>(undefined);
  readonly className = input<string | undefined>(undefined);

  protected readonly docs = this.documentsRepository.documents(() => this.subjectId());
  protected readonly dragging = signal(false);
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);
  private readonly fileInput = viewChild<ElementRef<HTMLInputElement>>('fileInput');
  protected readonly maxSize = formatBytes(MAX_SIZE_BYTES);
  protected readonly size = formatBytes;

  protected readonly rootClass = computed(() => cn('space-y-4', this.className()));
  protected readonly dropClass = computed(() =>
    cn(
      'flex flex-col items-center justify-center gap-2 rounded-lg border-2 border-dashed px-6 py-8 text-center transition-colors',
      this.dragging() ? 'border-[#013366] bg-[#013366]/5' : 'border-border bg-muted/30',
    ),
  );

  protected openPicker(): void {
    this.fileInput()?.nativeElement.click();
  }

  protected date(iso: string): string {
    return new Date(iso).toLocaleDateString();
  }

  protected onDragOver(e: DragEvent): void {
    e.preventDefault();
    this.dragging.set(true);
  }

  protected onDrop(e: DragEvent): void {
    e.preventDefault();
    this.dragging.set(false);
    if (this.readOnly()) return;
    void this.handleFiles(e.dataTransfer?.files ?? null);
  }

  protected onChange(e: Event): void {
    void this.handleFiles((e.target as HTMLInputElement).files);
  }

  protected onDownload(doc: StoredDocument): void {
    this.documentsRepository.recordDownload(doc.id, this.actor());
    if (doc.blobUrl) window.open(doc.blobUrl, '_blank', 'noopener,noreferrer');
  }

  protected remove(doc: StoredDocument): void {
    this.documentsRepository.remove(doc.id, this.actor());
  }

  private async handleFiles(files: FileList | null): Promise<void> {
    if (!files || files.length === 0) return;
    this.error.set(null);

    for (const file of Array.from(files)) {
      const check = validateFile({ type: file.type, size: file.size });
      if (!check.ok) {
        this.error.set(check.reason);
        continue;
      }

      this.busy.set(true);
      try {
        // 1) Store the real bytes in Blob.
        const form = new FormData();
        form.append('file', file);
        form.append('subjectId', this.subjectId());
        const res = await this.documentsRepository.fetchApi('/api/upload', { method: 'POST', body: form });
        if (!res.ok) throw new Error('Upload failed');
        const blob = await res.json();

        // 2) Register metadata (scan starts pending).
        const doc = this.documentsRepository.upload({
          subjectId: this.subjectId(),
          name: file.name,
          contentType: file.type,
          size: file.size,
          uploadedBy: this.actor(),
          category: this.category(),
          blobUrl: blob.url,
        });

        // 3) Simulate the AV scan resolving clean.
        setTimeout(() => this.documentsRepository.completeScan(doc.id, 'clean'), 1500);
      } catch {
        this.error.set('Something went wrong uploading that file. Please try again.');
      } finally {
        this.busy.set(false);
      }
    }
  }
}
