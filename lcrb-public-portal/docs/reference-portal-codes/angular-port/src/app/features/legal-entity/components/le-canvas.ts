/** Org-chart canvas on ngx-vflow; the Controls, Panel and MiniMap overlays are hand-built to match React Flow's DOM and styles. Read-only when locked by LCRB. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  output,
  signal,
  untracked,
  viewChild,
} from '@angular/core';
import {
  type ComponentNode,
  type Connection,
  type ConnectionSettings,
  createEdge,
  createNode,
  type Edge,
  type Node,
  type NodeDragEndEvent,
  Vflow,
  VflowComponent,
} from 'ngx-vflow';
import { type AnyComponentNodeEvent } from 'ngx-vflow';
import { ENTITY_META, RELATIONSHIP_META } from '@domain/legal-entity/legal-entity';
import { type LegalEntityBuild } from '@domain/models/legal-entity';
import { EntityNode, type LENodeData, PersonNode, rfNodeKind } from './entity-node';

type CanvasNode = Required<ComponentNode<LENodeData>>;

interface EdgeData {
  color: string;
  dash: string | null;
  animated: boolean;
}

interface EdgeLabelData {
  text: string;
  color: string;
}

const MIN_ZOOM = 0.2;
const MAX_ZOOM = 1.5;
const MINIMAP_WIDTH = 200;
const MINIMAP_HEIGHT = 150;
const MINIMAP_OFFSET_SCALE = 5;

function edgeLabel(sharePercent: number | undefined, type: string, label: string | undefined) {
  if (label) return label;
  if ((type === 'shareholding' || type === 'partnership') && typeof sharePercent === 'number') {
    return `${sharePercent}% Shares`;
  }
  return RELATIONSHIP_META[type as keyof typeof RELATIONSHIP_META]?.label ?? type;
}

const clamp = (v: number, min: number, max: number) => Math.min(Math.max(v, min), max);

@Component({
  selector: 'le-canvas',
  imports: [Vflow],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  styles: `
    .react-flow {
      --xy-controls-button-background-color-default: #fefefe;
      --xy-controls-button-background-color-hover-default: #f4f4f4;
      --xy-controls-button-border-color-default: #eee;
      --xy-controls-box-shadow-default: 0 0 2px 1px rgba(0, 0, 0, 0.08);
      --xy-minimap-background-color-default: #fff;
      --xy-minimap-mask-background-color-default: rgba(240, 240, 240, 0.6);
      --xy-minimap-mask-stroke-color-default: transparent;
      --xy-minimap-mask-stroke-width-default: 1;
      --xy-minimap-node-stroke-color-default: transparent;
      --xy-minimap-node-stroke-width-default: 2;
      background-color: transparent;
    }
    .react-flow__panel {
      position: absolute;
      z-index: 5;
      margin: 15px;
    }
    .react-flow__panel.top {
      top: 0;
    }
    .react-flow__panel.bottom {
      bottom: 0;
    }
    .react-flow__panel.left {
      left: 0;
    }
    .react-flow__panel.right {
      right: 0;
    }
    .react-flow__controls {
      display: flex;
      flex-direction: column;
      box-shadow: var(--xy-controls-box-shadow-default);
    }
    .react-flow__controls-button {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 26px;
      width: 26px;
      padding: 4px;
      border: none;
      background: var(--xy-controls-button-background-color-default);
      border-bottom: 1px solid var(--xy-controls-button-border-color-default);
      color: inherit;
      cursor: pointer;
      user-select: none;
    }
    .react-flow__controls-button svg {
      width: 100%;
      max-width: 12px;
      max-height: 12px;
      fill: currentColor;
    }
    .react-flow__controls-button:hover {
      background: var(--xy-controls-button-background-color-hover-default);
      color: inherit;
    }
    .react-flow__controls-button:disabled {
      pointer-events: none;
    }
    .react-flow__controls-button:disabled svg {
      fill-opacity: 0.4;
    }
    .react-flow__controls-button:last-child {
      border-bottom: none;
    }
    .react-flow__minimap {
      background: var(--xy-minimap-background-color-default);
    }
    .react-flow__minimap-svg {
      display: block;
    }
    .react-flow__minimap-mask {
      fill: var(--xy-minimap-mask-background-color-default);
      stroke: var(--xy-minimap-mask-stroke-color-default);
      stroke-width: var(--xy-minimap-mask-stroke-width-default);
    }
    .react-flow__minimap-node {
      stroke: var(--xy-minimap-node-stroke-color-default);
      stroke-width: var(--xy-minimap-node-stroke-width-default);
    }
    .react-flow__edge-path {
      fill: none;
    }
    .react-flow__edge-path.animated {
      stroke-dasharray: 5;
      animation: dashdraw 0.5s linear infinite;
    }
    .react-flow__edge-text {
      font-size: 10px;
      pointer-events: none;
      user-select: none;
    }
    @keyframes dashdraw {
      from {
        stroke-dashoffset: 10;
      }
    }
  `,
  template: `
    <div
      class="react-flow bg-muted/30 light"
      style="width: 100%; height: 100%; overflow: hidden; position: relative; z-index: 0"
      data-testid="rf__wrapper"
      (click)="onPaneClick($event)"
    >
      <vflow
        view="auto"
        selectionMode="manual"
        [nodes]="nodes()"
        [edges]="edges()"
        [minZoom]="minZoom"
        [maxZoom]="maxZoom"
        [background]="background"
        [connection]="connection"
        (connect)="handleConnect($event)"
        (nodeDragEnd)="handleDragEnd($event)"
        (componentNodeEvent)="handleNodeEvent($event)"
      >
        <ng-template edge let-ctx>
          <svg:path
            class="react-flow__edge-path"
            [class.animated]="ctx.data().animated"
            [attr.d]="ctx.path()"
            [attr.stroke]="ctx.data().color"
            stroke-width="2"
            [style.stroke-dasharray]="ctx.data().dash"
            [attr.marker-end]="ctx.markerEnd()"
          />
        </ng-template>
        <ng-template edgeLabelHtml let-ctx>
          <div
            class="react-flow__edge-text"
            style="padding: 3px 6px; border-radius: 4px; background-color: rgba(255, 255, 255, 0.9); font-size: 11px; font-weight: 600; white-space: nowrap"
            [style.color]="ctx.label.data.color"
          >
            {{ ctx.label.data.text }}
          </div>
        </ng-template>
      </vflow>

      <!-- Controls (showInteractive={false}) -->
      <div class="react-flow__panel react-flow__controls vertical bottom left" data-testid="rf__controls" aria-label="Control Panel">
        <button
          type="button"
          class="react-flow__controls-button react-flow__controls-zoomin"
          title="Zoom In"
          aria-label="Zoom In"
          [disabled]="maxZoomReached()"
          (click)="zoomBy(1.2)"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32">
            <path d="M32 18.133H18.133V32h-4.266V18.133H0v-4.266h13.867V0h4.266v13.867H32z" />
          </svg>
        </button>
        <button
          type="button"
          class="react-flow__controls-button react-flow__controls-zoomout"
          title="Zoom Out"
          aria-label="Zoom Out"
          [disabled]="minZoomReached()"
          (click)="zoomBy(1 / 1.2)"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 5">
            <path d="M0 0h32v4.2H0z" />
          </svg>
        </button>
        <button
          type="button"
          class="react-flow__controls-button react-flow__controls-fitview"
          title="Fit View"
          aria-label="Fit View"
          (click)="fitView()"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 30">
            <path
              d="M3.692 4.63c0-.53.4-.938.939-.938h5.215V0H4.708C2.13 0 0 2.054 0 4.63v5.216h3.692V4.631zM27.354 0h-5.2v3.692h5.17c.53 0 .984.4.984.939v5.215H32V4.631A4.624 4.624 0 0027.354 0zm.954 24.83c0 .532-.4.94-.939.94h-5.215v3.768h5.215c2.577 0 4.631-2.13 4.631-4.707v-5.139h-3.692v5.139zm-23.677.94c-.531 0-.939-.4-.939-.94v-5.138H0v5.139c0 2.577 2.13 4.707 4.708 4.707h5.138V25.77H4.631z"
            />
          </svg>
        </button>
      </div>

      <!-- MiniMap (pannable, zoomable) -->
      <div class="react-flow__panel react-flow__minimap !bg-card bottom right" data-testid="rf__minimap">
        <svg
          width="200"
          height="150"
          class="react-flow__minimap-svg"
          role="img"
          aria-labelledby="react-flow__minimap-desc-1"
          [attr.viewBox]="minimap().viewBox"
          (mousemove)="onMinimapPan($event)"
          (wheel)="onMinimapZoom($event)"
        >
          <title id="react-flow__minimap-desc-1">Mini Map</title>
          @for (n of minimap().nodes; track n.id) {
            <rect
              class="react-flow__minimap-node"
              [class.selected]="n.selected"
              [attr.x]="n.x"
              [attr.y]="n.y"
              rx="5"
              ry="5"
              [attr.width]="n.width"
              [attr.height]="n.height"
              [style.fill]="n.color"
              shape-rendering="crispEdges"
            />
          }
          <path class="react-flow__minimap-mask" [attr.d]="minimap().mask" fill-rule="evenodd" pointer-events="none" />
        </svg>
      </div>

      <div class="react-flow__panel rounded-md border bg-card/95 p-2 shadow-sm top left">
        <p class="mb-1 text-[10px] font-bold uppercase tracking-wide text-muted-foreground">Entity types</p>
        <div class="grid grid-cols-2 gap-x-3 gap-y-1">
          @for (entry of entityTypes; track entry.key) {
            <div class="flex items-center gap-1.5">
              <span class="h-2.5 w-2.5 rounded-sm" [style.background-color]="entry.meta.color"></span>
              <span class="text-[10px] text-foreground">{{ entry.meta.short }}</span>
            </div>
          }
        </div>
        <p class="mt-2 text-[10px] leading-tight text-muted-foreground">Solid line = confirmed · dashed = suggested</p>
        <p class="mt-1 flex items-center gap-1 text-[10px] leading-tight text-[#0F766E]">
          <span class="inline-block h-0 w-4 border-t-2 border-dotted border-[#0F766E]"></span>
          Tied House association (read-only)
        </p>
      </div>
    </div>
  `,
})
export class LECanvas {
  readonly build = input.required<LegalEntityBuild>();
  readonly readOnly = input.required<boolean>();
  readonly selectedId = input.required<string | null>();
  readonly selectNode = output<string | null>();
  readonly connectNodes = output<{ source: string; target: string }>();
  readonly positionChange = output<{ nodeId: string; position: { x: number; y: number } }>();

  protected readonly minZoom = MIN_ZOOM;
  protected readonly maxZoom = MAX_ZOOM;
  protected readonly background = { type: 'dots', gap: 20, size: 1, color: '#cbd5e1' } as const;
  protected readonly entityTypes = Object.entries(ENTITY_META).map(([key, meta]) => ({ key, meta }));
  protected readonly connection: ConnectionSettings = {
    allowSelfConnections: true,
    // A card that is not connectable (read-only build, or a Tied House mirror) cannot take part.
    validator: (c) => this.isConnectable(c.source) && this.isConnectable(c.target),
  };

  private readonly vflow = viewChild(VflowComponent);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly destroyRef = inject(DestroyRef);

  private readonly nodeMap = new Map<string, CanvasNode>();
  private readonly edgeMap = new Map<string, Edge<EdgeData>>();
  protected readonly nodes = signal<Node[]>([]);
  protected readonly edges = signal<Edge[]>([]);
  private readonly flowSize = signal({ width: 0, height: 0 });

  protected readonly maxZoomReached = computed(() => (this.vflow()?.viewport().zoom ?? 1) >= MAX_ZOOM);
  protected readonly minZoomReached = computed(() => (this.vflow()?.viewport().zoom ?? 1) <= MIN_ZOOM);

  /** React Flow's MiniMap geometry (viewBB, bounding rect, view scale, offset, mask path). */
  protected readonly minimap = computed(() => {
    const vp = this.vflow()?.viewport() ?? { x: 0, y: 0, zoom: 1 };
    const { width: fw, height: fh } = this.flowSize();
    const viewBB = { x: -vp.x / vp.zoom, y: -vp.y / vp.zoom, width: fw / vp.zoom, height: fh / vp.zoom };
    const rects = this.nodes().map((n) => {
      const node = n as CanvasNode;
      const { x, y } = node.point();
      return { node, x, y, width: node.width(), height: node.height() };
    });
    let bounds = viewBB;
    if (rects.length > 0) {
      const x1 = Math.min(viewBB.x, ...rects.map((r) => r.x));
      const y1 = Math.min(viewBB.y, ...rects.map((r) => r.y));
      const x2 = Math.max(viewBB.x + viewBB.width, ...rects.map((r) => r.x + r.width));
      const y2 = Math.max(viewBB.y + viewBB.height, ...rects.map((r) => r.y + r.height));
      bounds = { x: x1, y: y1, width: x2 - x1, height: y2 - y1 };
    }
    const viewScale = Math.max(bounds.width / MINIMAP_WIDTH, bounds.height / MINIMAP_HEIGHT);
    const viewWidth = viewScale * MINIMAP_WIDTH;
    const viewHeight = viewScale * MINIMAP_HEIGHT;
    const offset = MINIMAP_OFFSET_SCALE * viewScale;
    const x = bounds.x - (viewWidth - bounds.width) / 2 - offset;
    const y = bounds.y - (viewHeight - bounds.height) / 2 - offset;
    const width = viewWidth + offset * 2;
    const height = viewHeight + offset * 2;
    return {
      viewScale,
      viewBox: `${x} ${y} ${width} ${height}`,
      mask: `M${x - offset},${y - offset}h${width + offset * 2}v${height + offset * 2}h${-width - offset * 2}z M${viewBB.x},${viewBB.y}h${viewBB.width}v${viewBB.height}h${-viewBB.width}z`,
      nodes: rects.map((r) => ({
        id: r.node.id,
        x: r.x,
        y: r.y,
        width: r.width,
        height: r.height,
        selected: r.node.selected(),
        color: ENTITY_META[r.node.data().node.type].color,
      })),
    };
  });

  // The prototype resyncs on [build.updatedAt, build.id, readOnly] only.
  private readonly updatedAt = computed(() => this.build().updatedAt);
  private readonly buildId = computed(() => this.build().id);

  constructor() {
    effect(() => {
      this.updatedAt();
      this.buildId();
      this.readOnly();
      untracked(() => this.sync());
    });

    // Reflect selection highlight from the parent.
    effect(() => {
      const id = this.selectedId();
      for (const n of this.nodes() as CanvasNode[]) n.selected.set(n.id === id);
    });

    // React Flow's `fitView` prop: fit once the flow has laid out.
    effect(() => {
      const flow = this.vflow();
      if (flow?.initialized()) setTimeout(() => flow.fitView({ padding: 0.2 }));
    });

    afterNextRender(() => {
      const el = this.host.nativeElement.firstElementChild as HTMLElement;
      const measure = () => this.flowSize.set({ width: el.clientWidth, height: el.clientHeight });
      measure();
      const observer = new ResizeObserver(measure);
      observer.observe(el);
      this.destroyRef.onDestroy(() => observer.disconnect());
    });
  }

  private isConnectable(id: string): boolean {
    const data = this.nodeMap.get(id)?.data();
    return Boolean(data) && !data!.readOnly;
  }

  /** Maps the build onto stable ngx-vflow node and edge objects. */
  private sync(): void {
    const build = this.build();
    const readOnly = this.readOnly();

    const ids = new Set<string>();
    const nodes = build.nodes.map((n) => {
      ids.add(n.id);
      // External (Tied House) cards are read-only mirrors: never draggable or connectable, whatever the build's lock state.
      const external = Boolean(n.external);
      const data: LENodeData = { node: n, readOnly: readOnly || external };
      const type = rfNodeKind(n) === 'person' ? PersonNode : EntityNode;
      let node = this.nodeMap.get(n.id);
      if (!node || (node.type as unknown) !== type) {
        node = createNode<LENodeData>({
          id: n.id,
          type: type as ComponentNode<LENodeData>['type'],
          point: { ...n.position },
          data,
          draggable: !readOnly && !external,
          selected: false,
          width: type === PersonNode ? 210 : 300,
          height: 100,
        }) as CanvasNode;
        this.nodeMap.set(n.id, node);
      } else {
        const p = node.point();
        if (p.x !== n.position.x || p.y !== n.position.y) node.point.set({ ...n.position });
        node.data.set(data);
        node.draggable.set(!readOnly && !external);
      }
      return node;
    });
    for (const id of [...this.nodeMap.keys()]) if (!ids.has(id)) this.nodeMap.delete(id);
    this.nodes.set(nodes);

    const edgeIds = new Set<string>();
    const edges = build.edges.map((e) => {
      edgeIds.add(e.id);
      // Tied House mirror edges get a distinct teal, dotted styling.
      const external = Boolean(e.external);
      const color = external ? '#0F766E' : (RELATIONSHIP_META[e.type]?.color ?? '#64748B');
      const data: EdgeData = {
        color,
        // Suggested (portal-added) relationships are dashed until LCRB confirms them.
        dash: external ? '2 4' : e.locked ? null : '6 4',
        animated: !e.locked && !external,
      };
      const label: EdgeLabelData = { text: edgeLabel(e.sharePercent, e.type, e.label), color };
      const markers = { end: { type: 'arrow-closed' as const, color, width: 18, height: 18 } };
      const edgeLabels = { center: { type: 'html-template' as const, data: label } };
      let edge = this.edgeMap.get(e.id);
      if (!edge || edge.source !== e.source || edge.target !== e.target) {
        edge = createEdge<EdgeData>({
          id: e.id,
          source: e.source,
          target: e.target,
          type: 'template',
          curve: 'smooth-step',
          data,
          edgeLabels,
          markers,
        }) as Edge<EdgeData>;
        this.edgeMap.set(e.id, edge);
      } else {
        edge.data!.set(data);
        edge.edgeLabels!.set(edgeLabels);
        edge.markers!.set(markers);
      }
      return edge;
    });
    for (const id of [...this.edgeMap.keys()]) if (!edgeIds.has(id)) this.edgeMap.delete(id);
    this.edges.set(edges);
  }

  /** onNodeClick: the cards announce a click through ngx-vflow's component node events. */
  protected handleNodeEvent(event: AnyComponentNodeEvent): void {
    if (event.eventName === 'nodeClick') this.selectNode.emit(event.nodeId);
  }

  /** onPaneClick: a click on the empty canvas (not a card, edge or panel). */
  protected onPaneClick(event: MouseEvent): void {
    if ((event.target as Element).closest('.vflow-node, .react-flow__edge-path, .react-flow__edge-text, .react-flow__panel')) return;
    this.selectNode.emit(null);
  }

  protected handleConnect(c: Connection): void {
    if (this.readOnly() || !c.source || !c.target) return;
    this.connectNodes.emit({ source: c.source, target: c.target });
  }

  protected handleDragEnd(event: NodeDragEndEvent): void {
    const { id } = event.node;
    const position = { ...(event.node as CanvasNode).point() };
    const stored = untracked(this.build).nodes.find((n) => n.id === id)?.position;
    if (stored && stored.x === position.x && stored.y === position.y) return;
    this.positionChange.emit({ nodeId: id, position });
  }

  /** Controls: zoomIn / zoomOut scale the view by 1.2 around the viewport centre. */
  protected zoomBy(factor: number): void {
    const flow = this.vflow();
    if (!flow) return;
    this.scaleTo(flow, flow.viewport().zoom * factor);
  }

  protected fitView(): void {
    this.vflow()?.fitView({ padding: 0.2 });
  }

  private scaleTo(flow: VflowComponent, target: number): void {
    const vp = flow.viewport();
    const zoom = clamp(target, MIN_ZOOM, MAX_ZOOM);
    const { width, height } = this.flowSize();
    const k = zoom / vp.zoom;
    flow.viewportTo({ zoom, x: width / 2 - (width / 2 - vp.x) * k, y: height / 2 - (height / 2 - vp.y) * k });
  }

  /** MiniMap pannable: dragging the minimap moves the main viewport (xyflow's XYMinimap). */
  protected onMinimapPan(event: MouseEvent): void {
    const flow = this.vflow();
    if (!flow || event.buttons !== 1) return;
    const vp = flow.viewport();
    const moveScale = this.minimap().viewScale * Math.max(1, vp.zoom);
    flow.viewportTo({ zoom: vp.zoom, x: vp.x - event.movementX * moveScale, y: vp.y - event.movementY * moveScale });
  }

  /** MiniMap zoomable: the wheel zooms the main viewport (xyflow's XYMinimap). */
  protected onMinimapZoom(event: WheelEvent): void {
    const flow = this.vflow();
    if (!flow) return;
    event.preventDefault();
    const delta = -event.deltaY * (event.deltaMode === 1 ? 0.05 : event.deltaMode ? 1 : 0.002);
    this.scaleTo(flow, flow.viewport().zoom * 2 ** delta);
  }
}
