import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResolvedApplicationsComponent } from './resolved-applications.component';

describe('ResolvedApplicationsComponent', () => {
  let component: ResolvedApplicationsComponent;
  let fixture: ComponentFixture<ResolvedApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResolvedApplicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResolvedApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
