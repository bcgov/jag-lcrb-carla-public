import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RelatedJobnumberPickerComponent } from './related-jobnumber-picker.component';

describe('RelatedJobnumberPickerComponent', () => {
  let component: RelatedJobnumberPickerComponent;
  let fixture: ComponentFixture<RelatedJobnumberPickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RelatedJobnumberPickerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RelatedJobnumberPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
