import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';

@Component({
  selector: 'app-multi-stage-application-flow',
  templateUrl: './multi-stage-application-flow.component.html',
  styleUrls: ['./multi-stage-application-flow.component.scss']
})
export class MultiStageApplicationFlowComponent implements OnInit {
  licenseeApplicationId: string;

  constructor(private store: Store<AppState>) {
    store.select(state => state.onGoingLicenseeChangesApplicationIdState.onGoingLicenseeChangesApplicationId)
    .subscribe(id => {
      this.licenseeApplicationId = id;
    })
   }

  ngOnInit() {
  }

  selectionChange(event) {
  }

}
