import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LegalEntityQuestionsComponent } from '@shared/components/legal-entity-review/legal-entity-questions/legal-entity-questions.component';

describe('LegalEntityQuestionsComponent', () => {
  let component: LegalEntityQuestionsComponent;
  let fixture: ComponentFixture<LegalEntityQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LegalEntityQuestionsComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LegalEntityQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
