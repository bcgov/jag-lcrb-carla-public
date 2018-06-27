import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeyPersonnelComponent } from './key-personnel.component';

describe('KeyPersonelComponent', () => {
  let component: KeyPersonnelComponent;
  let fixture: ComponentFixture<KeyPersonnelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KeyPersonnelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeyPersonnelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
