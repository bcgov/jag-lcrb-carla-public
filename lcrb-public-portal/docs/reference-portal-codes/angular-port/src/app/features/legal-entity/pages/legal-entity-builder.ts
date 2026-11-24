/** Licensee-only tool for mapping the ownership structure and submitting it to LCRB; the build is tied to the business account and shared across its applications. */
import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { AddEntityDialog } from '../components/add-entity-dialog';
import { LECanvas } from '../components/le-canvas';
import { NodeInspector } from '../components/node-inspector';
import { RelationshipDialog } from '../components/relationship-dialog';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Separator } from '@shared/ui/primitives/separator';
import { injectBusinessProfile } from '@core/session/business-profile';
import type { LEBuildStatus, LENode, LERelationshipType } from '@domain/models/legal-entity';
import { actorName } from '@domain/roles';
import {
  addEdge,
  addNode,
  type DirectoryBusinessInput,
  injectLegalEntityBuild,
  injectTiedHouseOverlay,
  lcrbReview,
  lcrbSetLock,
  linkDirectoryBusiness,
  linkDirectoryIndividual,
  type NewNodeInput,
  reopenDraft,
  submitToLCRB,
  updateNodePosition,
} from '../state/legal-entity-store';
import { path } from '@/app/route-registry';

const STATUS_META: Record<LEBuildStatus, { label: string; className: string }> = {
  draft: { label: 'Draft', className: 'bg-slate-500/15 text-slate-700' },
  submitted: { label: 'Submitted to LCRB', className: 'bg-[#013366]/10 text-[#013366]' },
  'under-review': { label: 'Under LCRB review', className: 'bg-amber-500/15 text-amber-700' },
  'changes-requested': { label: 'Changes requested', className: 'bg-[#D8292F]/10 text-[#D8292F]' },
  approved: { label: 'Approved by LCRB', className: 'bg-emerald-500/15 text-emerald-700' },
};

function formatDateTime(iso?: string) {
  if (!iso) return '—';
  return new Date(iso).toLocaleString('en-CA', {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  });
}

@Component({
  selector: 'div[leRow]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex items-center justify-between gap-2' },
  template: `
    <span class="text-muted-foreground">{{ label() }}</span>
    <span class="text-right font-medium text-foreground"><ng-content /></span>
  `,
})
export class LeRow {
  readonly label = input.required<string>();
}

