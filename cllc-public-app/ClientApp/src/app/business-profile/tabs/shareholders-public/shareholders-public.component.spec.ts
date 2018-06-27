import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShareholdersPublicComponent } from './shareholders-public.component';

describe('ShareholdersPublicComponent', () => {
  let component: ShareholdersPublicComponent;
  let fixture: ComponentFixture<ShareholdersPublicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShareholdersPublicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShareholdersPublicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
