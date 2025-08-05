import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PermanentChangeAdditionReceiverExecutorComponent } from '@shared/components/permanent-change/permanent-change-addition-receiver-executor/permanent-change-addition-receiver-executor.component';

describe('PermanentChangeAdditionReceiverExecutorComponent', () => {
  let component: PermanentChangeAdditionReceiverExecutorComponent;
  let fixture: ComponentFixture<PermanentChangeAdditionReceiverExecutorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PermanentChangeAdditionReceiverExecutorComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PermanentChangeAdditionReceiverExecutorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
