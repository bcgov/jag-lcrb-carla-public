import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectionToProducersComponent } from './connection-to-producers.component';

describe('ConnectionToProducersComponent', () => {
  let component: ConnectionToProducersComponent;
  let fixture: ComponentFixture<ConnectionToProducersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConnectionToProducersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectionToProducersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
