/** Embeddable Tied House declaration section: gating question, existing relationships, declaration boxes, consent, split fee payment and submission. State lives in the shared store. */
import { ChangeDetectionStrategy, Component, computed, inject, input, type OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Separator } from '@shared/ui/primitives/separator';
import { injectBusinessProfile } from '@core/session/business-profile';
import { SOURCE_META } from '@domain/tied-house/tied-house';
import { type ExistingRelationship, type TiedHouseSource, type TiedHouseType } from '@domain/models/tied-house';
import {
  addDeclaration,
  cancelDeclaration,
  canSubmit,
  injectTiedHouseApplication,
  payCannabis,
  payLiquor,
  removeDeclaration,
  removeExisting,
  type SaveDeclarationInput,
  saveDeclaration,
  startChangeFromExisting,
  submitToBranch,
} from '../state/tied-house-store';
import { DeclarationBox } from './declaration-box';
import { ExistingRelationshipsList } from './existing-relationships-list';
import { FeePaymentPanel } from './fee-payment-panel';
import { TIED_HOUSE_REPOSITORY } from '@data-access/repositories/tied-house/tied-house.repository';
import { path } from '@/app/route-registry';

const GATING_QUESTION =
  'Do you or any of your shareholders, directors, or officers have any amount of interest in another B.C. liquor licence, or any association with a third party operator for another liquor licence, or have an immediate family member (spouse, parent, sibling, or child) with any amount of interest in another liquor licence?';

