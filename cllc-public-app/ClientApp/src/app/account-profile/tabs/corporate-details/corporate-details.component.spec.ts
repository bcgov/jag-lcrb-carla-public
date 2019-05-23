import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateDetailsComponent } from './corporate-details.component';

describe('CorporateDetailsComponent', () => {
  let component: CorporateDetailsComponent;
  let fixture: ComponentFixture<CorporateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CorporateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CorporateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
