import { async, ComponentFixture, TestBed } from "@angular/core/testing";

describe("FileUploaderComponent",
  () => {
    let component: FileUploaderComponent;
    let fixture: ComponentFixture<FileUploaderComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [FileUploaderComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(FileUploaderComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
