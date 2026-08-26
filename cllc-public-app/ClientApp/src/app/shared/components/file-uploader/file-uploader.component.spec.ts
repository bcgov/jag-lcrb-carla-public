import { HttpClient } from '@angular/common/http';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ApplicationDataService } from '@services/application-data.service';
import { of } from 'rxjs';
import { FileUploaderComponent } from './file-uploader.component';

const applicationDataServiceStub: Partial<ApplicationDataService> = {};
const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj('HttpClient', ['get']);

describe('FileUploaderComponent', () => {
  let component: FileUploaderComponent;
  let fixture: ComponentFixture<FileUploaderComponent>;

  beforeEach(waitForAsync(() => {
    httpClientSpy.get.and.returnValue(of([]));
    TestBed.configureTestingModule({
      declarations: [FileUploaderComponent],
      providers: [
        { provide: HttpClient, useValue: httpClientSpy },
        { provide: ApplicationDataService, useValue: applicationDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileUploaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
