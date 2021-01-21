import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationRequestTermChangeComponent } from './application-request-term-change.component';

describe('ApplicationRequestTermChangeComponent', () => {
  let component: ApplicationRequestTermChangeComponent;
  let fixture: ComponentFixture<ApplicationRequestTermChangeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplicationRequestTermChangeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationRequestTermChangeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
