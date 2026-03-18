import { Component, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { Subscription } from "rxjs";
import { faExclamationCircle } from "@fortawesome/free-solid-svg-icons"
import { NoticeBoxService, NoticeBox } from "@services/notice-box.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.scss"],
})
export class HomeComponent implements OnInit {
  window = window;
  busy: Subscription;
  faExclamationCircle = faExclamationCircle;
  public noticeBox: NoticeBox;
  public showNoticeBox: boolean = false;

  constructor(private titleService: Title, private noticeBoxService: NoticeBoxService) {}

  ngOnInit() {
    this.titleService.setTitle("Home - Liquor and Cannabis Regulation Branch");
    this.noticeBoxService.getNoticeBox().subscribe((noticeBox: NoticeBox) => {
      this.noticeBox = noticeBox;
      this.showNoticeBox = noticeBox.noticeBoxEnabled && noticeBox.noticeBoxText?.length > 0;
    });
  }
}
