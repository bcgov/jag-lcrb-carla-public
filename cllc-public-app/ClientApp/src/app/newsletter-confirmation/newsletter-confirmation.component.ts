import { Component, Input } from '@angular/core';
import { NewsletterDataService } from "../services/newsletter-data.service"

@Component({
    selector: 'app-newsletter-confirmation',
    templateUrl: './newsletter-confirmation.component.html',
    styleUrls: ['./newsletter-confirmation.component.scss']
})
/** newsletter-confirmation component*/
export class NewsletterConfirmationComponent {
  @Input('slug') slug: string;
  @Input('code') code: string;

  public description: string;
  public title: string;
  public email: string;
  public validEmail: any;

    /** newsletter-confirmation ctor */
  constructor(private newsletterDataService: NewsletterDataService) {

  }

  ngOnInit(): void {

    if (this.slug != null) {
      // validate the code.
      this.newsletterDataService.verifyCode(this.slug, this.code)
        .then((verificationResult) => {
          alert(verificationResult); 
        });

      this.newsletterDataService.getNewsletter(this.slug)
        .then((newsletter) => {
          this.description = newsletter.description;
          this.title = newsletter.title;
        });
    }


  }
}
