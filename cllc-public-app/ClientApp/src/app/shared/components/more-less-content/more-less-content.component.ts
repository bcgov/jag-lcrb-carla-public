import { AfterViewInit, Component, ViewChild } from "@angular/core";

/* If the content height is greater that this const, the 
* more or less button will show.
* Make sure to keep style sheet in sync when changing (.less { max-height})
*/
const CONTENT_HEIGHT_THRESHOLD = 200;

@Component({
  selector: "app-more-less-content",
  templateUrl: "./more-less-content.component.html",
  styleUrls: ["./more-less-content.component.scss"]
})
export class MoreLessContentComponent implements AfterViewInit {
  showMore = false;
  CONTENT_HEIGHT_THRESHOLD = CONTENT_HEIGHT_THRESHOLD;

  @ViewChild('content') contentDiv; 
  enableMoreLessToggle: boolean;

  ngAfterViewInit(){
    if(this.contentDiv.nativeElement.offsetHeight > CONTENT_HEIGHT_THRESHOLD){
      this.enableMoreLessToggle = true;
    }
  }

  toggleContent() {
    this.showMore = !this.showMore;
  }
}
