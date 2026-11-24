/** SEP submit flow: builds the permit from the form, submits to the SEP store, deletes the draft, flushes the store and opens the list with the permit highlighted. */
import { ApplicationRef, inject, Injectable } from '@angular/core';
import { path } from '@/app/route-registry';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { jurisdictionForCity } from '@domain/special-events/seps';

@Injectable({ providedIn: 'root' })
export class SepApplyFacade {
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  private readonly sepStore = this.specialEventsRepository.store();
  private readonly appRef = inject(ApplicationRef);

  /** Submits the application in `formData` and navigates to the permit list; returns the new permit id. */
  submit(formData: Record<string, unknown>): string {
    const city = (formData['eventCity'] as string) || (formData['locationName'] as string) || 'Victoria';
    const jurisdiction = jurisdictionForCity(city);
    const id = this.sepStore.submitApplication({
      eventName: (formData['eventName'] as string) || 'Untitled event',
      eventType: (formData['eventType'] as string) || 'private',
      applicantName: 'Tyee_firstname_testing LRCB Training_surname_testing',
      venueName: (formData['locationName'] as string) || 'Event venue',
      address: (formData['addressLine1'] as string) || '',
      city,
      jurisdiction: jurisdiction.name,
      lat: jurisdiction.lat,
      lng: jurisdiction.lng,
      startDate: (formData['eventDate'] as string) || (formData['eventStartDate'] as string) || '',
      endDate: (formData['eventDate'] as string) || (formData['eventStartDate'] as string) || '',
      // Carry the service window through to the permit record; without these the summary page showed "— – —" even though both times were entered.
      startTime: (formData['eventStartTime'] as string) || '',
      endTime: (formData['eventEndTime'] as string) || '',
      attendance: Number(formData['maxGuests']) || 0,
      serviceType: (formData['liquorSaleType'] as string) === 'selling' ? 'Sale of liquor' : 'Free of charge',
      description: (formData['eventOccasion'] as string) || '',
      liquorTypes: Array.isArray(formData['liquorTypes']) ? (formData['liquorTypes'] as string[]) : undefined,
      estimatedDrinks: formData['estimatedDrinks'] ? Number(formData['estimatedDrinks']) : undefined,
      hasSecurity: formData['hasSecurity'] ? formData['hasSecurity'] === 'yes' : undefined,
      securityStaffCount: formData['securityStaffCount'] ? Number(formData['securityStaffCount']) : undefined,
      isPublicProperty: formData['isPublicProperty'] === 'yes',
    });
    this.applicationsRepository.deleteDraft('draft:sep');
    // The store persists from an effect; flush it before the page reloads.
    this.appRef.tick();
    window.location.href = `${path('special-events.list')}?submitted=${encodeURIComponent(id)}`;
    return id;
  }
}
