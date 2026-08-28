import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FileDataService } from '../../services/file-data.service';
import { AccountCompletenessComponent } from './account-completeness.component';

const fileDataServiceStub: Partial<FileDataService> = {};

describe('AccountCompletenessComponent', () => {
  let component: AccountCompletenessComponent;
  let fixture: ComponentFixture<AccountCompletenessComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: '', component: AccountCompletenessComponent },
          { path: 'simple', component: AccountCompletenessComponent }
        ]),
        HttpClientTestingModule
      ],
      declarations: [AccountCompletenessComponent],
      providers: [
        {
          provide: FileDataService,
          useValue: fileDataServiceStub
        }
      ]
    }).compileComponents();
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
