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
  panel1OpenState: boolean = false;
  panel2OpenState: boolean = false;
  panel3OpenState: boolean = false;
  panel4OpenState: boolean = false;
  panel5OpenState: boolean = false;
  panel6OpenState: boolean = false;
  panel7OpenState: boolean = false;
  panel8OpenState: boolean = false;
  panel9OpenState: boolean = false;
  panel10OpenState: boolean = false;
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
