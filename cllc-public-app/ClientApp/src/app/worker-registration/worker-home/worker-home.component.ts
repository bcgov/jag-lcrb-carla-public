import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { PolicyDocumentComponent } from '../../policy-document/policy-document.component';

@Component({
  selector: 'app-worker-home',
  templateUrl: './worker-home.component.html',
  styleUrls: ['./worker-home.component.scss']
})
export class WorkerHomeComponent implements OnInit, AfterViewInit {

  policySlug = 'employee-registration-training';
  @ViewChild('policyDocs') policyDocs: PolicyDocumentComponent;
  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    this.policyDocs.setSlug(this.policySlug);
  }

}
