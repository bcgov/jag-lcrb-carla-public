import { Component, OnInit, ChangeDetectorRef, Input, Output, EventEmitter } from '@angular/core';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/licensee-change-log.model';
import { Application } from '@models/application.model';
import { LegalEntity } from '@models/legal-entity.model';
import { MatDialog, MatSnackBar } from '@angular/material';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { takeWhile, filter } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';
import { LicenseeTreeComponent } from '../licensee-tree/licensee-tree.component';

@Component({
  selector: 'app-personal-history-summary',
  templateUrl: './personal-history-summary.component.html',
  styleUrls: ['./personal-history-summary.component.scss']
})
export class PersonalHistorySummaryComponent extends FormBase implements OnInit {
  @Input() personalHistoryItems: LicenseeChangeLog[] = [];
  @Input() rootNode: LicenseeChangeLog;
  @Input() changeTypeSuffix: string;
  @Input() addLabel: string = 'Add Associate';
  businessType: string = 'Society';
  @Output() childAdded = new EventEmitter<LicenseeChangeLog>();

  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;

  constructor(private fb: FormBuilder) {
    super();
  }


  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      contactPersonFirstName: ['', Validators.required],
      contactPersonLastName: ['', Validators.required],
      contactPersonRole: [''],
      amalgamationDone: [''],
      contactPersonEmail: ['', Validators.required],
      contactPersonPhone: ['', Validators.required],
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
    });
  }

  addAssociate() {
    const associate = new LicenseeChangeLog();
    associate.edit = true;
    associate.changeType = `add${this.changeTypeSuffix}`;
    this.childAdded.emit(associate);

  }

  showPosition(): boolean {
    return this.businessType === 'Society'
      || this.businessType === 'PublicCorporation';
  }

}
