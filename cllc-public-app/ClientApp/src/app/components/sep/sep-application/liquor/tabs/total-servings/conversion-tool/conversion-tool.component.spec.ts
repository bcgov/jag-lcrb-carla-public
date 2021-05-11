import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConversionToolComponent } from './conversion-tool.component';

describe('ConversionToolComponent', () => {
  let component: ConversionToolComponent;
  let fixture: ComponentFixture<ConversionToolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConversionToolComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConversionToolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
