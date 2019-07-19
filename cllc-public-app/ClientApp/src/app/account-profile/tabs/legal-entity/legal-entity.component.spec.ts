import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LegalEntityComponent } from './legal-entity.component';

describe('LegalEntityComponent', () => {
  let component: LegalEntityComponent;
  let fixture: ComponentFixture<LegalEntityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LegalEntityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
