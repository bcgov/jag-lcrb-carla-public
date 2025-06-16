import { Component, OnInit, ViewChild, Output, EventEmitter } from "@angular/core";
import { MatAutocompleteTrigger } from "@angular/material/autocomplete";
import { FormBuilder, FormGroup } from "@angular/forms";
import { filter, tap, switchMap } from "rxjs/operators";
import { ApplicationDataService } from "@services/application-data.service";
import { LicenseDataService } from "@services/license-data.service";
import { RelatedLicence } from "@models/related-licence";
import { exit } from "process";


@Component({
  selector: 'app-related-jobnumber-picker',
  templateUrl: './related-jobnumber-picker.component.html',
  styleUrls: ['./related-jobnumber-picker.component.scss']
})
export class RelatedJobnumberPickerComponent implements OnInit {
  @ViewChild("autocomplete", { read: MatAutocompleteTrigger, static: true })
  inputAutoComplete: MatAutocompleteTrigger;
  @Output() valueSelected = new EventEmitter<string>();
  @Output() autoCompFldFocusEvent = new EventEmitter<string>();
  form: FormGroup;
  autocompleteJobnumbers: any[];
  jobnumberRequestInProgress: boolean;


  constructor(private applicationDataService: ApplicationDataService,
    private licenseDataService: LicenseDataService,
    private fb: FormBuilder) { }

  // 2024-03-27 LCSD - 6368 waynezen; Tied House form autocomplete component search by JobNumber
  ngOnInit(): void {
    this.form = this.fb.group({
      autocompleteJobNumber: [""]
    });

    this.form.get("autocompleteJobNumber").valueChanges
      .pipe(filter(value => value && value.length >= 6),
        tap(_ => {
          this.autocompleteJobnumbers = [];
          this.jobnumberRequestInProgress = true;
        }),
        switchMap(value => this.applicationDataService.getAutocomplete(value))
      )
      .subscribe(data => {
        this.autocompleteJobnumbers = data;
        this.jobnumberRequestInProgress = false;
        this.inputAutoComplete.openPanel();
      });

  }

  autoCompFldFocus() {
    // when the cursor enters the autoCompleteJobNumber field, let parent know name of field
    this.autoCompFldFocusEvent.emit("autocompleteJobNumber");
  }

  autoCompFldClear() {
    // clear field
    this.form.get("autocompleteJobNumber").setValue('');
    this.autocompleteJobnumbers = null;
  }

  autocompleteDisplay(item: RelatedLicence) {

    if (item) {
      return item.name;
    }
  }

  onOptionSelect($event) {

    let selectedLicence = $event.option.value as RelatedLicence;

    if (selectedLicence && selectedLicence.valid) {

      // use the licence # autocomplete function to get missing Licensee name
      this.licenseDataService.getAutocomplete({ licenceNumber: selectedLicence.licenceNumber })
        .subscribe((data) => {
          if (data && data.length > 0) {
            let lookupLicence = data[0] as RelatedLicence;

            /*
             * NOTE: ApplicationsController.GetAutoComplete sets RelatedLicence.Id = <jobnumber>
             * because it doesn't have the necessary Licence Id.
             * When the user selects the item they want, we "fix" RelatedLicence.Id
             * to the Licence Id, so the Save() function on application-tied-house-exemption will work.
            */

            $event.option.value.licensee = lookupLicence.licensee;
            $event.option.value.id = lookupLicence.id;

            this.valueSelected.emit($event.option.value);
          }
        });

      this.valueSelected.emit($event.option.value);
    }
  }


}
