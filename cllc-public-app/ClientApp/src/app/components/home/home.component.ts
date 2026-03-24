import { Component, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { Subscription } from "rxjs";
import { faExclamationCircle } from "@fortawesome/free-solid-svg-icons"

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.scss"],
})
export class HomeComponent implements OnInit {
  window = window;
  busy: Subscription;
  faExclamationCircle = faExclamationCircle;

  constructor(private titleService: Title) {}

  ngOnInit() {
    this.titleService.setTitle("Home - Liquor and Cannabis Regulation Branch");
  }
}
