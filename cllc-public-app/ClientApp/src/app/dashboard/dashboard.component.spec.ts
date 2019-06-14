import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardComponent } from './dashboard.component';
import { Component, Input } from '@angular/core';
import { StoreModule } from '@ngrx/store';
import { reducers, metaReducers } from '@app/app-state/reducers/reducers';
import { by, element } from 'protractor';


@Component({selector: 'app-applications-and-licences', template: ''})
class ApplicationsAndLicencesComponent {
  @Input() account: any;
}

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        StoreModule.forRoot(reducers, { metaReducers }),
      ],
      declarations: [ DashboardComponent, ApplicationsAndLicencesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have "Cannabis" in the title', () => {
    const bannerElement: HTMLElement = fixture.nativeElement;
    const header = bannerElement.querySelector('h1');
    expect(header.textContent).toContain('Cannabis');
  });
});
