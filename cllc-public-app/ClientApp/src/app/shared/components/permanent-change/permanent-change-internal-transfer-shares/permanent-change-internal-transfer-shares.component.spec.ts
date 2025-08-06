import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeInternalTransferShares } from '@shared/components/permanent-change/permanent-change-internal-transfer-shares/permanent-change-internal-transfer-shares.component';

describe('PermanentChangeInternalTransferShares', () => {
  let component: PermanentChangeInternalTransferShares;
  let fixture: ComponentFixture<PermanentChangeInternalTransferShares>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeInternalTransferShares]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeInternalTransferShares);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
