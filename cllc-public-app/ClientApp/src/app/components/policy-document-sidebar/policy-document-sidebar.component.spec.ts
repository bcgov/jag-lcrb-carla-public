import { TestBed, ComponentFixture, ComponentFixtureAutoDetect, waitForAsync } from "@angular/core/testing";
import { BrowserModule } from "@angular/platform-browser";
import { PolicyDocumentSidebarComponent } from "./policy-document-sidebar.component";
import { PolicyDocumentDataService } from "@services/policy-document-data.service";

let component: PolicyDocumentSidebarComponent;
let fixture: ComponentFixture<PolicyDocumentSidebarComponent>;

describe("PolicyDocumentSidebar component",
  () => {
    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [PolicyDocumentSidebarComponent],
        imports: [BrowserModule],
        providers: [
          { provide: ComponentFixtureAutoDetect, useValue: true },
          { provide: PolicyDocumentDataService, useValue: {} },
        ]
      });
      fixture = TestBed.createComponent(PolicyDocumentSidebarComponent);
      component = fixture.componentInstance;
    }));

    it("should do something",
      waitForAsync(() => {
        expect(true).toEqual(true);
      }));
  });
