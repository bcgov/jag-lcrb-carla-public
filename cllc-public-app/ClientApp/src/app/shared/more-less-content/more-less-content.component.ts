import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-more-less-content',
  templateUrl: './more-less-content.component.html',
  styleUrls: ['./more-less-content.component.scss']
})
export class MoreLessContentComponent {
  showMore = false;

  toggleContent() {
    this.showMore = !this.showMore;
  }
}
