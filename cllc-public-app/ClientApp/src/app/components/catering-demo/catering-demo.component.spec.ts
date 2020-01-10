import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CateringDemoComponent } from './catering-demo.component';

describe('CateringDemoComponent', () => {
  let component: CateringDemoComponent;
  let fixture: ComponentFixture<CateringDemoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CateringDemoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CateringDemoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
