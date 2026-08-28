/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { TakeHomeEventComponent } from './take-home-event.component';

describe('TakeHomeEventComponent', () => {
  let component: TakeHomeEventComponent;
  let fixture: ComponentFixture<TakeHomeEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TakeHomeEventComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TakeHomeEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
