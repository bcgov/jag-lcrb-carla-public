import { Component, OnInit, Input } from '@angular/core';
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
      this.isExpandAll = false;

      // inform angular to run a change detection cycle
      setTimeout(() => this.isExpandAll = true, 0);
  }

  collapseAll() {

      this.isExpandAll = true;

      // inform angular to run a change detection cycle
      setTimeout(() => this.isExpandAll = false, 0);
  }

}
