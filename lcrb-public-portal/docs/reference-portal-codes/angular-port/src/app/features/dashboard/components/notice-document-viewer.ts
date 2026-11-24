/** The prototype's `onOpen(id)` prop is the `opened` output. */
import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { Button } from '@shared/ui/primitives/button';
import { DIALOG } from '@shared/ui/primitives/dialog';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ScrollArea } from '@shared/ui/primitives/scroll-area';
import { formatNoticeDate } from '@domain/notifications/notices';
import { type Notice } from '@domain/models/notifications';
import { generateNoticePdf } from '@shared/util/pdf/notice-pdf';

@Component({
  selector: 'notice-document-viewer',
  imports: [...DIALOG, Button, LucideIcon, ScrollArea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @let d = notice().document;
    <div class="mt-3 flex flex-wrap items-center gap-2">
      <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onOpenChange($event === 'open')">
        <button uiButton uiDialogTrigger variant="outline" size="sm" class="gap-1.5 bg-background">
          <svg lucide="FileText" class="h-4 w-4"></svg>
          Read full notice
        </button>
        <ui-dialog-content *uiDialogPortal class="max-w-2xl p-0 gap-0">
          <div uiDialogHeader class="px-6 pt-6 pb-4 border-b">
            <h2 uiDialogTitle class="text-base">{{ notice().title }}</h2>
            <p uiDialogDescription>Official notice {{ d.documentNumber }} — reference {{ notice().reference }}</p>
          </div>

          <!-- On-screen rendering of the full official letter. -->
          <div uiScrollArea class="max-h-[60vh]">
            <article class="px-6 py-6 text-sm leading-relaxed text-foreground">
              <header class="border-b-2 border-[#013366] pb-4">
                <p class="text-lg font-bold tracking-wide text-[#013366]">BRITISH COLUMBIA</p>
                <p class="font-semibold text-[#013366]">Liquor and Cannabis Regulation Branch</p>
                <p class="text-xs text-muted-foreground">Ministry of Public Safety and Solicitor General</p>
              </header>

              <div class="mt-5 flex items-start justify-between gap-4">
                <span>{{ formatNoticeDate(notice().date) }}</span>
                <span class="text-muted-foreground">File: {{ d.documentNumber }}</span>
              </div>

              <div class="mt-5">
                <p>{{ d.recipientName }}</p>
                <p>{{ d.recipientBusiness }}</p>
                @for (line of d.recipientAddress; track line) {
                  <p>{{ line }}</p>
                }
              </div>

              <p class="mt-5 font-semibold">RE: {{ d.subject }}</p>

              <p class="mt-4">{{ d.salutation }}</p>

              <div class="mt-4 space-y-4">
                @for (para of d.body; track $index) {
                  <p class="text-pretty">{{ para }}</p>
                }
              </div>

              <div class="mt-6">
                <p>{{ d.closing }}</p>
                <p class="mt-8 font-semibold">{{ d.signatoryName }}</p>
                <p class="text-muted-foreground">{{ d.signatoryTitle }}</p>
                <p class="text-muted-foreground">{{ d.signatoryBranch }}</p>
              </div>

              @if (d.enclosures && d.enclosures.length > 0) {
                <div class="mt-6">
                  <p class="font-semibold">Enclosures</p>
                  <ul class="mt-1 list-disc pl-5 text-muted-foreground">
                    @for (enc of d.enclosures; track enc) {
                      <li>{{ enc }}</li>
                    }
                  </ul>
                </div>
              }

              @if (d.references && d.references.length > 0) {
                <div class="mt-6 border-t pt-3 text-xs italic text-muted-foreground">
                  <p>Authority:</p>
                  @for (ref of d.references; track ref) {
                    <p>{{ ref }}</p>
                  }
                </div>
              }
            </article>
          </div>

          <div class="flex items-center justify-end gap-2 border-t px-6 py-4">
            <button
              uiButton
              (click)="handleDownload()"
              [disabled]="downloading()"
              class="gap-1.5 bg-[#013366] hover:bg-[#38598A]"
            >
              @if (downloading()) {
                <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
              } @else {
                <svg lucide="Download" class="h-4 w-4"></svg>
              }
              Download PDF
            </button>
          </div>
        </ui-dialog-content>
      </ui-dialog>

      <button uiButton variant="ghost" size="sm" (click)="handleDownload()" [disabled]="downloading()" class="gap-1.5">
        @if (downloading()) {
          <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
        } @else {
          <svg lucide="Download" class="h-4 w-4"></svg>
        }
        Download PDF
      </button>
    </div>
  `,
})
export class NoticeDocumentViewer {
  readonly notice = input.required<Notice>();
  /** Emitted when the licensee opens the full document (used to mark as read). */
  readonly opened = output<number>();

  protected readonly open = signal(false);
  protected readonly downloading = signal(false);
  protected readonly formatNoticeDate = formatNoticeDate;

  protected onOpenChange(next: boolean): void {
    this.open.set(next);
    if (next) this.opened.emit(this.notice().id);
  }

  protected async handleDownload(): Promise<void> {
    try {
      this.downloading.set(true);
      await generateNoticePdf(this.notice());
    } catch (err) {
      console.error('[v0] notice pdf error:', err);
    } finally {
      this.downloading.set(false);
    }
  }
}
