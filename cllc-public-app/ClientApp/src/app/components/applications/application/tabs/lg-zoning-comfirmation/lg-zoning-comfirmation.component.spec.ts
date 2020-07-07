import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LgZoningComfirmationComponent } from './lg-zoning-comfirmation.component';

describe('LgZoningComfirmationComponent', () => {
  let component: LgZoningComfirmationComponent;
  let fixture: ComponentFixture<LgZoningComfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LgZoningComfirmationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LgZoningComfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
