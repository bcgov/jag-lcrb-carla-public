import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShareholdersAndPartnersComponent } from './shareholders-and-partners.component';

describe('ShareholdersAndPartnersComponent', () => {
  let component: ShareholdersAndPartnersComponent;
  let fixture: ComponentFixture<ShareholdersAndPartnersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShareholdersAndPartnersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShareholdersAndPartnersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
