import { Component, OnInit } from '@angular/core';
import { MatButtonModule, MatCheckboxModule } from '@angular/material';
import { MatExpansionModule } from '@angular/material/expansion';

@Component({
  selector: 'app-accordion',
  templateUrl: './accordion.component.html',
  styleUrls: ['./accordion.component.scss']
})
export class AccordionComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  panelOpenState: boolean = false;
  step = 0;

  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.step++;
  }

  prevStep() {
    this.step--;
  }

}
