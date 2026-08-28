import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { Account, TransferAccount } from '@models/account.model';
import { AccountDataService } from '@services/account-data.service';
import { filter, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-account-picker',
  templateUrl: './account-picker.component.html',
  styleUrls: ['./account-picker.component.scss']
})
export class AccountPickerComponent implements OnInit {
  @ViewChild('autocomplete', { read: MatAutocompleteTrigger, static: true })
  inputAutoComplit: MatAutocompleteTrigger;
  @Output()
  valueSelected = new EventEmitter<string>();
  form: FormGroup;
  autocompleteAccounts: any[];
  accountRequestInProgress: boolean;

  constructor(
    private accountDataService: AccountDataService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      autocompleteInput: ['']
    });

    this.form
      .get('autocompleteInput')
      .valueChanges.pipe(
        filter((value) => value && value.length >= 3),
        tap((_) => {
          this.autocompleteAccounts = [];
          this.accountRequestInProgress = true;
        }),
        switchMap((value) => this.accountDataService.getAutocomplete(value))
      )
      .subscribe((data) => {
        data.forEach((item) => {
          const account = new Account();
          account.businessType = item.businessType;
          item.businessType = account.getBusinessTypeName();
        });
        this.autocompleteAccounts = data;
        this.accountRequestInProgress = false;
        this.inputAutoComplit.openPanel();
      });
  }

  autocompleteDisplay(item: TransferAccount) {
    return item.accountName;
  }

  onOptionSelect(event) {
    this.valueSelected.emit(event.option.value);
  }
}
