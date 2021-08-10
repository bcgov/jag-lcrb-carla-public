import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SepClaimComponent } from './sep-claim.component';

describe('SepClaimComponent', () => {
  let component: SepClaimComponent;
  let fixture: ComponentFixture<SepClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SepClaimComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SepClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
