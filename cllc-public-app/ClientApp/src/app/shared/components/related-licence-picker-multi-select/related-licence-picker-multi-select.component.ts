import { Component, OnInit, ViewChild, Output, EventEmitter, ElementRef, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { filter, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { LicenseDataService } from '@services/license-data.service';
import { RelatedLicence } from '@models/related-licence';

@Component({
  selector: 'app-related-licence-picker-multi-select',
  templateUrl: './related-licence-picker-multi-select.component.html',
  styleUrls: ['./related-licence-picker-multi-select.component.scss']
})
export class RelatedLicencePickerMulitiSelectComponent implements OnInit {
  @ViewChild('autocomplete', { read: MatAutocompleteTrigger, static: true })
  inputAutoComplete: MatAutocompleteTrigger;

  @ViewChild('inputElement', { static: false }) inputElement: ElementRef;

  @Output() selectedLicencesChange = new EventEmitter<any[]>();
  @Output() autoCompFldFocusEvent = new EventEmitter<string>();


  @Input() form: FormGroup;
  autocompleteLicences: any[];
  @Input() selectedLicences: any[] = [];
  licenceRequestInProgress: boolean;

  constructor(private licenceDataService: LicenseDataService,
    private fb: FormBuilder) { }

  get associatedLicencesFormArray(): FormArray {
    return this.form.get('associatedLiquorLicense') as FormArray;
  }

  ngOnInit(): void {
    if(!this.form)
    this.form = this.fb.group({
      autocompleteInput: [''],
      associatedLiquorLicense: this.fb.array([]),
    });

    this.form.get("autocompleteInput").valueChanges
      .pipe(filter(value => value && value.length >= 3),
        tap(_ => {
          this.autocompleteLicences = [];
          this.licenceRequestInProgress = true;
        }),
        switchMap(value => this.licenceDataService.getAutocomplete({
          name: value,
          licenceNumber: value
        }))
      )
      .subscribe(data => {
        this.autocompleteLicences = data.filter(l => !this.selectedLicences.some(sl => sl.id === l.id));
        this.licenceRequestInProgress = false;
        this.inputAutoComplete.openPanel();
      });
  }

  autoCompFldFocus(): void {
    this.autoCompFldFocusEvent.emit('autocompleteInput');
  }

  autoCompFldClear() {
    // clear field
    this.form.get("autocompleteInput").setValue('');
    this.autocompleteLicences = null;
  }

  autocompleteDisplay(item: RelatedLicence): string {
    return item.name;
  }

  onOptionSelect($event: any): void {
    var x = $event;
    const selectedLicence = $event.option.value as RelatedLicence;

    // Avoid duplicates
    if (!this.selectedLicences.some(l => l.id === selectedLicence.id)) {
      this.selectedLicences.push(selectedLicence);
      //this.selectedLicencesChange.emit(this.selectedLicences);

      // Add to FormArray
      const formArray = this.form.get('associatedLiquorLicense') as FormArray;
      formArray.push(this.fb.control(selectedLicence)); // Add full object or selectedLicence.id depending on your needs
    }

    this.form.get("autocompleteInput").setValue('');
    this.autocompleteLicences = [];
  }

  removeLicenceAtIndex(index: number): void {
    this.associatedLicencesFormArray.removeAt(index);
  }
}
