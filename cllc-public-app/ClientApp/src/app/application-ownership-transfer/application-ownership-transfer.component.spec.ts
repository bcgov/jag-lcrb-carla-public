import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationOwnershipTransferComponent } from './application-ownership-transfer.component';

describe('ApplicationOwnershipTransferComponent', () => {
  let component: ApplicationOwnershipTransferComponent;
  let fixture: ComponentFixture<ApplicationOwnershipTransferComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationOwnershipTransferComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationOwnershipTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
