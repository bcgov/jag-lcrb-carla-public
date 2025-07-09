import { Component, OnInit } from "@angular/core";
import { UserDataService } from "@services/user-data.service";
import { User } from "@models/user.model";
import { ContactDataService } from "@services/contact-data.service";
import { Contact } from "@models/contact.model";
import * as CurrentUserActions from "@app/app-state/actions/current-user.action";
import { Store } from "@ngrx/store";
import { Subscription, Observable, Subject, zip, forkJoin } from "rxjs";
import { FormBuilder, FormGroup, Validators, FormArray, FormControl } from "@angular/forms";
import { AliasDataService } from "@services/alias-data.service";
import { PreviousAddressDataService } from "@services/previous-address-data.service";
import { Alias } from "@models/alias.model";
import { PreviousAddress } from "@models/previous-address.model";
import { ActivatedRoute, Router } from "@angular/router";
import { COUNTRIES } from "@app/constants/countries";
import { FormBase } from "@shared/form-base";
import { Worker } from "@models/worker.model";
import { faCopy, faSave } from "@fortawesome/free-regular-svg-icons";
import { faChevronLeft, faChevronRight, faExclamationTriangle, faQuestion, faTrash } from "@fortawesome/free-solid-svg-icons";

import { AppState } from "@app/app-state/models/app-state";
import { WorkerDataService } from "@services/worker-data.service";

@Component({
  selector: "app-worker-application",
  templateUrl: "./worker-application.component.html",
  styleUrls: ["./worker-application.component.scss"],
})
export class WorkerApplicationComponent extends FormBase implements OnInit {
  faSave = faSave;
  faTrash = faTrash;
  faCopy = faCopy;
  faQuestion = faQuestion;
  faChevronLeft = faChevronLeft;
  faChevronRight = faChevronRight;
  faExclamationTriangle = faExclamationTriangle;
  currentUser: User;
  dataLoaded = false;
  busy: Subscription;
  busy2: Promise<any>;
  form: FormGroup;
  countryList = COUNTRIES;

  addressesToDelete: PreviousAddress[] = [];
  aliasesToDelete: Alias[] = [];
  workerId: string;
  saveFormData: any;
  workerStatus: string;

  currentDate: Date = new Date();
  minDate: Date;
  bsConfig: any = { locale: "en", dateInputFormat: "YYYY-MM-DD", containerClass: "theme-dark-blue" };
  worker: Worker;

  get addresses(): FormArray {
    return this.form.get("addresses") as FormArray;
  }

  get aliases(): FormArray {
    return this.form.get("worker.aliases") as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private aliasDataService: AliasDataService,
    private previousAddressDataService: PreviousAddressDataService,
    private contactDataService: ContactDataService,
    private workerDataService: WorkerDataService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    super();
    // minDate is a 100 year ago
    this.minDate = new Date();
    this.minDate.setFullYear(this.minDate.getFullYear() - 100);

    this.route.paramMap.subscribe(params => {
      this.workerId = params.get("id");
    });
  }

  ngOnInit() {
    this.form = this.fb.group({
      contact: this.fb.group({
        id: [],
        firstname: [""],
        middlename: [""],
        lastname: [""],
        emailaddress1: [""],
        telephone1: [""],
        address1_line1: [""],
        address1_city: [""],
        address1_stateorprovince: [""],
        address1_country: [""],
        address1_postalcode: [""],
        address2_line1: ["", Validators.required],
        address2_city: ["", Validators.required],
        address2_stateorprovince: ["", Validators.required],
        address2_country: ["", Validators.required],
        address2_postalcode: ["", [Validators.required, this.customZipCodeValidator("address2_country")]],

        birthPlace: ["", Validators.required],
        gender: [""],
        mobilePhone: ["", Validators.required],
        primaryIdNumber: [""],
        secondaryIdNumber: [""]
      }),
      worker: this.fb.group({
        id: [],
        isldbworker: [false],
        firstname: [{ value: "", disabled: true }],
        middlename: [{ value: "", disabled: true }],
        lastname: [{ value: "", disabled: true }],
        dateofbirth: [{ value: "", disabled: true }],
        email: ["", [Validators.required, Validators.email]],
        fromdate: ["", Validators.required],
        todate: [{ value: new Date(), disabled: true }],
        aliases: this.fb.array([
        ]),
      }),
      addresses: this.fb.array([
      ])
    });
    this.reloadUser();
  }

