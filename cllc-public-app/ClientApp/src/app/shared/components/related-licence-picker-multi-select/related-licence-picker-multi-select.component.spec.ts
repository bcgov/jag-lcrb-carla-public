import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RelatedLicencePickerComponent } from '../related-licence-picker/related-licence-picker.component';


describe('RelatedLicencePickerComponent', () => {
  let component: RelatedLicencePickerComponent;
  let fixture: ComponentFixture<RelatedLicencePickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RelatedLicencePickerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RelatedLicencePickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
