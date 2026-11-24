/** Staff inspection planner slide-over: schedule, AI scheduling, upcoming and completed lists, and the active inspection (timer, checklist, notes, speech-to-text, photos). */
import { ChangeDetectionStrategy, Component, computed, DestroyRef, effect, ElementRef, inject, input, output, signal, untracked, viewChild } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { Progress } from '../primitives/progress';
import { ScrollArea } from '../primitives/scroll-area';
import { SELECT } from '../primitives/select';
import { Separator } from '../primitives/separator';
import { Slider } from '../primitives/slider';
import { Switch } from '../primitives/switch';
import { TABS } from '../primitives/tabs';
import { Textarea } from '../primitives/textarea';
import { MAP_REPOSITORY } from '@data-access/repositories/map/map.repository';
import { calculateRiskScore } from '@domain/map/license-data';
import { type License } from '@domain/models/map';
import { getSpeechRecognitionCtor } from '@shared/util/speech-recognition';

interface ChecklistItem {
  id: string;
  label: string;
  response: 'yes' | 'no' | null;
  notes: string;
  expanded?: boolean;
}

interface InspectionPhoto {
  id: string;
  dataUrl: string;
  timestamp: string;
  selected: boolean;
}

type InspectionType = 'administration' | 'public-safety' | 'minors-underage';

interface PlannedInspection {
  id: string;
  license: License;
  scheduledDate: string;
  completedDate?: string;
  assignedInspector: string;
  priority: 'low' | 'medium' | 'high';
  inspectionType: InspectionType;
  status: 'scheduled' | 'in-progress' | 'paused' | 'completed';
  checklist: ChecklistItem[];
  inspectorNotes: string;
  duration?: number; // seconds
  elapsedSeconds?: number;
  photos?: InspectionPhoto[];
}

const defaultChecklist: ChecklistItem[] = [
  { id: 'license-visible', label: 'Liquor license is visible', expanded: false, notes: '', response: null },
  {
    id: 'social-responsibility',
    label: 'Social Responsibility is visible',
    expanded: false,
    notes: '',
    response: null,
  },
  {
    id: 'floorplan',
    label: 'Floorplan accurately depicts physical layout',
    expanded: false,
    notes: '',
    response: null,
  },
];

const AI_INSPECTION_SUMMARY = 'Check social responsibility posters and check that the owner has a Selling it Right and Serving it Right.';

const DATE_INPUT_CLASS =
  'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2';
const SEARCH_INPUT_CLASS =
  'flex h-10 w-full rounded-md border border-input bg-background pl-10 pr-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2';
const TAB_TRIGGER_CLASS =
  'text-xs py-2 px-1 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground flex items-center justify-center gap-1';