  reloadUser() {
    this.busy = this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
        this.store.dispatch(new CurrentUserActions.SetCurrentUserAction(data));
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.contactid) {
          this.busy2 = forkJoin(
            this.workerDataService.getWorker(this.workerId),
            this.aliasDataService.getAliases(this.currentUser.contactid),
            this.previousAddressDataService.getPreviousAdderesses(this.currentUser.contactid)
          ).toPromise().then(res => {
            const worker = res[0];
            const contact = worker.contact;
            delete worker.contact;
            const aliases = res[1];
            const addresses = res[2];
            this.form.patchValue({
              worker: worker,
              contact: contact,
            });

            this.clearAliases();
            aliases.forEach(alias => {
              this.addAlias(alias);
            });

            this.clearAddresses();
            addresses.forEach(address => {
              this.addAddress(address);
            });

            this.saveFormData = this.form.value;
            this.worker = (worker as any);
            this.workerStatus = worker.status;
            if (worker.status !== "Application Incomplete") {
              this.form.disable();
            }
          });
        }
      });
  }

  confirmContact(confirm: boolean) {
    if (confirm) {
      // create contact here
      const contact = new Contact();
      contact.fullname = this.currentUser.name;
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      this.busy = this.contactDataService.createWorkerContact(contact).subscribe(res => {
          this.reloadUser();
        },
        error => alert("Failed to create contact"));
    } else {
      window.location.href = "logout";
    }
  }

  createAddress(address: PreviousAddress = null) {
    address = address ||
    ({
      id: undefined,
      streetaddress: "",
      city: "",
      provstate: "",
      country: "Canada",
      postalcode: "",
      fromdate: "",
      todate: ""
    } as PreviousAddress);
    return this.fb.group({
      id: [address.id],
      streetaddress: [address.streetaddress, Validators.required],
      city: [address.city, Validators.required],
      provstate: [address.provstate, Validators.required],
      country: [address.country, Validators.required],
      postalcode: [address.postalcode, [Validators.required, this.customZipCodeValidator("country")]],
      fromdate: [address.fromdate, Validators.required],
      todate: [address.todate, Validators.required]
    });
  }

  addAddress(address: PreviousAddress = null) {
    this.addresses.push(this.createAddress(address));
  }

  copyPhysicalAddressToMailingAddress(): void {
    let contact = this.form.get("contact").value;
    contact = {
      address2_line1: contact.address1_line1,
      address2_city: contact.address1_city,
      address2_stateorprovince: contact.address1_stateorprovince,
      address2_country: contact.address1_country,
      address2_postalcode: contact.address1_postalcode,
    };
    this.form.get("contact").patchValue(contact);
  }

  deleteAddress(index: number) {
    const address = this.addresses.controls[index];
    if (address.value.id) {
      this.addressesToDelete.push(address.value);
    }
    this.addresses.removeAt(index);
  }

  clearAddresses() {
    for (let i = this.addresses.controls.length; i > 0; i--) {
      this.addresses.removeAt(0);
    }
  }

  addAlias(alias: Alias = null) {
    this.aliases.push(this.createAlias(alias));
  }

  deleteAlias(index: number) {
    const alias = this.aliases.controls[index];
    if (alias.value.id) {
      this.aliasesToDelete.push(alias.value);
    }
    this.aliases.removeAt(index);
  }

  clearAliases() {
    for (let i = this.aliases.controls.length; i > 0; i--) {
      this.aliases.removeAt(0);
    }
  }

  createAlias(alias: Alias = null) {
    alias = alias ||
    ({
      firstname: "",
      middlename: "",
      lastname: ""
    } as Alias);
    return this.fb.group({
      id: [alias.id],
      firstname: [alias.firstname, Validators.required],
      middlename: [alias.middlename],
      lastname: [alias.lastname, Validators.required],
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.workerStatus !== "Application Incomplete" ||
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Subject<boolean> {
    const subResult = new Subject<boolean>();
    const value = { ...this.form.value };
    // Make sure the contact email and phone number are in sync with worker
    value.contact.emailaddress1 = value.worker.email;
    value.contact.birthDate = this.worker.dateofbirth;

    // copy fields from contact to worker
    value.worker.birthplace = value.contact.birthPlace;
    value.worker.driverslicencenumber = value.contact.primaryIdNumber;
    value.worker.bcidcardnumber = value.contact.secondaryIdNumber;
    value.worker.phonenumber = value.contact.mobilePhone;


    const saves: Observable<any>[] = [
      this.contactDataService.updateContact(value.contact),
      this.workerDataService.updateWorker(value.worker, value.worker.id)
    ];

    this.addressesToDelete.forEach(a => {
      const save = this.previousAddressDataService.deletePreviousAddress(a.id);
      saves.push(save);
    });

    const addressControls = this.addresses.controls;
    for (let i = 0; i < addressControls.length; i++) {
      if (addressControls[i].value.id) {
        const save =
          this.previousAddressDataService.updatePreviousAdderess(addressControls[i].value, addressControls[i].value.id);
        saves.push(save);
      } else {
        const newAddress = addressControls[i].value;
        newAddress.contactId = value.contact.id;
        newAddress.workerId = value.worker.id;
        const save = this.previousAddressDataService.createPreviousAdderess(newAddress);
        saves.push(save);
      }
    }

    this.aliasesToDelete.forEach(a => {
      const save = this.aliasDataService.deleteAlias(a.id);
      saves.push(save);
    });

    const aliasControls = this.aliases.controls;
    for (let j = 0; j < aliasControls.length; j++) {
      if (aliasControls[j].value.id) {
        const save = this.aliasDataService.updateAlias(aliasControls[j].value, aliasControls[j].value.id);
        saves.push(save);
      } else {
        const alias = aliasControls[j].value;
        alias.contact = { id: value.contact.id };
        alias.worker = { id: value.worker.id };
        const save = this.aliasDataService.createAlias(alias);
        saves.push(save);
      }
    }

    this.busy2 = zip(...saves).toPromise().then(res => {
        subResult.next(true);
        this.reloadUser();
      },
      err => subResult.next(false));

    return subResult;
  }

  pastAddressesAreValid() {
    let valid = true;
    // add current address range
    let dateRanges = [
      {
        fd: new Date(this.form.get("worker.fromdate").value),
        td: new Date(this.form.get("worker.todate").value)
      }
    ];

    // extract date ranges
    const addressControls = this.addresses.controls;
    for (let i = 0; i < addressControls.length; i++) {
      const fromDate = new Date(addressControls[i].value.fromdate);
      const toDate = new Date(addressControls[i].value.todate);
      dateRanges.push({ fd: fromDate, td: toDate });
      if (fromDate > toDate) {
        valid = false;
      }
    }

    if (dateRanges.length < 1) {
      return false;
    }

    dateRanges = dateRanges.sort((a, b) => {
      let res = 0;
      if (a.fd < b.fd) {
        res = -1;
      }
      if (a.fd > b.fd) {
        res = 1;
      }
      return res;
    });

    // The commented out code makes sure there are no gaps in the address dates
    // verify there is no gap between dates
    // let isContinuous = true;
    // for (let i = 1; i < dateRanges.length; i++) {
    //   const element = dateRanges[i];
    //   if (element.fd >= dateRanges[i - 1].td && this.daysBetween(element.fd, dateRanges[i - 1].td) > 1) {
    //     isContinuous = false;
    //     valid = false;
    //     break;
    //   }
    // }

    // if (isContinuous) {
    const daysIn5years = 365 * 5 + 1;
    // verify that the dates form a range >=  5 years
    if (this.daysBetween(dateRanges[0].fd, dateRanges[dateRanges.length - 1].td) < daysIn5years) {
      valid = false;
    }
    // }
    return valid;
  }

  private daysBetween(firstDate: Date, secondDate: Date) {
    const oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    const diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    return diffDays;
  }

  isAscending(fromDate: string, toDate: string) {
    return new Date(toDate) >= new Date(fromDate);
  }

  gotoStep2() {
    if (this.form.valid && this.isBCIDValid() && this.pastAddressesAreValid()) {
      this.router.navigate([`/worker-qualification/spd-consent/${this.workerId}`]);
    } else {
      this.markAsTouched();
    }
  }

  // marking the form as touched makes the validation messages show
  markAsTouched() {
    this.form.markAsTouched();

    const workerControls = ((this.form.get("worker")) as FormGroup).controls;
    for (const c in workerControls) {
      if (typeof (workerControls[c].markAsTouched) === "function") {
        workerControls[c].markAsTouched();
      }
    }

    const contactControls = ((this.form.get("contact")) as FormGroup).controls;
    for (const c in contactControls) {
      if (typeof (contactControls[c].markAsTouched) === "function") {
        contactControls[c].markAsTouched();
      }
    }

    (this.addresses.controls as FormGroup[]).forEach(address => {
      for (const c in address.controls) {
        if (typeof (address.controls[c].markAsTouched) === "function") {
          address.controls[c].markAsTouched();
        }
      }
    });
    (this.aliases.controls as FormGroup[]).forEach(alias => {
      for (const c in alias.controls) {
        if (typeof (alias.controls[c].markAsTouched) === "function") {
          alias.controls[c].markAsTouched();
        }
      }
    });
  }

  isBCIDValid(): boolean {
    const validDriver = !!(this.form.get("contact.primaryIdNumber").value &&
      (this.form.get("contact.primaryIdNumber").value + "").length === 7);
    const validBceid = !!(this.form.get("contact.secondaryIdNumber").value &&
      (this.form.get("contact.secondaryIdNumber").value + "").length === 9);
    return validDriver || validBceid;
  }

  rejectIfNotDigitOrBackSpace(event) {
    const acceptedKeys = [
      "Backspace", "Tab", "End", "Home", "ArrowLeft", "ArrowRight", "Control",
      "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
    ];
    if (acceptedKeys.indexOf(event.key) === -1) {
      event.preventDefault();
    }
  }

  trimValue(control: FormControl) {
    const value = control.value;
    control.setValue("");
    control.setValue(value.trim());
  }
}
