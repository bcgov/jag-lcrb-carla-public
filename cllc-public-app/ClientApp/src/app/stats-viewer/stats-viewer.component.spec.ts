import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatsViewerComponent } from './stats-viewer.component';

describe('StatsViewerComponent', () => {
  let component: StatsViewerComponent;
  let fixture: ComponentFixture<StatsViewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatsViewerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatsViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
