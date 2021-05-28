import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliceSummaryComponent } from './police-summary.component';

describe('PoliceSummaryComponent', () => {
  let component: PoliceSummaryComponent;
  let fixture: ComponentFixture<PoliceSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PoliceSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliceSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
