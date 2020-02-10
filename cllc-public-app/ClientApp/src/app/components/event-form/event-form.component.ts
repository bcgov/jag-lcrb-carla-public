import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';
import { SpecificLocation, EventClass, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { takeWhile, switchMap } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { User } from '@models/user.model';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.scss']
})
export class EventFormComponent extends FormBase implements OnInit {
  licenceEvent: LicenceEvent;

  busy: Subscription;
  specificLocation = SpecificLocation;
  foodService = FoodService;
  entertainment = Entertainment;
  eventClass = EventClass;
  eventType = EventType;
  eventStatus = EventStatus;

  eventForm = this.fb.group({
    status: ['', [Validators.required]],
    licenceId: ['', []],
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],
    eventType: ['', [Validators.required]],
    description: ['', [Validators.required]],
    clientHostname: ['', [Validators.required]],
    maxAttendance: ['', [Validators.required]],
    minorsAttending: ['', [Validators.required]],
    foodService: ['', [Validators.required]],
    foodServiceDescription: ['', []],
    entertainmentProvided: ['', [Validators.required]],
    entertainmentDescription: ['', []],
    venueDescription: ['', [Validators.required]],
    specificLocation: ['', [Validators.required]],
    additionalInformation: ['', []],
    street1: ['', [Validators.required]],
    street2: ['', []],
    city: ['', [Validators.required]],
    postalCode: ['', [Validators.required]],
    agreement: ['', [Validators.required]]
  });

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
    ) {
      super();
      this.route.paramMap.subscribe(params => {
        this.eventForm.controls['licenceId'].setValue(params.get('licenceId'));
        console.log(params);
        // if (params.get('licenceId')) {
        // }
      });
    }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: User) => {
        this.eventForm.controls['contactEmail'].setValue(data.email);
      });

    if (true) {
      this.eventForm.controls['status'].setValue(this.eventStatus.Draft.toString());
    }
  }

  save(submit = false) {
    if (submit) {
      this.eventForm.controls['status'].setValue(this.eventStatus.InReview.toString());
    }
    this.busy = this.licenceEvents.createLicenceEvent(this.eventForm.value)
    .subscribe((licenceEvent) => {
      console.log(licenceEvent);
      this.router.navigate(['/licences']);
    });
  }
}
