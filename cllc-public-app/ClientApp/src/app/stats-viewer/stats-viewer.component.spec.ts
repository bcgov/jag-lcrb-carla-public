import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatsViewerComponent } from './stats-viewer.component';
import { StatsDataService } from '@services/stats-data.service';
import { of } from 'rxjs';

const statsDataServiceStub: Partial<StatsDataService> = {
  getStats: () => of([])
};

describe('StatsViewerComponent', () => {
  let component: StatsViewerComponent;
  let fixture: ComponentFixture<StatsViewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatsViewerComponent ],
      providers: [
        {provide: StatsDataService, useValue: statsDataServiceStub}
      ]
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
