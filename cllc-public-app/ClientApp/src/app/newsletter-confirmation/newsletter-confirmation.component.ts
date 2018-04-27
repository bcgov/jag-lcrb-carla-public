import { Component, Input, OnInit } from '@angular/core';
import { NewsletterDataService } from "../services/newsletter-data.service"
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-newsletter-confirmation',
    templateUrl: './newsletter-confirmation.component.html',
    styleUrls: ['./newsletter-confirmation.component.scss']
})
/** newsletter-confirmation component*/
export class NewsletterConfirmationComponent {

  public slug: string;
  public description: string;
  public title: string;
  public email: string;
  public code: string;
  public verificationResult: string;

    /** newsletter-confirmation ctor */
  constructor(private newsletterDataService: NewsletterDataService, private route: ActivatedRoute,
    private router: Router) {
    this.slug = this.route.snapshot.params["slug"];
    this.route.queryParams.subscribe(params => {
      this.code = params['code'];
    });
  }
  
  ngOnInit(): void {
    if (this.slug != null) {      
      // validate the code.
      console.log('*********** before verifyCode ***************');
      this.newsletterDataService.verifyCode(this.slug, this.code)
        .then((verificationResult) => {
          //alert(verificationResult);
          this.verificationResult = verificationResult;
        });
      console.log('*********** after verifyCode ***************');

      
    }    
  }
}
