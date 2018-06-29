import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShareholdersUclLlcComponent } from './shareholders-ucl-llc.component';

describe('ShareholdersUclLlcComponent', () => {
  let component: ShareholdersUclLlcComponent;
  let fixture: ComponentFixture<ShareholdersUclLlcComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShareholdersUclLlcComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShareholdersUclLlcComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