@Component({
  selector: 'tied-house-declaration-section',
  imports: [Button, LucideIcon, Separator, DeclarationBox, ExistingRelationshipsList, FeePaymentPanel],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (!app() || !fees()) {
      <div class="rounded-lg border border-border bg-card p-6 text-sm text-muted-foreground">Loading tied house declaration…</div>
    } @else {
      <div class="space-y-6">
        <!-- Source context -->
        <div class="flex items-start gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-3 text-sm text-[#013366]">
          <svg lucide="Info" class="mt-0.5 h-4 w-4 shrink-0"></svg>
          <p>
            <span class="font-medium">{{ sourceMeta[source()].label }}.</span> {{ sourceMeta[source()].context }}
          </p>
        </div>

        <!-- Gating question -->
        <section class="rounded-lg border border-border bg-card p-5" aria-labelledby="th-gate-heading">
          <h3 id="th-gate-heading" class="flex items-start gap-2 text-sm font-medium text-foreground">
            <svg lucide="ShieldQuestion" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]"></svg>
            <span class="text-pretty leading-relaxed">{{ gatingQuestion }}</span>
          </h3>
          <div class="mt-4 flex flex-wrap gap-2 pl-7">
            @for (v of gateOptions; track v) {
              <button
                type="button"
                (click)="gate.set(v)"
                [disabled]="submitted()"
                [class]="
                  'rounded-md border px-5 py-2 text-sm font-medium capitalize transition-colors disabled:opacity-60 ' +
                  (gate() === v ? 'border-[#013366] bg-[#013366] text-white' : 'border-border text-muted-foreground hover:bg-muted')
                "
              >
                {{ v }}
              </button>
            }
          </div>
          @if (gate() === 'no') {
            <p class="mt-4 pl-7 text-sm text-muted-foreground">No tied house connections to declare. No fee applies.</p>
          }
        </section>

        @if (gate() === 'yes') {
          <!-- Existing relationships -->
          <section aria-labelledby="th-existing-heading">
            <h3 id="th-existing-heading" class="mb-3 flex items-center gap-2 text-sm font-semibold text-foreground">
              <svg lucide="Users" class="h-4 w-4 text-[#013366]"></svg> Existing Tied House Relationships
            </h3>
            <existing-relationships-list
              [relationships]="app()!.existingRelationships"
              [disabled]="submitted()"
              (changed)="handleChangeExisting($event)"
              (removed)="handleRemoveExisting($event)"
            />
          </section>

          <div uiSeparator></div>

          <!-- Declarations -->
          <section aria-labelledby="th-decl-heading">
            <h3 id="th-decl-heading" class="mb-3 flex items-center gap-2 text-sm font-semibold text-foreground">
              <svg lucide="Handshake" class="h-4 w-4 text-[#013366]"></svg> Tied House Declarations
            </h3>

            @if (app()!.declarations.length === 0) {
              <p class="rounded-md border border-dashed border-border bg-muted/30 px-4 py-6 text-center text-sm text-muted-foreground">
                No declarations added yet. Use “Add New Tied House” to declare a connection.
              </p>
            } @else {
              <div class="space-y-3">
                @for (d of app()!.declarations; track d.id) {
                  <div
                    declarationBox
                    [declaration]="d"
                    [disabled]="submitted()"
                    (saved)="handleSave(d.id, $event)"
                    (cancelled)="cancelDecl(d.id)"
                    (removed)="removeDecl(d.id)"
                  ></div>
                }
              </div>
            }

            @if (!submitted()) {
              <div class="mt-3 flex flex-wrap gap-2">
                <button uiButton variant="outline" (click)="handleAdd('individual')" class="gap-1.5">
                  <svg lucide="Plus" class="h-4 w-4"></svg> Add New Tied House
                </button>
              </div>
            }

            <!-- Bilateral consent line -->
            @if (hasSaved()) {
              <p class="mt-4 rounded-md border border-border bg-muted/30 px-4 py-3 text-xs leading-relaxed text-muted-foreground">
                By saving a declaration you consent to a dated tied house connection being recorded on both your file and the associated establishment's file. Connections remain until formally removed.
              </p>
            }
          </section>

          <!-- Payment -->
          @if (showPayment()) {
            <section
              feePaymentPanel
              [fees]="fees()!"
              [liquorReceipt]="app()!.liquorReceipt"
              [cannabisReceipt]="app()!.cannabisReceipt"
              (payLiquor)="handlePayLiquor()"
              (payCannabis)="handlePayCannabis()"
              [disabled]="submitted()"
            ></section>
          }

          <!-- Submit -->
          @if (hasSaved() && !submitted()) {
            <div class="flex flex-col items-end gap-2">
              @if (!submittable()) {
                <p class="text-xs text-muted-foreground">
                  {{ fees()!.grandTotal > 0 ? 'Pay all outstanding fees to enable submission.' : 'Save at least one declaration to submit.' }}
                </p>
              }
              <button uiButton (click)="handleSubmit()" [disabled]="!submittable()" class="gap-1.5 bg-emerald-600 hover:bg-emerald-700">
                <svg lucide="Send" class="h-4 w-4"></svg> Submit to the Branch
              </button>
            </div>
          }
        }
      </div>
    }
  `,
})
export class TiedHouseDeclarationSection implements OnInit {
  private readonly tiedHouseRepository = inject(TIED_HOUSE_REPOSITORY);
  readonly source = input.required<TiedHouseSource>();
  /** When true (e.g. an LE-review outcome requires it), default the gate to Yes. */
  readonly requiredByReview = input<boolean | undefined>(undefined);

  private readonly router = inject(Router);
  private readonly profile = injectBusinessProfile();
  protected readonly app = injectTiedHouseApplication(
    () => this.source(),
    () => this.profile().legalName,
    () => this.profile().bceidBusinessNumber,
  );

  protected readonly sourceMeta = SOURCE_META;
  protected readonly gatingQuestion = GATING_QUESTION;
  protected readonly gateOptions = ['yes', 'no'] as const;

  // The prototype's useState initialiser reads `requiredByReview` once.
  protected readonly gate = signal<'unset' | 'yes' | 'no'>('unset');

  protected readonly fees = computed(() => {
    const app = this.app();
    return app ? this.tiedHouseRepository.computeFees(app.declarations, app.jobNumber) : null;
  });
  protected readonly hasSaved = computed(() => this.app()?.declarations.some((d) => d.status === 'saved') ?? false);
  protected readonly showPayment = computed(() => {
    const fees = this.fees();
    return Boolean(fees && (fees.liquor.total > 0 || fees.cannabis.total > 0) && this.hasSaved());
  });
  protected readonly submitted = computed(() => this.app()?.status === 'submitted');
  protected readonly submittable = computed(() => {
    const app = this.app();
    return app ? canSubmit(app) : false;
  });

  ngOnInit(): void {
    if (this.requiredByReview()) this.gate.set('yes');
  }

  protected handleAdd(type: TiedHouseType): void {
    addDeclaration(this.app()!.id, type);
  }

  protected handleSave(declarationId: string, input: SaveDeclarationInput): void {
    saveDeclaration(this.app()!.id, declarationId, input);
    toast.success('Declaration saved', {
      description: 'A dated connection has been recorded on both files. Review the fees below.',
    });
  }

  protected cancelDecl(declarationId: string): void {
    cancelDeclaration(this.app()!.id, declarationId);
  }

  protected removeDecl(declarationId: string): void {
    removeDeclaration(this.app()!.id, declarationId);
  }

  protected handleChangeExisting(existing: ExistingRelationship): void {
    startChangeFromExisting(this.app()!.id, existing);
    toast.info('Editing existing relationship', { description: 'Update the details and save.' });
  }

  protected handleRemoveExisting(existing: ExistingRelationship): void {
    removeExisting(this.app()!.id, existing);
    toast.info('Relationship marked for removal');
  }

  protected handlePayLiquor(): void {
    payLiquor(this.app()!.id);
    toast.success('Liquor fees paid', { description: 'Payment received from the government payment provider.' });
  }

  protected handlePayCannabis(): void {
    payCannabis(this.app()!.id);
    toast.success('Cannabis fees paid', { description: 'Payment received from the government payment provider.' });
  }

  protected handleSubmit(): void {
    const result = submitToBranch(this.app()!.id);
    if (result) {
      toast.success('Submitted to the Branch');
      void this.router.navigateByUrl(path('legal-entity.tied-house-confirmation', { id: encodeURIComponent(this.app()!.id) }));
    }
  }
}
