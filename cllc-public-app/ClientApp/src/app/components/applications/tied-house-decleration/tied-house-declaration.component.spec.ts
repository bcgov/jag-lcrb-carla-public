import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiedHouseDeclarationComponent } from './tied-house-declaration.component';

describe('TiedHouseDeclarationComponent', () => {
  let component: TiedHouseDeclarationComponent;
  let fixture: ComponentFixture<TiedHouseDeclarationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TiedHouseDeclarationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TiedHouseDeclarationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
