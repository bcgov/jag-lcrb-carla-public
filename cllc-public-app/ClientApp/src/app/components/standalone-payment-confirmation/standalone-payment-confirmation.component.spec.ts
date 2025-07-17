import { NO_ERRORS_SCHEMA } from "@angular/core";
import { StandalonePaymentConfirmationComponent } from "./standalone-payment-confirmation.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";

describe("StandalonePaymentConfirmationComponent", () => {

  let fixture: ComponentFixture<StandalonePaymentConfirmationComponent>;
  let component: StandalonePaymentConfirmationComponent;
  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
      ],
      declarations: [StandalonePaymentConfirmationComponent]
    });

    fixture = TestBed.createComponent(StandalonePaymentConfirmationComponent);
    component = fixture.componentInstance;

  });

  it("should be able to create component instance", () => {
    expect(component).toBeDefined();
  });
  
});
