import { TestBed, ComponentFixture, ComponentFixtureAutoDetect, waitForAsync } from "@angular/core/testing";
import { BrowserModule } from "@angular/platform-browser";
import { StatusBadgeComponent } from "./status-badge.component";

let component: StatusBadgeComponent;
let fixture: ComponentFixture<StatusBadgeComponent>;

describe("statusbadge component",
  () => {
    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [StatusBadgeComponent],
        imports: [BrowserModule],
        providers: [
          { provide: ComponentFixtureAutoDetect, useValue: true }
        ]
      });
      fixture = TestBed.createComponent(StatusBadgeComponent);
      component = fixture.componentInstance;
    }));

    it("should do something",
      waitForAsync(() => {
        expect(true).toEqual(true);
      }));
  });
