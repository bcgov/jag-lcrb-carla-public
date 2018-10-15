import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { PolicyDocumentComponent } from '../../policy-document/policy-document.component';

@Component({
  selector: 'app-worker-information',
  templateUrl: './worker-information.component.html',
  styleUrls: ['./worker-information.component.scss']
})
export class WorkerInformationComponent implements OnInit, AfterViewInit {

  policySlug = 'worker-qualification-training';
  @ViewChild('policyDocs') policyDocs: PolicyDocumentComponent;
  constructor() { }

  ngOnInit() {
  }

  ngAfterViewInit(): void {
    this.policyDocs.setSlug(this.policySlug);
  }

}
