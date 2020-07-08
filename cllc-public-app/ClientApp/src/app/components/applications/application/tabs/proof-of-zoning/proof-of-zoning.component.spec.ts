import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProofOfZoningComponent } from './proof-of-zoning.component';

describe('ProofOfZoningComponent', () => {
  let component: ProofOfZoningComponent;
  let fixture: ComponentFixture<ProofOfZoningComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProofOfZoningComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProofOfZoningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
