import { Component, OnInit } from "@angular/core";
import { UserDataService } from "@services/user-data.service";
import { User } from "@models/user.model";
import { Worker } from "@models/worker.model";
import { ContactDataService } from "@services/contact-data.service";
import { Contact } from "@models/contact.model";
import { forkJoin, Subscription } from "rxjs";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { takeWhile } from "rxjs/operators";
import { FormBase } from "@shared/form-base";
import { FormGroup, FormControl } from "@angular/forms";
import { faDownload, faQuestion } from "@fortawesome/free-solid-svg-icons";
import { WorkerDataService } from "@services/worker-data.service";


@Component({
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"]
})
export class WorkerDashboardComponent extends FormBase implements OnInit {
  faDownload = faDownload;
  faQuestion = faQuestion;
  currentUser: User;
  displayedColumns = ["lastUpdated", "worker", "status"];
  dataSource: Worker[] = [];
  isNewUser: boolean;
  dataLoaded = false;
  applicationStatus: string;
  // numberOfApplications = 0;
  id: string;
  firstName: string;
  lastName: string;
  mainForm: FormGroup;
  phone: string;
  email: string;
  dataTable: any[];

  busy: Subscription;
  currentApplication: Worker;

  // @ViewChild(MatPaginator) paginator: MatPaginator;
  constructor(
    private userDataService: UserDataService,
    private workerDataService: WorkerDataService,
    private contactDataService: ContactDataService,
    private store: Store<AppState>
  ) {
    super();
    this.mainForm = new FormGroup({});

  }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(user => this.loadUser(user));

    // add phone and email to the dashboard so the user can modify them
    this.mainForm.addControl("phone", new FormControl(""));
    this.mainForm.addControl("email", new FormControl(""));


  }

  loadUser(user: User) {
    this.currentUser = user;
    this.isNewUser = this.currentUser.isNewUser;
    this.dataLoaded = true;
    if (this.currentUser && this.currentUser.contactid) {
      this.busy = this.workerDataService.getWorkerByContactId(this.currentUser.contactid).subscribe(res => {
        this.dataSource = res;
        this.currentApplication = res[0];
        this.dataTable = [
          { key: "First", value: this.currentApplication.firstname },
          { key: "Middle", value: this.currentApplication.middlename },
          { key: "Last", value: this.currentApplication.lastname },
          { key: "Sex", value: this.currentApplication.gender },
          { key: "Street Address", value: this.currentApplication.contact.address1_line1 },
          { key: "State or Province", value: this.currentApplication.contact.address1_stateorprovince },
          { key: "Postal Code", value: this.currentApplication.contact.address1_postalcode },
          { key: "Country", value: this.currentApplication.contact.address1_country },
        ];

        // set the values of the phone and email to what we have on file.      
        this.mainForm.controls["phone"].setValue(this.currentApplication.contact.mobilePhone);
        this.mainForm.controls["email"].setValue(this.currentApplication.contact.emailaddress1);

        // retrieve status of the most recent application
        this.setClientSideStatus(this.currentApplication);
        this.applicationStatus = this.getStatus(res);
        const passedApplications = res.filter(i => (i as any).status === "Active");

        if (passedApplications.length > 0) {
          this.displayedColumns.push("actions");
        }
      });


    }
  }

  // when the phone field is updated, update the contact record
  updatePhone(event: any) {
    if (event.target.value === null) {
      return false;
    }

    const phone = this.mainForm.controls["phone"].value;
    const contact = Object.assign(new Contact(),
      {
        id: this.currentUser.contactid,
        mobilePhone: phone
      });

    this.updateContact(contact);

  }

  // when the email field is updated, update the contact record
  updateEmail(event: any) {
    if (event.target.value === null) {
      return false;
    }

    const email = this.mainForm.controls["email"].value;

    const contact = Object.assign(new Contact(),
      {
        id: this.currentUser.contactid,
        emailaddress1: email,
      });
    this.updateContact(contact);
  }

  // take the provided contact record and update it with the changes implemented
  updateContact(contact: Contact) {

    this.busy = forkJoin([
        this.contactDataService.updateContact(contact)
      ])
      .subscribe(([resp]) => {
        contact = resp;
      });
  }


  setClientSideStatus(worker: Worker) {
    worker.clientSideStatus = worker.status;
    if (!worker.paymentReceived) {
      worker.clientSideStatus = "Not Completed";
    } else if (worker.paymentReceived &&
      worker.status !== "Active" &&
      worker.status !== "Withdrawn" &&
      worker.status !== "Rejected" &&
      worker.status !== "Revoked" &&
      worker.status !== "Expired") {
      worker.clientSideStatus = "Pending Review";
    }
  }

  getStatus(res) {
    // if we've received payment
    if (res[0].status === "Not Submitted" && this.isNewUser) {
      return "Not Started";
    }
    return res[0].status;

  }

  getName(res) {
    return;
  }

}
