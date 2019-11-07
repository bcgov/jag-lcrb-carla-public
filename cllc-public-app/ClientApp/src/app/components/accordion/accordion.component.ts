import { Component } from '@angular/core';

@Component({
  selector: 'app-accordion',
  templateUrl: './accordion.component.html',
  styleUrls: ['./accordion.component.scss']
})
export class AccordionComponent {
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
