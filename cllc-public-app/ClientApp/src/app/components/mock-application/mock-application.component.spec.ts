import { NO_ERRORS_SCHEMA } from "@angular/core";
import { MockApplicationComponent } from "./mock-application.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";

describe("MockApplicationComponent", () => {

  let fixture: ComponentFixture<MockApplicationComponent>;
  let component: MockApplicationComponent;
  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
      ],
      declarations: [MockApplicationComponent]
    });

    fixture = TestBed.createComponent(MockApplicationComponent);
    component = fixture.componentInstance;

  });

  it("should be able to create component instance", () => {
    expect(component).toBeDefined();
  });
  
});
