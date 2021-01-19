import { TestBed, ComponentFixture, ComponentFixtureAutoDetect, waitForAsync } from "@angular/core/testing";
import { BrowserModule } from "@angular/platform-browser";
import { EditShareholdersComponent } from "./shareholders.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";

let component: EditShareholdersComponent;
let fixture: ComponentFixture<EditShareholdersComponent>;

describe("EditShareholders component",
  () => {
    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [EditShareholdersComponent],
        imports: [BrowserModule],
        providers: [
          { provide: ComponentFixtureAutoDetect, useValue: true }
        ],
        schemas: [NO_ERRORS_SCHEMA]
      });
      fixture = TestBed.createComponent(EditShareholdersComponent);
      component = fixture.componentInstance;
    }));

    // it('should do something', async(() => {
    //     expect(true).toEqual(true);
    // }));
  });
