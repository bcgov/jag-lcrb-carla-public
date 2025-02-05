import { Component, OnInit } from '@angular/core';
import { MaintenanceBannerService, MaintenanceBanner } from '@services/maintenance-banner.service';

@Component({
  selector: 'app-maintenance-banner',
  templateUrl: './maintenance-banner.component.html',
  styleUrls: ['./maintenance-banner.component.scss']
})

export class MaintenanceBannerComponent implements OnInit {
  public banner: MaintenanceBanner;
  public showBanner: boolean = false;

  constructor(private maintenanceBannerService: MaintenanceBannerService) { }

  ngOnInit() {
    this.maintenanceBannerService.getBanner().subscribe((banner: MaintenanceBanner) => {
      this.banner = banner;
      this.showBanner = this.isBannerActive();
    });
  }
  
  private isBannerActive(): boolean {
    try {        
        const isBannerEnabled = this.banner.bannerEnabled === true;
        const bannerMessage = this.banner.bannerText;

        if(!isBannerEnabled || !bannerMessage.length){
            return false;
        }

        // If the start and end date are not set, show the banner if enabled
        if (!this.isValidDate(this.banner.bannerStartDate) || !this.isValidDate(this.banner.bannerEndDate)) {
            return isBannerEnabled && bannerMessage.length > 0;
        }

        const bannerStartDate = new Date(this.banner.bannerStartDate);
        const bannerEndDate = new Date(this.banner.bannerEndDate);

        // If the current date is between the start and end date, show the banner
        const currentDate = new Date();
        const startDate = new Date(bannerStartDate);
        const endDate = new Date(bannerEndDate);

        return isBannerEnabled && bannerMessage.length > 0 && currentDate >= startDate && currentDate <= endDate;

    } catch (error) {
        console.error("Error in showBanner: ", error);
        return false;
    }
    
  }

  private isValidDate(date: string): boolean {
    return date && !isNaN(Date.parse(date));
  }
}