import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeExternalTransferShares } from '@shared/components/permanent-change/permanent-change-external-transfer-shares/permanent-change-external-transfer-shares.component';

describe('PermanentChangeExternalTransferShares', () => {
  let component: PermanentChangeExternalTransferShares;
  let fixture: ComponentFixture<PermanentChangeExternalTransferShares>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeExternalTransferShares]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeExternalTransferShares);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
