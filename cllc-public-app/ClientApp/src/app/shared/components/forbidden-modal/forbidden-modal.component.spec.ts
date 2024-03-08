import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForbiddenModalComponent } from './forbidden-modal.component';

describe('ForbiddenModalComponent', () => {
  let component: ForbiddenModalComponent;
  let fixture: ComponentFixture<ForbiddenModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ForbiddenModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ForbiddenModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
