import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { KeyPersonelComponent } from './key-personel.component';

describe('KeyPersonelComponent', () => {
  let component: KeyPersonelComponent;
  let fixture: ComponentFixture<KeyPersonelComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KeyPersonelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KeyPersonelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
