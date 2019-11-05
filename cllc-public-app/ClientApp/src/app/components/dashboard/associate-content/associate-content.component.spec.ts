import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociateContentComponent } from './associate-content.component';

describe('AssociateContentComponent', () => {
  let component: AssociateContentComponent;
  let fixture: ComponentFixture<AssociateContentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssociateContentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssociateContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
