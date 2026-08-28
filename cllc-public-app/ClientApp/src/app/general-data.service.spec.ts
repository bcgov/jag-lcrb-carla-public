import { inject, TestBed } from '@angular/core/testing';
import { GeneralDataService } from './general-data.service';

describe('GeneralDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GeneralDataService]
    });
  });

  it('should ...', inject([GeneralDataService], (service: GeneralDataService) => {
    expect(service).toBeTruthy();
  }));
});
