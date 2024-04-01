import { Component, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { MatAutocompleteTrigger } from "@angular/material/autocomplete";
import { FormBuilder, FormGroup } from "@angular/forms";
import { AccountDataService } from "@services/account-data.service";
import { filter, tap, switchMap } from "rxjs/operators";
import { LicenseDataService } from "@services/license-data.service";
import { RelatedLicence } from "@models/related-licence";

@Component({
  selector: 'app-related-licence-picker',
  templateUrl: './related-licence-picker.component.html',
  styleUrls: ['./related-licence-picker.component.scss']
})
export class RelatedLicencePickerComponent implements OnInit {
  @ViewChild("autocomplete", { read: MatAutocompleteTrigger, static: true })
  inputAutoComplete: MatAutocompleteTrigger;
  @Output() valueSelected = new EventEmitter<string>();
  @Output() autoCompFldFocusEvent = new EventEmitter<string>();
  form: FormGroup;
  autocompleteLicences: any[];
  licenceRequestInProgress: boolean;


  constructor(private licenceDataService: LicenseDataService,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      autocompleteInput: [""]
    });

    this.form.get("autocompleteInput").valueChanges
      .pipe(filter(value => value && value.length >= 3),
        tap(_ => {
          this.autocompleteLicences = [];
          this.licenceRequestInProgress = true;
        }),
        switchMap(value => this.licenceDataService.getAutocomplete(value))
      )
      .subscribe(data => {        
        this.autocompleteLicences = data;
        this.licenceRequestInProgress = false;
        this.inputAutoComplete.openPanel();
      });
  }

  
  autoCompFldFocus() {
    // when the cursor enters the autoCompleteJobNumber field, let parent know name of field
    this.autoCompFldFocusEvent.emit("autocompleteInput");
  }

  autoCompFldClear() {
    // clear field
    this.form.get("autocompleteInput").setValue('');
    this.autocompleteLicences = null;
  }

  autocompleteDisplay(item: RelatedLicence) {
    return item.name;
  }

  onOptionSelect(event) {
    this.valueSelected.emit(event.option.value);
  }



}
