import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FeedbackComponent } from "./feedback.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";

describe("FeedbackComponent", () => {

  let fixture: ComponentFixture<FeedbackComponent>;
  let component: FeedbackComponent;
  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
      ],
      declarations: [FeedbackComponent]
    });

    fixture = TestBed.createComponent(FeedbackComponent);
    component = fixture.componentInstance;

  });

  it("should be able to create component instance", () => {
    expect(component).toBeDefined();
  });
  
});
