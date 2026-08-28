/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ServicecardUserConfirmationComponent } from './servicecard-user-confirmation.component';

describe('ServicecardUserConfirmationComponent', () => {
  let component: ServicecardUserConfirmationComponent;
  let fixture: ComponentFixture<ServicecardUserConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ServicecardUserConfirmationComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServicecardUserConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
