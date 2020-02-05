import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClientModule } from '@angular/common/http';
import { AccountCompletenessComponent } from './account-completeness.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FileDataService } from '../../services/file-data.service';

const fileDataServiceStub: Partial<FileDataService> = {};

describe('AccountCompletenessComponent', () => {
  let component: AccountCompletenessComponent;
  let fixture: ComponentFixture<AccountCompletenessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes(
          [{ path: '', component: AccountCompletenessComponent }, { path: 'simple', component: AccountCompletenessComponent }]
        ),
        HttpClientTestingModule
      ],
      declarations: [AccountCompletenessComponent],
      providers: [
        {
            provide: FileDataService, useValue: fileDataServiceStub
        }
        ]
    })
    .compileComponents();
  }));

  // TODO - add a test that passes parameters to the component

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountCompletenessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
