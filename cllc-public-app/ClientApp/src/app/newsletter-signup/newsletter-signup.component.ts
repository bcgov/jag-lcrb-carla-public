import { Component, Input, OnInit, ViewContainerRef } from '@angular/core';
import { NewsletterDataService } from "./newsletter-data.service"
import { ToastsManager } from 'ng2-toastr/ng2-toastr';
import { FormGroup, FormControl, FormBuilder, Validators, EmailValidator } from '@angular/forms';

@Component({
    selector: 'app-newsletter-signup',
    templateUrl: './newsletter-signup.component.html',
    styleUrls: ['./newsletter-signup.component.scss']
})
/** newsletter-signup component*/
export class NewsletterSignupComponent implements OnInit {
  public newsletterSignupForm: any;
  @Input('slug') slug: string;
  public description: string;
  public title: string;
  public email: string;

    /** newsletter-signup ctor */
  constructor(private newsletterDataService: NewsletterDataService, public toastr: ToastsManager, vcr: ViewContainerRef) {
    this.toastr.setRootViewContainerRef(vcr);    
  }

  ngOnInit(): void {
    
      if (this.slug != null) {
        this.newsletterDataService.getNewsletter(this.slug)
          .then((newsletter) => {
            this.description = newsletter.description;
            this.title = newsletter.title;            
          });
      }
    }

  signup() {    
      // subscribe to the newsletter.
      this.newsletterDataService.signup(this.slug, this.email).then((results) => {
        this.toastr.success('Thanks for signing up!', 'Success!');
      });
    }
}
