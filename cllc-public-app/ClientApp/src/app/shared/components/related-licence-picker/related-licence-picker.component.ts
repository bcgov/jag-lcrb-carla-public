import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { RelatedLicence } from '@models/related-licence';
import { LicenseDataService } from '@services/license-data.service';
import { filter, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-related-licence-picker',
  templateUrl: './related-licence-picker.component.html',
  styleUrls: ['./related-licence-picker.component.scss']
})
export class RelatedLicencePickerComponent implements OnInit {
  @ViewChild('autocomplete', { read: MatAutocompleteTrigger, static: true })
  inputAutoComplete: MatAutocompleteTrigger;
  @Output() valueSelected = new EventEmitter<string>();
  @Output() autoCompFldFocusEvent = new EventEmitter<string>();
  form: FormGroup;
  autocompleteLicences: any[];
  licenceRequestInProgress: boolean;

  constructor(
    private licenceDataService: LicenseDataService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      autocompleteInput: ['']
    });

    this.form
      .get('autocompleteInput')
      .valueChanges.pipe(
        filter((value) => value && value.length >= 3),
        tap((_) => {
          this.autocompleteLicences = [];
          this.licenceRequestInProgress = true;
        }),
        switchMap((value) => this.licenceDataService.getAutocomplete({ name: value, licenceNumber: value }))
      )
      .subscribe((data) => {
        this.autocompleteLicences = data;
        this.licenceRequestInProgress = false;
        this.inputAutoComplete.openPanel();
      });
  }

  autoCompFldFocus() {
    // when the cursor enters the autoCompleteJobNumber field, let parent know name of field
    this.autoCompFldFocusEvent.emit('autocompleteInput');
  }

  autoCompFldClear() {
    // clear field
    this.form.get('autocompleteInput').setValue('');
    this.autocompleteLicences = null;
  }

  autocompleteDisplay(item: RelatedLicence) {
    return item.name;
  }

  onOptionSelect($event) {
    let selectedLicence = $event.option.value as RelatedLicence;

    this.valueSelected.emit($event.option.value);
  }
}
