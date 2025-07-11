import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TiedHouseDeclarationFormComponent } from './tied-house-declaration-form.component';

describe('TiedHouseDeclarationFormComponent', () => {
  let component: TiedHouseDeclarationFormComponent;
  let fixture: ComponentFixture<TiedHouseDeclarationFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TiedHouseDeclarationFormComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TiedHouseDeclarationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
