import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { AccountDataService } from '@services/account-data.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-sep-all-applications',
  templateUrl: './all-applications.component.html',
  styleUrls: ['./all-applications.component.scss']
})
export class AllApplicationsComponent extends FormBase implements OnInit {

  // form
  form = this.fb.group({
  });

  constructor(
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private fb: FormBuilder,
    private router: Router,
  ) {
    super();
  }

  ngOnInit() {
  }

}
