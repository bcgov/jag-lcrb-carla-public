import { Component } from '@angular/core';
import { Router } from "@angular/router";

@Component({
    selector: 'app-not-found',
    templateUrl: './not-found.component.html',
    styleUrls: ['./not-found.component.scss']
})
/** NotFound component*/
export class NotFoundComponent {
    /** NotFound ctor */
  constructor(private router: Router) {
    this.router.navigateByUrl("/404");

    }
}
