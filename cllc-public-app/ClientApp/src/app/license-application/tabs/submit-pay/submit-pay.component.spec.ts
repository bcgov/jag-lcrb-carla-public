import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitPayComponent } from './submit-pay.component';

describe('SubmitPayComponent', () => {
  let component: SubmitPayComponent;
  let fixture: ComponentFixture<SubmitPayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SubmitPayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SubmitPayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
