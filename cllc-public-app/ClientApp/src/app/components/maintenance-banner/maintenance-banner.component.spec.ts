import { ComponentFixture, TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { MaintenanceBannerComponent } from "./maintenance-banner.component";
import { MaintenanceBanner, MaintenanceBannerService } from "@services/maintenance-banner.service";

fdescribe("MaintenanceBannerComponent", () => {
    let component: MaintenanceBannerComponent;
    let fixture: ComponentFixture<MaintenanceBannerComponent>;
    let maintenanceBannerService: MaintenanceBannerService;
    
    beforeEach(() => {
        TestBed.configureTestingModule({
        declarations: [MaintenanceBannerComponent],
        providers: [
            {
            provide: MaintenanceBannerService,
            useValue: {
                getBanner: () => of({ bannerEnabled: true, bannerText: "Test", bannerStartDate: "2021-01-01", bannerEndDate: "2021-12-31" } as MaintenanceBanner)
            }
            }
        ]
        });
    
        fixture = TestBed.createComponent(MaintenanceBannerComponent);
        component = fixture.componentInstance;
        maintenanceBannerService = TestBed.inject(MaintenanceBannerService);
    });
    
    it("should create", () => {
        expect(component).toBeTruthy();
    });
    
    it("should show banner", () => {
        component.ngOnInit();
        expect(component.showBanner).toBeTruthy();
    });
    
    it("should not show banner if BANNER_ENABLED is set to false", () => {
        spyOn(maintenanceBannerService, "getBanner").and.returnValue(of({ bannerEnabled: false, bannerText: "", bannerStartDate: "", bannerEndDate: "" } as MaintenanceBanner));
        component.ngOnInit();
        expect(component.showBanner).toBeFalsy();
    });

    it("should show banner if BANNER_ENABLED is set to true without any dates", () => { 
        spyOn(maintenanceBannerService, "getBanner").and.returnValue(of({ bannerEnabled: true, bannerText: "Test", bannerStartDate: "", bannerEndDate: "" } as MaintenanceBanner));
        component.ngOnInit();
        expect(component.showBanner).toBeTruthy();
    });
    
    it("should not show banner when start date is in the future", () => {
        spyOn(maintenanceBannerService, "getBanner").and.returnValue(of({ bannerEnabled: true, bannerText: "Test", bannerStartDate: "2050-01-01", bannerEndDate: "2050-12-31" } as MaintenanceBanner));
        component.ngOnInit();
        expect(component.showBanner).toBeFalsy();
    });
    
    it("should not show banner when end date is in the past", () => {
        spyOn(maintenanceBannerService, "getBanner").and.returnValue(of({ bannerEnabled: true, bannerText: "Test", bannerStartDate: "2020-01-01", bannerEndDate: "2020-12-31" } as MaintenanceBanner));
        component.ngOnInit();
        expect(component.showBanner).toBeFalsy();
    });
});