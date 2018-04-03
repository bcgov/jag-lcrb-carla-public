import { Component, Input, OnInit, ViewContainerRef } from '@angular/core';
import { NewsletterDataService } from "./newsletter-data.service"
import { ToastsManager } from 'ng2-toastr/ng2-toastr';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

@Component({
    selector: 'app-newsletter-signup',
    templateUrl: './newsletter-signup.component.html',
    styleUrls: ['./newsletter-signup.component.scss']
})
/** newsletter-signup component*/
export class NewsletterSignupComponent implements OnInit {
  @Input('slug') slug: string;
  private description: string;
  private title: string;
  public signupForm: FormGroup;
    /** newsletter-signup ctor */
  constructor(private newsletterDataService: NewsletterDataService, public toastr: ToastsManager, vcr: ViewContainerRef, private _fb: FormBuilder) {
    this.toastr.setRootViewContainerRef(vcr);

    
  }

  ngOnInit(): void {

    this.signupForm = this._fb.group({
      newsletterSignupEmail: ['', [<any>Validators.required, <any>Validators.minLength(5)]]      
    });

      if (this.slug != null) {
        this.newsletterDataService.getNewsletter(this.slug)
          .then((newsletter) => {
            this.description = newsletter.description;
            this.title = newsletter.title;            
          });
      }
    }

  signup() {
    var email = this.signupForm.get("newsletterSignupEmail").value;
      alert(email);
      // subscribe to the newsletter.
      this.newsletterDataService.signup(this.slug, email).then((results) => {
        this.toastr.success('Thanks for signing up!', 'Success!');
      });
    }
}
