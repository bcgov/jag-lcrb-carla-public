import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { LicenseeTreeComponent } from './licensee-tree.component';


describe('LicenseeTreeComponent', () => {
  let component: LicenseeTreeComponent;
  let fixture: ComponentFixture<LicenseeTreeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseeTreeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseeTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