@Component({
  selector: 'legal-entity-builder',
  imports: [
    RouterLink,
    ...PAGE_HERO,
    ...CARD,
    Badge,
    Button,
    LucideIcon,
    Separator,
    LECanvas,
    NodeInspector,
    AddEntityDialog,
    RelationshipDialog,
    LeRow,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (session() && session()!.role !== 'licensee') {
      <div class="mx-auto max-w-2xl px-4 py-16">
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle class="flex items-center gap-2">
              <svg lucide="Lock" class="h-5 w-5"></svg> Licensee tool
            </div>
          </div>
          <div uiCardContent class="space-y-4 text-sm text-muted-foreground">
            <p>The Legal Entity Builder is available to licensee (business) accounts. Your current role does not have access to this tool.</p>
            <a uiButton variant="outline" [routerLink]="path('account.login')">Switch account</a>
          </div>
        </div>
      </div>
    } @else {
      <page-hero
        [breadcrumbs]="breadcrumbs"
        title="Legal Entity Builder"
        [description]="
          'Map the companies, trusts, partnerships and people connected to your business. This structure is linked to your account (BN ' +
          profile().bceidBusinessNumber +
          ') and shared with every application and request, so the LCRB doesn\\'t have to rebuild it each time.'
        "
      >
        <span uiBadge heroBadge [class]="'shrink-0 ' + status().className + ' hover:' + status().className">{{ status().label }}</span>
        <ng-container heroMeta>{{ profile().legalName }}</ng-container>
      </page-hero>

      <div class="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
        <!-- Lock / review banners -->
        @if (readOnly()) {
          <div class="mb-4 flex items-start gap-2 rounded-lg border border-amber-300 bg-amber-50 p-3 text-sm text-amber-900">
            <svg lucide="Lock" class="mt-0.5 h-4 w-4 shrink-0"></svg>
            <p>
              This build is <strong>locked by the LCRB</strong> and is being managed in Dynamics. It is read-only in the portal. You'll be able to suggest changes again once it is unlocked.
            </p>
          </div>
        }
        @if (build().status === 'changes-requested' && !readOnly()) {
          <div class="mb-4 flex items-start gap-2 rounded-lg border border-[#D8292F]/30 bg-[#D8292F]/5 p-3 text-sm text-[#D8292F]">
            <svg lucide="AlertTriangle" class="mt-0.5 h-4 w-4 shrink-0"></svg>
            <p>The LCRB has requested changes. Review their notes below, update your structure, and resubmit.</p>
          </div>
        }

        <!-- Toolbar -->
        <div class="mb-3 flex flex-wrap items-center gap-2 rounded-lg border bg-card p-2">
          <button uiButton size="sm" (click)="addOpen.set(true)" [disabled]="readOnly()" class="gap-1.5 bg-[#013366] hover:bg-[#013366]/90">
            <svg lucide="Plus" class="h-4 w-4"></svg> Add entity
          </button>
          <button uiButton size="sm" variant="outline" (click)="openRelationship()" [disabled]="readOnly()" class="gap-1.5">
            <svg lucide="Link2" class="h-4 w-4"></svg> Add relationship
          </button>
          <div class="hidden items-center gap-1.5 px-2 text-xs text-muted-foreground sm:flex">
            <svg lucide="Users" class="h-3.5 w-3.5"></svg> {{ build().nodes.length }} entities · {{ build().edges.length }} relationships
            @if (overlay().nodes.length > 0) {
              <span class="flex items-center gap-1 text-[#0F766E]">
                <svg lucide="Link2" class="h-3.5 w-3.5"></svg> {{ overlay().nodes.length }} from Tied House
              </span>
            }
          </div>
          <div class="ml-auto flex items-center gap-2">
            <!-- Reporting a change (e.g. a new director) is separate from mapping the structure, so surface that channel here. -->
            <a uiButton size="sm" variant="outline" class="gap-1.5" [routerLink]="path('applications.flow', { slug: 'pcl-change' })">
              <svg lucide="FileText" class="h-4 w-4"></svg> Report a change
            </a>
            @if (build().status === 'draft' || build().status === 'changes-requested') {
              <button uiButton size="sm" (click)="handleSubmit()" [disabled]="readOnly()" class="gap-1.5 bg-emerald-600 hover:bg-emerald-700">
                <svg lucide="Send" class="h-4 w-4"></svg> Submit to LCRB
              </button>
            } @else {
              @if (!readOnly()) {
                <button uiButton size="sm" variant="outline" (click)="handleReopen()" class="gap-1.5">
                  <svg lucide="Undo2" class="h-4 w-4"></svg> Reopen draft
                </button>
              }
            }
          </div>
        </div>

        <!-- Canvas + inspector -->
        <div class="relative flex h-[70vh] min-h-[560px] overflow-hidden rounded-lg border bg-card">
          <div class="min-w-0 flex-1">
            <le-canvas
              [build]="displayBuild()"
              [readOnly]="readOnly()"
              [selectedId]="selectedId()"
              (selectNode)="selectedId.set($event)"
              (connectNodes)="handleConnectNodes($event.source, $event.target)"
              (positionChange)="onPositionChange($event.nodeId, $event.position)"
            />
          </div>
          @if (selectedNode(); as node) {
            <div class="absolute inset-y-0 right-0 z-10 w-full border-l bg-card shadow-xl sm:w-[340px] lg:static lg:z-auto lg:shadow-none">
              <node-inspector
                [build]="displayBuild()"
                [node]="node"
                [readOnly]="readOnly()"
                [actor]="actor()"
                (closed)="selectedId.set(null)"
                (addRelationship)="openRelationshipFor($event)"
              />
            </div>
          }
        </div>
        <p class="mt-2 flex items-center gap-1.5 text-xs text-muted-foreground">
          <svg lucide="Info" class="h-3.5 w-3.5"></svg>
          Tip: click a card to view or edit it. Drag from a card's edge to another card to propose a relationship. Facts marked with a lock come from LCRB records and can't be changed here.
        </p>

        <!-- Review + Dynamics + suggestions -->
        <div class="mt-6 grid gap-4 lg:grid-cols-3">
          <!-- Dynamics / status -->
          <div uiCard>
            <div uiCardHeader class="pb-3">
              <div uiCardTitle class="flex items-center gap-2 text-base">
                <svg lucide="RefreshCw" class="h-4 w-4"></svg> LCRB / Dynamics
              </div>
            </div>
            <div uiCardContent class="space-y-2 text-sm">
              <div leRow label="Status">
                <span uiBadge [class]="status().className + ' hover:' + status().className">{{ status().label }}</span>
              </div>
              <div leRow label="Dynamics case">{{ build().dynamicsCaseId ?? 'Not yet synced' }}</div>
              <div leRow label="Last synced">{{ formatDateTime(build().lastSyncedAt) }}</div>
              <div leRow label="Version">v{{ build().version }}</div>
              <div leRow label="Last updated">{{ formatDateTime(build().updatedAt) }}</div>
            </div>
          </div>

          <!-- Review notes -->
          <div uiCard>
            <div uiCardHeader class="pb-3">
              <div uiCardTitle class="flex items-center gap-2 text-base">
                <svg lucide="ShieldCheck" class="h-4 w-4"></svg> LCRB review notes
              </div>
            </div>
            <div uiCardContent class="text-sm">
              @if (build().reviewNotes.length === 0) {
                <p class="text-muted-foreground">No review notes yet.</p>
              } @else {
                <ul class="space-y-3">
                  @for (r of build().reviewNotes.slice(0, 5); track r.id) {
                    <li class="border-l-2 border-[#38598A] pl-3">
                      <div class="flex items-center gap-2">
                        @if (r.decision === 'approved') {
                          <svg lucide="CheckCircle2" class="h-3.5 w-3.5 text-emerald-600"></svg>
                        }
                        @if (r.decision === 'changes-requested') {
                          <svg lucide="AlertTriangle" class="h-3.5 w-3.5 text-[#D8292F]"></svg>
                        }
                        <span class="font-medium text-foreground">{{ r.actor }}</span>
                        <span class="text-xs text-muted-foreground">{{ formatDateTime(r.at) }}</span>
                      </div>
                      <p class="mt-0.5 text-muted-foreground">{{ r.message }}</p>
                    </li>
                  }
                </ul>
              }
            </div>
          </div>

          <!-- Suggestions log -->
          <div uiCard>
            <div uiCardHeader class="pb-3">
              <div uiCardTitle class="flex items-center gap-2 text-base">
                <svg lucide="History" class="h-4 w-4"></svg> Your recent changes
                @if (build().suggestions.length > 0) {
                  <span uiBadge variant="secondary">{{ build().suggestions.length }}</span>
                }
              </div>
            </div>
            <div uiCardContent class="text-sm">
              @if (build().suggestions.length === 0) {
                <p class="text-muted-foreground">Changes you make are tracked here and flagged for LCRB confirmation.</p>
              } @else {
                <ul class="space-y-2">
                  @for (s of build().suggestions.slice(0, 6); track s.id) {
                    <li class="flex items-start gap-2">
                      <span class="mt-1.5 h-1.5 w-1.5 shrink-0 rounded-full bg-amber-500"></span>
                      <div>
                        <p class="text-foreground">{{ s.summary }}</p>
                        <p class="text-xs text-muted-foreground">{{ formatDateTime(s.at) }}</p>
                      </div>
                    </li>
                  }
                </ul>
              }
            </div>
          </div>
        </div>

        <!-- LCRB (Dynamics) demo controls, the staff side -->
        <div uiCard class="mt-6 border-dashed">
          <div uiCardHeader class="pb-3">
            <div uiCardTitle class="flex items-center gap-2 text-base text-muted-foreground">
              <svg lucide="ShieldCheck" class="h-4 w-4"></svg> Demo: LCRB staff actions (Dynamics side)
            </div>
          </div>
          <div uiCardContent class="space-y-3">
            <p class="text-sm text-muted-foreground">
              In production these actions happen in MS Dynamics by LCRB case managers. They're surfaced here so you can see how staff decisions flow back into the portal.
            </p>
            <div uiSeparator></div>
            <div class="flex flex-wrap gap-2">
              @if (!build().lockedByLCRB) {
                <button uiButton size="sm" variant="outline" class="gap-1.5" (click)="lockBuild()">
                  <svg lucide="Lock" class="h-4 w-4"></svg> Lock &amp; take over
                </button>
              } @else {
                <button uiButton size="sm" variant="outline" class="gap-1.5" (click)="unlockBuild()">
                  <svg lucide="Unlock" class="h-4 w-4"></svg> Unlock
                </button>
              }
              <button uiButton size="sm" variant="outline" class="gap-1.5 text-emerald-700" (click)="approveBuild()">
                <svg lucide="CheckCircle2" class="h-4 w-4"></svg> Approve
              </button>
              <button uiButton size="sm" variant="outline" class="gap-1.5 text-[#D8292F]" (click)="requestChanges()">
                <svg lucide="AlertTriangle" class="h-4 w-4"></svg> Request changes
              </button>
            </div>
          </div>
        </div>

        <!-- Dialogs -->
        <add-entity-dialog
          [open]="addOpen()"
          (openChange)="addOpen.set($event)"
          (create)="handleAddEntity($event)"
          [existingBusinessNumbers]="existingBusinessNumbers()"
          [existingIndividualNames]="existingIndividualNames()"
          (linkBusiness)="handleLinkBusiness($event)"
          (linkIndividual)="handleLinkIndividual($event)"
        />
        <relationship-dialog
          [open]="relOpen()"
          (openChange)="relOpen.set($event)"
          [build]="build()"
          [defaultSource]="relDefaults().source"
          [defaultTarget]="relDefaults().target"
          (create)="handleCreateRelationship($event)"
        />
      </div>
    }
  `,
})
export class LegalEntityBuilder {
  protected readonly path = path;
  private readonly sessionProvider = injectSession();
  protected readonly session = this.sessionProvider.session;
  protected readonly profile = injectBusinessProfile();
  protected readonly actor = computed(() => {
    const session = this.session();
    return session ? actorName(session) : 'Licensee';
  });
  protected readonly build = injectLegalEntityBuild(
    () => this.profile().legalName,
    () => this.profile().bceidBusinessNumber,
  );

  protected readonly selectedId = signal<string | null>(null);
  protected readonly addOpen = signal(false);
  protected readonly relOpen = signal(false);
  protected readonly relDefaults = signal<{ source?: string; target?: string }>({});

  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Business Structure' }, { label: 'Legal Entity Builder' }];
  protected readonly formatDateTime = formatDateTime;

  protected readonly readOnly = computed(() => this.build().lockedByLCRB);

  // Live read-only mirror of the Tied House register, shown as external cards and edges; never persisted into the build or editable.
  private readonly applicantNodeId = computed(() => this.build().nodes.find((n) => n.type === 'applicant')?.id ?? 'node_applicant');
  protected readonly overlay = injectTiedHouseOverlay(() => this.build().businessNumber, this.applicantNodeId);

  // Merge the persisted build with the overlay for display only.
  protected readonly displayBuild = computed(() => {
    const build = this.build();
    const overlay = this.overlay();
    return overlay.nodes.length === 0 ? build : { ...build, nodes: [...build.nodes, ...overlay.nodes], edges: [...build.edges, ...overlay.edges] };
  });

  protected readonly selectedNode = computed<LENode | null>(() => this.displayBuild().nodes.find((n) => n.id === this.selectedId()) ?? null);
  protected readonly status = computed(() => STATUS_META[this.build().status]);

  protected readonly existingBusinessNumbers = computed(() =>
    this.build()
      .nodes.flatMap((n) => [n.businessNumber, n.incorporationNumber])
      .filter((v): v is string => Boolean(v)),
  );
  protected readonly existingIndividualNames = computed(() =>
    this.build()
      .nodes.filter((n) => n.type === 'individual')
      .map((n) => n.legalName),
  );


  protected handleAddEntity(input: NewNodeInput): void {
    const node = addNode(this.build().id, this.actor(), input);
    if (node) {
      this.selectedId.set(node.id);
      toast.success('Entity added', { description: `"${node.legalName}" was added as a suggestion.` });
    }
  }

  protected handleLinkBusiness(input: DirectoryBusinessInput): void {
    const before = this.build();
    const node = linkDirectoryBusiness(before.id, this.actor(), input);
    if (node) {
      this.selectedId.set(node.id);
      if (this.build().nodes.length === before.nodes.length && before.nodes.some((n) => n.id === node.id)) {
        toast.info('Already on the canvas', { description: `"${node.legalName}" is already in this build.` });
      } else {
        toast.success('Business added', { description: `"${node.legalName}" was added from records with its licences.` });
      }
    }
  }

  protected handleLinkIndividual(name: string): void {
    const before = this.build();
    const node = linkDirectoryIndividual(before.id, this.actor(), name);
    if (node) {
      this.selectedId.set(node.id);
      if (this.build().nodes.length === before.nodes.length && before.nodes.some((n) => n.id === node.id)) {
        toast.info('Already on the canvas', { description: `"${name}" is already in this build.` });
      } else {
        toast.success('Person added', { description: `"${name}" was added. Draw their connections next.` });
      }
    }
  }

  protected handleCreateRelationship(input: { source: string; target: string; type: LERelationshipType; sharePercent?: number }): void {
    addEdge(this.build().id, this.actor(), input);
    toast.success('Relationship added', { description: 'It will be reviewed by the LCRB.' });
  }

  protected openRelationship(): void {
    this.relDefaults.set({});
    this.relOpen.set(true);
  }

  protected openRelationshipFor(nodeId: string): void {
    this.relDefaults.set({ source: nodeId });
    this.relOpen.set(true);
  }

  protected handleConnectNodes(source: string, target: string): void {
    this.relDefaults.set({ source, target });
    this.relOpen.set(true);
  }

  protected onPositionChange(nodeId: string, position: { x: number; y: number }): void {
    updateNodePosition(this.build().id, nodeId, position);
  }

  protected handleSubmit(): void {
    const result = submitToLCRB(this.build().id, this.actor());
    if (!result) return;
    if (result.dynamicsCaseId && result.status === 'submitted') {
      toast.success('Submitted to LCRB', { description: `Synced to Dynamics as case ${result.dynamicsCaseId}.` });
    } else {
      toast.warning('Submitted — sync pending', {
        description: "We couldn't reach Dynamics. The LCRB will still receive your build; sync will retry.",
      });
    }
  }

  protected handleReopen(): void {
    reopenDraft(this.build().id, this.actor());
    toast.info('Reopened for editing');
  }

  protected lockBuild(): void {
    lcrbSetLock(this.build().id, true);
    toast.info('LCRB locked the build');
  }

  protected unlockBuild(): void {
    lcrbSetLock(this.build().id, false);
    toast.info('LCRB unlocked the build');
  }

  protected approveBuild(): void {
    lcrbReview(this.build().id, 'approved', 'Structure verified against corporate registry. Approved.');
    toast.success('LCRB approved the build');
  }

  protected requestChanges(): void {
    lcrbReview(
      this.build().id,
      'changes-requested',
      "Please confirm the shareholding percentages for the holding companies and add the trustee's ID.",
    );
    toast.warning('LCRB requested changes');
  }
}
