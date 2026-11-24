/** Public "File a Complaint" form for a licence. */
import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { Button } from '../primitives/button';
import { DIALOG } from '../primitives/dialog';
import { Input } from '../primitives/input';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { SELECT } from '../primitives/select';
import { Textarea } from '../primitives/textarea';
import type { License } from '@domain/models/map';

const EMPTY_FORM = { name: '', email: '', phone: '', complaintType: '', description: '' };

@Component({
  selector: 'complaint-dialog',
  imports: [...DIALOG, ...SELECT, Button, Input, Label, LucideIcon, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <ui-dialog-content *uiDialogPortal [class]="submitted() ? 'sm:max-w-md' : 'sm:max-w-lg'">
        @if (submitted()) {
          <div class="flex flex-col items-center justify-center py-8 text-center">
            <svg lucide="CheckCircle" class="w-16 h-16 text-green-500 mb-4"></svg>
            <h3 class="text-xl font-semibold mb-2">Complaint Submitted</h3>
            <p class="text-muted-foreground">Thank you for your report. A reference number will be sent to your email.</p>
          </div>
        } @else {
          <div uiDialogHeader>
            <h2 uiDialogTitle>File a Complaint</h2>
            <p uiDialogDescription>Submit a complaint about {{ license().businessName }}</p>
          </div>

          <form (submit)="handleSubmit($event)" class="space-y-4">
            <div class="grid gap-4 sm:grid-cols-2">
              <div class="space-y-2">
                <label uiLabel for="name">Your Name</label>
                <input uiInput id="name" [value]="formData().name" (input)="patch('name', $any($event.target).value)" required />
              </div>
              <div class="space-y-2">
                <label uiLabel for="email">Email</label>
                <input uiInput id="email" type="email" [value]="formData().email" (input)="patch('email', $any($event.target).value)" required />
              </div>
            </div>

            <div class="grid gap-4 sm:grid-cols-2">
              <div class="space-y-2">
                <label uiLabel for="phone">Phone (optional)</label>
                <input uiInput id="phone" type="tel" [value]="formData().phone" (input)="patch('phone', $any($event.target).value)" />
              </div>
              <div class="space-y-2">
                <label uiLabel for="type">Complaint Type</label>
                <ui-select [value]="formData().complaintType" (valueChange)="patch('complaintType', $event ?? '')">
                  <button uiSelectTrigger id="type">
                    <span uiSelectValue placeholder="Select type"></span>
                  </button>
                  <ng-template uiSelectContent>
                    <ui-select-item value="noise">Noise Disturbance</ui-select-item>
                    <ui-select-item value="intoxication">Intoxicated Patrons</ui-select-item>
                    <ui-select-item value="minors">Service to Minors</ui-select-item>
                    <ui-select-item value="hours">Operating Hours</ui-select-item>
                    <ui-select-item value="safety">Safety Concern</ui-select-item>
                    <ui-select-item value="other">Other</ui-select-item>
                  </ng-template>
                </ui-select>
              </div>
            </div>

            <div class="space-y-2">
              <label uiLabel for="description">Description</label>
              <textarea
                uiTextarea
                id="description"
                placeholder="Please provide details about your complaint..."
                [value]="formData().description"
                (input)="patch('description', $any($event.target).value)"
                rows="4"
                required
              ></textarea>
            </div>

            <div class="flex justify-end gap-3">
              <button uiButton type="button" variant="outline" (click)="openChange.emit(false)">Cancel</button>
              <button uiButton type="submit">Submit Complaint</button>
            </div>
          </form>
        }
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class ComplaintDialog {
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();
  readonly license = input.required<License>();

  protected readonly submitted = signal(false);
  protected readonly formData = signal({ ...EMPTY_FORM });

  protected patch(key: keyof typeof EMPTY_FORM, value: string): void {
    this.formData.update((f) => ({ ...f, [key]: value }));
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }

  protected handleSubmit(e: Event): void {
    e.preventDefault();
    // The prototype's Radix Select (`required`) adds a hidden native select, so the browser blocks the submit while no complaint type is chosen.
    if (!this.formData().complaintType) return;
    // Simulate submission
    this.submitted.set(true);
    setTimeout(() => {
      this.submitted.set(false);
      this.openChange.emit(false);
      this.formData.set({ ...EMPTY_FORM });
    }, 2000);
  }
}
