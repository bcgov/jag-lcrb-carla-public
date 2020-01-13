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
  @Input() account: Account;
  @Output() childAdded = new EventEmitter<LicenseeChangeLog>();
  applicationId: string;
  application: Application;
  currentChangeLogs: LicenseeChangeLog[];
  currentLegalEntities: LegalEntity;

  editedTree: LicenseeChangeLog;
  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  busySave: any;
  numberOfNonTerminatedApplications: number;
  cancelledLicenseeChanges: LicenseeChangeLog[] = [];

  constructor(public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private fb: FormBuilder,
    public cd: ChangeDetectorRef,
    public router: Router,
    private store: Store<AppState>,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private legalEntityDataService: LegalEntityDataService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
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

    // this.store.select(state => state.currentAccountState.currentAccount)
    //   .pipe(takeWhile(() => this.componentActive))
    //   .pipe(filter(account => !!account))
    //   .subscribe((account) => {
    //     this.account = account;
    //   });

    // this.loadData();
  }

  // loadData() {
  //   // this.busy = forkJoin(this.applicationDataService.getApplicationById(this.applicationId),
  //   //   this.legalEntityDataService.getChangeApplicationLogs(this.applicationId),
  //   //   this.legalEntityDataService.getCurrentHierachy())
  //   //   .pipe(takeWhile(() => this.componentActive))
  //   //   .subscribe((data: [Application, LicenseeChangeLog[], LegalEntity]) => {
  //   //     this.application = data[0];
  //   //     const currentChangeLogs = data[1] || [];
  //   //     const currentLegalEntities = data[2];
  //   //     const tree = LicenseeChangeLog.processLegalEntityTree(currentLegalEntities);
  //   //     tree.isRoot = true;
  //   //     tree.applySavedChangeLogs(currentChangeLogs);
  //   //     //flatten the tree
  //   //     this.personalHistoryItems = this.flattenChangeLogs(tree);

  //   //   },
  //   //     () => {
  //   //       console.log('Error occured');
  //   //     }
  //   //   );
  // }

  flattenChangeLogs(node: LicenseeChangeLog): LicenseeChangeLog[] {
    let flatNodes: LicenseeChangeLog[] = [];

    if (node.children) {
      flatNodes = flatNodes.concat(node.children);
      node.children.forEach(child => {
        flatNodes = flatNodes.concat(this.flattenChangeLogs(child));
      });
    }
    return flatNodes;
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