@Component({
  selector: 'inspection-planner',
  imports: [NgTemplateOutlet, Badge, Button, ...CARD, Label, LucideIcon, Progress, ScrollArea, ...SELECT, Separator, Slider, Switch, ...TABS, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  templateUrl: './inspection-planner.html',
})
export class InspectionPlanner {
  private readonly map = inject(MAP_REPOSITORY);
  readonly isOpen = input.required<boolean>();
  readonly closed = output<void>();
  readonly licenses = input.required<License[]>();
  /** The prototype's `onNavigateToLicense` (declared by the prototype, never called). */
  readonly navigateToLicense = output<License>();

  protected readonly regions = this.map.regions();
  protected readonly dateInputClass = DATE_INPUT_CLASS;
  protected readonly searchInputClass = SEARCH_INPUT_CLASS;
  protected readonly tabTriggerClass = TAB_TRIGGER_CLASS;
  protected readonly aiInspectionSummary = AI_INSPECTION_SUMMARY;
  protected readonly inspectorList = this.map.inspectors();

  protected readonly plannedInspections = signal<PlannedInspection[]>([]);
  protected readonly selectedRegion = signal<string>('');
  protected readonly selectedLicense = signal<string>('');
  protected readonly selectedInspector = signal<string>('');
  protected readonly selectedDate = signal<string>('');
  protected readonly selectedPriority = signal<'low' | 'medium' | 'high'>('medium');
  protected readonly selectedInspectionType = signal<InspectionType>('administration');

  protected readonly activeInspection = signal<string | null>(null);
  protected readonly timerSeconds = signal(0);
  private timerInterval: ReturnType<typeof setInterval> | null = null;
  protected readonly isPaused = signal(false);

  protected readonly activeTab = signal<string | undefined>('schedule');

  protected readonly upcomingStartDate = signal<string>('');
  protected readonly upcomingEndDate = signal<string>('');
  protected readonly completedStartDate = signal<string>('');
  protected readonly completedEndDate = signal<string>('');

  protected readonly aiScheduleWeeks = signal<number>(2);
  protected readonly aiPriorityWeight = signal<number[]>([70]);
  protected readonly aiWorkloadBalance = signal<number[]>([50]);
  protected readonly aiIncludeHighRisk = signal(true);
  protected readonly aiIncludeMediumRisk = signal(true);
  protected readonly aiIncludeLowRisk = signal(true);
  protected readonly aiIncludeIntelligenceFlags = signal(true);
  protected readonly aiGeneratedSchedule = signal<PlannedInspection[]>([]);
  protected readonly isGenerating = signal(false);
  protected readonly selectedRegionForAi = signal<string>('all');

  protected readonly inspectionSearch = signal<string>('');

  protected readonly listeningItemId = signal<string | null>(null);
  protected readonly isListeningInspectorNotes = signal(false);
  private recognition: SpeechRecognition | null = null;
  private inspectorNotesRecognition: SpeechRecognition | null = null;

  // State for managing photos in the active inspection: kept inside plannedInspections.
  protected readonly isReviewingPhotos = signal(false);
  protected readonly isTakingPhoto = signal(false);
  // The prototype's videoRef/canvasRef: no element in the template carries these refs (unused code).
  private readonly videoRef = viewChild<ElementRef<HTMLVideoElement>>('videoRef');
  private readonly canvasRef = viewChild<ElementRef<HTMLCanvasElement>>('canvasRef');
  private readonly cameraInput = viewChild<ElementRef<HTMLInputElement>>('cameraInput');
  private readonly galleryInput = viewChild<ElementRef<HTMLInputElement>>('galleryInput');

  protected readonly licensedEstablishments = computed(() => {
    const licensed = this.licenses().filter((l) => l.status === 'licensed');
    const selectedRegion = this.selectedRegion();
    if (!selectedRegion) return licensed;
    return licensed.filter((l) => l.region === selectedRegion);
  });

  protected readonly filteredInspectors = computed(() => {
    const selectedRegion = this.selectedRegion();
    if (!selectedRegion) return this.map.inspectors();
    return this.map.inspectors().filter((i) => i.region === selectedRegion);
  });

  protected readonly activeInspectionData = computed(() => this.plannedInspections().find((i) => i.id === this.activeInspection()));

  protected readonly scheduledInspections = computed(() => this.plannedInspections().filter((i) => i.status === 'scheduled'));

  protected readonly upcomingInspections = computed(() => {
    const upcomingStartDate = this.upcomingStartDate();
    const upcomingEndDate = this.upcomingEndDate();
    return this.plannedInspections()
      .filter((i) => i.status === 'scheduled' || i.status === 'in-progress' || i.status === 'paused')
      .filter((i) => {
        if (upcomingStartDate && i.scheduledDate < upcomingStartDate) return false;
        if (upcomingEndDate && i.scheduledDate > upcomingEndDate) return false;
        return true;
      })
      .sort((a, b) => a.scheduledDate.localeCompare(b.scheduledDate));
  });

  protected readonly completedInspections = computed(() => {
    const completedStartDate = this.completedStartDate();
    const completedEndDate = this.completedEndDate();
    return this.plannedInspections()
      .filter((i) => i.status === 'completed')
      .filter((i) => {
        const dateToCheck = i.completedDate || i.scheduledDate;
        if (completedStartDate && dateToCheck < completedStartDate) return false;
        if (completedEndDate && dateToCheck > completedEndDate) return false;
        return true;
      })
      .sort((a, b) => (b.completedDate || a.scheduledDate).localeCompare(a.completedDate || a.scheduledDate));
  });

  protected readonly filteredUpcoming = computed(() => {
    let inspections = this.upcomingInspections();
    const upcomingStartDate = this.upcomingStartDate();
    const upcomingEndDate = this.upcomingEndDate();
    if (upcomingStartDate) {
      inspections = inspections.filter((i) => i.scheduledDate >= upcomingStartDate);
    }
    if (upcomingEndDate) {
      inspections = inspections.filter((i) => i.scheduledDate <= upcomingEndDate);
    }
    return this.filterInspectionsBySearch(inspections);
  });

  protected readonly filteredCompleted = computed(() => {
    let inspections = this.completedInspections();
    const completedStartDate = this.completedStartDate();
    const completedEndDate = this.completedEndDate();
    if (completedStartDate) {
      inspections = inspections.filter((i) => {
        const dateToCheck = i.completedDate || i.scheduledDate;
        return dateToCheck >= completedStartDate;
      });
    }
    if (completedEndDate) {
      inspections = inspections.filter((i) => {
        const dateToCheck = i.completedDate || i.scheduledDate;
        return dateToCheck <= completedEndDate;
      });
    }
    return this.filterInspectionsBySearch(inspections);
  });

  /** Workload bars of the generated schedule preview (the first five inspectors). */
  protected readonly workloadRows = computed(() => {
    const schedule = this.aiGeneratedSchedule();
    return this.map.inspectors().slice(0, 5).map((inspector) => {
      const workload = schedule.filter((i) => i.assignedInspector === inspector.name).length;
      const maxWorkload = Math.max(...this.map.inspectors().map((i) => schedule.filter((insp) => insp.assignedInspector === i.name).length));
      return { inspector, workload, percent: maxWorkload ? (workload / maxWorkload) * 100 : 0 };
    });
  });

  constructor() {
    effect(() => {
      this.selectedRegion();
      untracked(() => {
        this.selectedLicense.set('');
        this.selectedInspector.set('');
      });
    });
    inject(DestroyRef).onDestroy(() => {
      if (this.timerInterval) clearInterval(this.timerInterval);
    });
  }

  protected addInspection(): void {
    if (!this.selectedLicense() || !this.selectedInspector() || !this.selectedDate()) return;

    const license = this.licenses().find((l) => l.id === this.selectedLicense());
    if (!license) return;

    const newInspection: PlannedInspection = {
      id: `plan-${Date.now()}`,
      license,
      scheduledDate: this.selectedDate(),
      assignedInspector: this.selectedInspector(),
      priority: this.selectedPriority(),
      inspectionType: this.selectedInspectionType(),
      status: 'scheduled',
      checklist: defaultChecklist.map((item) => ({ ...item })),
      inspectorNotes: '',
      photos: [],
    };

    this.plannedInspections.set([...this.plannedInspections(), newInspection]);
    this.selectedRegion.set('');
    this.selectedLicense.set('');
    this.selectedInspector.set('');
    this.selectedDate.set('');
    this.selectedPriority.set('medium');
    this.selectedInspectionType.set('administration');
  }

  protected startInspection(inspectionId: string): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    this.activeInspection.set(inspectionId);

    // Check if this inspection was previously paused and has elapsed time
    const inspection = this.plannedInspections().find((i) => i.id === inspectionId);
    const savedTime = inspection?.elapsedSeconds || 0;

    this.timerSeconds.set(savedTime);
    this.isPaused.set(false);
    this.timerInterval = setInterval(() => {
      this.timerSeconds.update((s) => s + 1);
    }, 1000);
    this.plannedInspections.update((prev) => prev.map((i) => (i.id === inspectionId ? { ...i, status: 'in-progress' as const } : i)));
  }

  protected resumeInspection(): void {
    this.isPaused.set(false);
    this.timerInterval = setInterval(() => {
      this.timerSeconds.update((s) => s + 1);
    }, 1000);
    const activeInspection = this.activeInspection();
    if (activeInspection) {
      this.plannedInspections.update((prev) => prev.map((i) => (i.id === activeInspection ? { ...i, status: 'in-progress' as const } : i)));
    }
  }

  protected pauseTimer(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
    this.isPaused.set(true);
    const activeInspection = this.activeInspection();
    if (activeInspection) {
      const timerSeconds = this.timerSeconds();
      this.plannedInspections.update((prev) =>
        prev.map((i) => (i.id === activeInspection ? { ...i, status: 'paused' as const, elapsedSeconds: timerSeconds } : i)),
      );
    }
  }

  protected goBackToList(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
    const activeInspection = this.activeInspection();
    if (activeInspection) {
      const timerSeconds = this.timerSeconds();
      this.plannedInspections.update((prev) =>
        prev.map((i) => (i.id === activeInspection ? { ...i, elapsedSeconds: timerSeconds, status: 'paused' } : i)),
      );
    }
    this.activeInspection.set(null);
    this.isPaused.set(true); // Set isPaused to true so state is consistent
    // Don't reset timer - it will be restored from elapsedSeconds when resuming
  }

  protected resetTimer(): void {
    this.timerSeconds.set(0);
  }

  protected completeInspection(id: string): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }

    const today = new Date().toISOString().split('T')[0];
    const timerSeconds = this.timerSeconds();
    this.plannedInspections.update((prev) =>
      prev.map((i) =>
        i.id === id
          ? {
              ...i,
              status: 'completed' as const,
              completedDate: today,
              duration: timerSeconds,
            }
          : i,
      ),
    );
    this.activeInspection.set(null);
    this.timerSeconds.set(0);
    this.isPaused.set(false);
  }

  protected toggleChecklistItem(inspectionId: string, itemId: string): void {
    this.plannedInspections.update((prev) =>
      prev.map((i) =>
        i.id === inspectionId
          ? {
              ...i,
              checklist: i.checklist.map((c) => (c.id === itemId ? { ...c, expanded: !c.expanded } : c)),
            }
          : i,
      ),
    );
  }

  protected toggleSpeechToText(inspectionId: string, itemId: string, currentNotes: string): void {
    const SpeechRecognition = getSpeechRecognitionCtor();
    if (!SpeechRecognition) {
      alert('Speech recognition is not supported in your browser. Please use Chrome, Edge, or Safari.');
      return;
    }

    if (this.listeningItemId() === itemId) {
      this.recognition?.stop();
      this.listeningItemId.set(null);
      return;
    }

    if (this.recognition) {
      this.recognition.stop();
    }

    const recognition = new SpeechRecognition();
    recognition.continuous = true;
    recognition.interimResults = true;
    recognition.lang = 'en-CA';

    recognition.onresult = (event: SpeechRecognitionEvent) => {
      let finalTranscript = '';

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
          finalTranscript += transcript;
        }
      }

      if (finalTranscript) {
        const separator = currentNotes.trim() ? ' ' : '';
        this.updateChecklistNotes(inspectionId, itemId, currentNotes + separator + finalTranscript);
      }
    };

    recognition.onerror = (event: SpeechRecognitionErrorEvent) => {
      console.error('Speech recognition error:', event.error);
      if (event.error === 'not-allowed') {
        alert('Microphone access denied. Please allow microphone access to use speech-to-text.');
      }
      this.listeningItemId.set(null);
    };

    recognition.onend = () => {
      this.listeningItemId.set(null);
    };

    this.recognition = recognition;
    recognition.start();
    this.listeningItemId.set(itemId);
  }

  protected toggleInspectorNotesSpeech(inspectionId: string, currentNotes: string): void {
    const SpeechRecognition = getSpeechRecognitionCtor();
    if (!SpeechRecognition) {
      alert('Speech recognition is not supported in your browser. Please use Chrome, Edge, or Safari.');
      return;
    }

    if (this.isListeningInspectorNotes()) {
      this.inspectorNotesRecognition?.stop();
      this.isListeningInspectorNotes.set(false);
      return;
    }

    if (this.inspectorNotesRecognition) {
      this.inspectorNotesRecognition.stop();
    }

    const recognition = new SpeechRecognition();
    recognition.continuous = true;
    recognition.interimResults = true;
    recognition.lang = 'en-CA';

    recognition.onresult = (event: SpeechRecognitionEvent) => {
      let finalTranscript = '';

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
          finalTranscript += transcript;
        }
      }

      if (finalTranscript) {
        const separator = currentNotes.trim() ? ' ' : '';
        this.updateInspectorNotes(inspectionId, currentNotes + separator + finalTranscript);
      }
    };

    recognition.onerror = (event: SpeechRecognitionErrorEvent) => {
      console.error('Speech recognition error:', event.error);
      if (event.error === 'not-allowed') {
        alert('Microphone access denied. Please allow microphone access to use speech-to-text.');
      }
      this.isListeningInspectorNotes.set(false);
    };

    recognition.onend = () => {
      this.isListeningInspectorNotes.set(false);
    };

    this.inspectorNotesRecognition = recognition;
    recognition.start();
    this.isListeningInspectorNotes.set(true);
  }

  protected handlePhotoCapture(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    const activeInspection = this.activeInspection();
    if (!files || !activeInspection) return;

    const newPhotosToAdd: InspectionPhoto[] = [];

    Array.from(files).forEach((file) => {
      const reader = new FileReader();
      reader.onload = (e) => {
        const dataUrl = e.target?.result as string;
        const photo: InspectionPhoto = {
          id: `photo-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
          dataUrl,
          timestamp: new Date().toISOString(),
          selected: false,
        };
        newPhotosToAdd.push(photo);

        if (newPhotosToAdd.length === files.length) {
          this.plannedInspections.update((prev) =>
            prev.map((inspection) =>
              inspection.id === activeInspection ? { ...inspection, photos: [...(inspection.photos || []), ...newPhotosToAdd] } : inspection,
            ),
          );
        }
      };
      reader.readAsDataURL(file);
    });

    // Reset input so same file can be captured again if needed
    input.value = '';
  }

  private updateActivePhotos(update: (photos: InspectionPhoto[] | undefined) => InspectionPhoto[] | undefined): void {
    const activeInspection = this.activeInspection();
    if (!activeInspection) return;

    this.plannedInspections.update((prev) =>
      prev.map((inspection) => (inspection.id === activeInspection ? { ...inspection, photos: update(inspection.photos) } : inspection)),
    );
  }

  protected togglePhotoSelection(photoId: string): void {
    this.updateActivePhotos((photos) => photos?.map((photo) => (photo.id === photoId ? { ...photo, selected: !photo.selected } : photo)));
  }

  protected deletePhoto(photoId: string): void {
    this.updateActivePhotos((photos) => photos?.filter((photo) => photo.id !== photoId));
  }

  protected deleteUnselectedPhotos(): void {
    if (!this.activeInspection()) return;
    this.updateActivePhotos((photos) => photos?.filter((photo) => photo.selected));
    this.isReviewingPhotos.set(false);
  }

  protected selectAllPhotos(): void {
    this.updateActivePhotos((photos) => photos?.map((photo) => ({ ...photo, selected: true })));
  }

  protected deselectAllPhotos(): void {
    this.updateActivePhotos((photos) => photos?.map((photo) => ({ ...photo, selected: false })));
  }

  protected hasUnselectedPhoto(inspection: PlannedInspection): boolean {
    return !!inspection.photos?.some((p) => !p.selected);
  }

  protected selectedPhotoCount(inspection: PlannedInspection): number {
    return inspection.photos?.filter((p) => p.selected).length || 0;
  }

  protected updateChecklistNotes(inspectionId: string, itemId: string, notes: string): void {
    this.plannedInspections.update((prev) =>
      prev.map((i) =>
        i.id === inspectionId
          ? {
              ...i,
              checklist: i.checklist.map((c) => (c.id === itemId ? { ...c, notes } : c)),
            }
          : i,
      ),
    );
  }

  protected updateChecklistResponse(inspectionId: string, itemId: string, response: 'yes' | 'no'): void {
    this.plannedInspections.update((prev) =>
      prev.map((i) =>
        i.id === inspectionId
          ? {
              ...i,
              checklist: i.checklist.map((c) => (c.id === itemId ? { ...c, response, expanded: false } : c)),
            }
          : i,
      ),
    );
  }

  protected updateInspectorNotes(inspectionId: string, notes: string): void {
    this.plannedInspections.update((prev) => prev.map((i) => (i.id === inspectionId ? { ...i, inspectorNotes: notes } : i)));
  }

  protected formatTime(seconds: number): string {
    const hrs = Math.floor(seconds / 3600);
    const mins = Math.floor((seconds % 3600) / 60);
    const secs = seconds % 60;
    return `${hrs.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  protected getInspectionTypeLabel(type: InspectionType): string | undefined {
    switch (type) {
      case 'administration':
        return 'Administration';
      case 'public-safety':
        return 'Public Safety';
      case 'minors-underage':
        return 'Minors Underage';
    }
  }

  protected getInspectionTypeColor(type: InspectionType): string | undefined {
    switch (type) {
      case 'administration':
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200';
      case 'public-safety':
        return 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200';
      case 'minors-underage':
        return 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200';
    }
  }

  protected getRegionName(regionId: string): string {
    const region = this.map.regions().find((r) => r.id === regionId);
    return region ? region.name : regionId;
  }

  protected timeLabel(timestamp: string): string {
    return new Date(timestamp).toLocaleTimeString();
  }

  private filterInspectionsBySearch(inspections: PlannedInspection[]): PlannedInspection[] {
    const inspectionSearch = this.inspectionSearch();
    if (!inspectionSearch.trim()) return inspections;
    const query = inspectionSearch.toLowerCase();
    return inspections.filter((inspection) => {
      return (
        inspection.license.businessName.toLowerCase().includes(query) ||
        inspection.license.licenseNumber.toLowerCase().includes(query) ||
        inspection.assignedInspector.toLowerCase().includes(query) ||
        inspection.license.region?.toLowerCase().includes(query) ||
        inspection.license.city.toLowerCase().includes(query) ||
        inspection.inspectionType.toLowerCase().includes(query)
      );
    });
  }

  protected openCamera(): void {
    this.cameraInput()?.nativeElement.click();
  }

  protected openGallery(): void {
    this.galleryInput()?.nativeElement.click();
  }

  protected removeInspection(id: string): void {
    this.plannedInspections.set(this.plannedInspections().filter((i) => i.id !== id));
    if (this.activeInspection() === id) {
      this.goBackToList();
    }
  }

  protected getPriorityColor(priority: string): string {
    switch (priority) {
      case 'high':
        return 'destructive';
      case 'medium':
        return 'default';
      case 'low':
        return 'secondary';
      default:
        return 'default';
    }
  }

  protected async startCamera(): Promise<void> {
    const constraints = { video: true };
    try {
      const stream = await navigator.mediaDevices.getUserMedia(constraints);
      const video = this.videoRef()?.nativeElement;
      if (video) {
        video.srcObject = stream;
      }
      this.isTakingPhoto.set(true);
    } catch (err) {
      console.error('Error accessing camera:', err);
      alert('Could not access camera. Please check permissions.');
    }
  }

  protected stopCamera(): void {
    const video = this.videoRef()?.nativeElement;
    if (video && video.srcObject) {
      const stream = video.srcObject as MediaStream;
      const tracks = stream.getTracks();
      tracks.forEach((track) => track.stop());
      video.srcObject = null;
    }
    this.isTakingPhoto.set(false);
  }

  protected capturePhoto(): void {
    const video = this.videoRef()?.nativeElement;
    const canvas = this.canvasRef()?.nativeElement;
    if (video && canvas) {
      const context = canvas.getContext('2d');
      if (context) {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, canvas.width, canvas.height);
        const dataUrl = canvas.toDataURL('image/jpeg');

        const newPhoto: InspectionPhoto = {
          id: `photo-${Date.now()}`,
          dataUrl,
          timestamp: new Date().toISOString(),
          selected: false,
        };
        const activeInspection = this.activeInspection();
        this.plannedInspections.update((prev) =>
          prev.map((inspection) =>
            inspection.id === activeInspection ? { ...inspection, photos: [...(inspection.photos || []), newPhoto] } : inspection,
          ),
        );
      }
    }
  }

  protected getInspectorWorkloadFromSchedule(inspectorName: string): number {
    return [...this.plannedInspections(), ...this.aiGeneratedSchedule()].filter(
      (i) => i.assignedInspector === inspectorName && i.status !== 'completed',
    ).length;
  }

  protected handlePhotoUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if (!files || files.length === 0) return;

    const newPhotos: InspectionPhoto[] = [];
    const reader = new FileReader();

    reader.onload = (e) => {
      if (e.target?.result) {
        const newPhoto: InspectionPhoto = {
          id: `upload-${Date.now()}`,
          dataUrl: e.target.result as string,
          timestamp: new Date().toISOString(),
          selected: false,
        };
        newPhotos.push(newPhoto);
        if (newPhotos.length < files.length) {
          reader.readAsDataURL(files[newPhotos.length]);
        } else {
          const activeInspection = this.activeInspection();
          this.plannedInspections.update((prev) =>
            prev.map((inspection) =>
              inspection.id === activeInspection ? { ...inspection, photos: [...(inspection.photos || []), ...newPhotos] } : inspection,
            ),
          );
        }
      }
    };

    if (files.length > 0) {
      reader.readAsDataURL(files[0]);
    }

    // Reset the input value to allow selecting the same file again
    input.value = '';
  }

  protected togglePhotoReview(): void {
    this.isReviewingPhotos.set(!this.isReviewingPhotos());
  }

  protected deleteSelectedPhotos(): void {
    if (!this.activeInspection()) return;
    this.updateActivePhotos((photos) => photos?.filter((photo) => photo.selected));
    this.isReviewingPhotos.set(false);
  }

  protected generateAiSchedule(): void {
    this.isGenerating.set(true);

    // Simulate AI processing
    setTimeout(() => {
      const selectedRegionForAi = this.selectedRegionForAi();
      const eligibleLicenses = this.map.licenses().filter((license) => {
        if (license.status !== 'licensed') return false;

        if (selectedRegionForAi !== 'all' && license.region !== selectedRegionForAi) return false;

        const risk = calculateRiskScore(license.contraventions, license.complaints);
        if (risk === 'high' && !this.aiIncludeHighRisk()) return false;
        if (risk === 'medium' && !this.aiIncludeMediumRisk()) return false;
        if (risk === 'low' && !this.aiIncludeLowRisk()) return false;

        return true;
      });

      // Sort by risk priority, considering the aiPriorityWeight
      const sortedLicenses = eligibleLicenses.sort((a, b) => {
        const riskOrder = { high: 0, medium: 1, low: 2 };
        const riskA = calculateRiskScore(a.contraventions, a.complaints);
        const riskB = calculateRiskScore(b.contraventions, b.complaints);
        return riskOrder[riskA] - riskOrder[riskB];
      });

      // Generate schedule across weeks with workload balancing
      const generated: PlannedInspection[] = [];
      const inspectorWorkload: Record<string, number> = {};
      this.map.inspectors().forEach((i) => {
        inspectorWorkload[i.name] = 0;
      });

      const maxInspections = Math.min(sortedLicenses.length, this.aiScheduleWeeks() * 10);
      const today = new Date();

      sortedLicenses.slice(0, maxInspections).forEach((license, idx) => {
        const weekOffset = Math.floor(idx / 10);
        const dayOffset = (idx % 5) + weekOffset * 7; // Mon-Fri
        const scheduleDate = new Date(today);
        scheduleDate.setDate(today.getDate() + dayOffset + 1);

        const regionInspectors = this.map.inspectors().filter((i) => i.region === license.region);
        let leastBusyInspector = regionInspectors.length > 0 ? regionInspectors[0] : this.map.inspectors()[0];

        if (regionInspectors.length > 0) {
          leastBusyInspector = regionInspectors.reduce((min, curr) => ((inspectorWorkload[curr.name] || 0) < (inspectorWorkload[min.name] || 0) ? curr : min));
        }

        inspectorWorkload[leastBusyInspector.name] = (inspectorWorkload[leastBusyInspector.name] || 0) + 1;

        const risk = calculateRiskScore(license.contraventions, license.complaints);

        let determinedInspectionType: InspectionType = 'administration';
        if (risk === 'high') {
          determinedInspectionType = 'public-safety';
        } else if (risk === 'medium') {
          const types: InspectionType[] = ['administration', 'public-safety', 'minors-underage'];
          determinedInspectionType = types[Math.floor(Math.random() * 3)];
        }

        generated.push({
          id: `ai-${Date.now()}-${idx}`,
          license,
          scheduledDate: scheduleDate.toISOString().split('T')[0],
          assignedInspector: leastBusyInspector.name,
          priority: risk === 'high' ? 'high' : risk === 'medium' ? 'medium' : 'low',
          status: 'scheduled',
          inspectionType: determinedInspectionType,
          checklist: defaultChecklist.map((item) => ({ ...item })),
          inspectorNotes: '',
          photos: [],
        });
      });

      this.aiGeneratedSchedule.set(generated);
      this.isGenerating.set(false);
    }, 2000); // Simulate network latency/AI processing time
  }

  protected applyAiSchedule(): void {
    this.plannedInspections.update((prev) => [...prev, ...this.aiGeneratedSchedule()]);
    this.aiGeneratedSchedule.set([]);
  }

  protected setAiScheduleWeeks(value: string | undefined): void {
    this.aiScheduleWeeks.set(Number.parseInt(value as string));
  }
}
