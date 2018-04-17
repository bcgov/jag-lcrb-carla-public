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

  isExpandAll = false;

  showAll() {
    this.isExpandAll = true;
  }

  collapseAll() {
    this.isExpandAll = false;
  }

}
