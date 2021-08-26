import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AppState } from "@app/app-state/models/app-state";
import { Account } from "@models/account.model";
import { User } from "@models/user.model";
import { Store } from "@ngrx/store";
import { SpecialEventsDataService } from "@services/special-events-data.service";

@Component({
  selector: "app-sep-claim",
  templateUrl: "./sep-claim.component.html",
  styleUrls: ["./sep-claim.component.scss"]
})
export class SepClaimComponent implements OnInit {
  jobNumber = "<JOB_NUM>";
  associatedContactId = "<GUID_HERE>";
  user: User;
  claimInfo: any;
  dataLoaded: boolean;
  constructor(
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute,
    private sepDataService: SpecialEventsDataService
  ) {
    this.store.select(state => state.currentUserState.currentUser)
    .subscribe(user => this.user = user);

      this.route.paramMap.subscribe(pmap => {
        this.jobNumber = pmap.get("jobNumber");
        this.sepDataService.getClaimInfo(this.jobNumber)
        .subscribe(data => {
          this.claimInfo = data;
          this.dataLoaded = true;
        });
      });
   }

  ngOnInit(): void {
  }

  linkToAccount() {
    this.sepDataService.linkClaimToContact(this.jobNumber)
    .subscribe(data => {
      this.router.navigateByUrl("/sep/my-applications");
    });
  }

}
