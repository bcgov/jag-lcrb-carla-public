import { EventEmitter } from '@angular/core';
import { fakeAsync, flushMicrotasks } from '@angular/core/testing';
import { FormGroup } from '@angular/forms';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { of, Subject, throwError } from 'rxjs';
import { ApplicationRenewalComponent } from './application-renewal/application-renewal.component';
import { ApplicationComponent } from './application/application.component';

describe('Application refresh after saving', () => {
  let application: Application;
  let service: jasmine.SpyObj<ApplicationDataService>;
  let snackBar: { open: jasmine.Spy };
  let store: { dispatch: jasmine.Spy };

  beforeEach(() => {
    application = {
      id: '1',
      jobNumber: 'old',
      applicationType: { isFree: false }
    } as Application;
    service = jasmine.createSpyObj<ApplicationDataService>('ApplicationDataService', [
      'updateApplication',
      'getApplicationById'
    ]);
    service.updateApplication.and.callFake((data) => of(data));
    snackBar = { open: jasmine.createSpy('open') };
    store = { dispatch: jasmine.createSpy('dispatch') };
  });

  describe('application submission', () => {
    let component: ApplicationComponent;

    beforeEach(() => {
      component = Object.create(ApplicationComponent.prototype);
      Object.assign(component, {
        application,
        applicationId: application.id,
        applicationDataService: service,
        snackBar,
        store,
        componentActive: true,
        savesInProgress: 0,
        form: new FormGroup({}),
        saveComplete: new EventEmitter<boolean>()
      });
      spyOn(component, 'isValid').and.returnValue(true);
      spyOn(component, 'prepareTiedHouseSaveRequest').and.returnValue(of(null));
      spyOn(component, 'normalizeFormData').and.returnValue({
        description2: '',
        serviceAreas: [],
        outsideAreas: [],
        capacityArea: [],
        indigenousNationId: null,
        policeJurisdictionId: null
      });
    });

    it('waits for the refresh before sending the completion PUT', () => {
      const refresh = new Subject<Application>();
      const refreshed = { ...application, jobNumber: 'updated' };
      service.getApplicationById.and.returnValues(refresh, of(refreshed));

      component.submit_application();

      expect(service.updateApplication).toHaveBeenCalledTimes(1);
      refresh.next(refreshed);
      refresh.complete();
      expect(service.updateApplication).toHaveBeenCalledTimes(2);
      expect(service.updateApplication.calls.mostRecent().args[0].jobNumber).toBe('updated');
      expect(service.updateApplication.calls.mostRecent().args[0].isApplicationComplete).toBe('Yes');
      expect(store.dispatch).toHaveBeenCalled();
    });

    it('does not send the completion PUT if the refresh fails', () => {
      const refresh = new Subject<Application>();
      service.getApplicationById.and.returnValue(refresh);
      spyOn(component.saveComplete, 'emit');

      component.submit_application();
      refresh.error(new Error('Refresh failed'));

      expect(service.updateApplication).toHaveBeenCalledTimes(1);
      expect(component.saveComplete.emit).not.toHaveBeenCalled();
      expect(component.submitApplicationInProgress).toBeFalse();
      expect(component.savesInProgress).toBe(0);
      expect(snackBar.open).toHaveBeenCalled();
    });

    it('tracks overlapping saves until each refresh completes or is cancelled', () => {
      const firstRefresh = new Subject<Application>();
      const secondRefresh = new Subject<Application>();
      service.getApplicationById.and.returnValues(firstRefresh, secondRefresh);

      const firstSave = component.save().subscribe();
      const secondSave = component.save().subscribe();
      expect(component.savesInProgress).toBe(2);

      firstRefresh.next(application);
      firstRefresh.complete();
      expect(component.savesInProgress).toBe(1);

      secondSave.unsubscribe();
      expect(component.savesInProgress).toBe(0);
      expect(firstSave.closed).toBeTrue();
    });

    it('keeps submission in progress until navigation after a free submission', fakeAsync(() => {
      application.applicationType.isFree = true;
      const submitted = { ...application, applicationStatus: 'Submitted' };
      let finishNavigation: (value: boolean) => void;
      const navigation = new Promise<boolean>((resolve) => (finishNavigation = resolve));
      const router = { navigateByUrl: jasmine.createSpy('navigateByUrl').and.returnValue(navigation) };
      Object.assign(component, { router });
      service.getApplicationById.and.returnValues(of(application), of(submitted));

      component.submit_application();

      expect(component.application.applicationStatus).toBe('Submitted');
      expect(component.savesInProgress).toBe(0);
      expect(component.submitApplicationInProgress).toBeTrue();
      expect(component.saveOnDeactivate).toBeFalse();
      expect(router.navigateByUrl).toHaveBeenCalledWith('/dashboard');
      expect(snackBar.open).toHaveBeenCalledWith('Application submitted', 'Success', jasmine.any(Object));

      finishNavigation(false);
      flushMicrotasks();
      expect(component.submitApplicationInProgress).toBeFalse();
      expect(snackBar.open).toHaveBeenCalledWith('Error returning to the dashboard', 'Fail', jasmine.any(Object));
    }));

    it('uses the refreshed invoice to decide whether to proceed to payment', () => {
      const refresh = new Subject<Application>();
      const payment = jasmine.createSpyObj<PaymentDataService>('PaymentDataService', ['getPaymentSubmissionUrl']);
      payment.getPaymentSubmissionUrl.and.returnValue(throwError(() => 'Payment already made'));
      Object.assign(component, { paymentDataService: payment });
      service.getApplicationById.and.returnValue(refresh);

      component.submit_application();
      refresh.next({ ...application, invoiceId: 'invoice' });
      refresh.complete();

      expect(service.updateApplication).toHaveBeenCalledTimes(1);
      expect(payment.getPaymentSubmissionUrl).toHaveBeenCalledWith(application.id);
    });

    it('waits for the refresh before navigating after LG/IN submission', () => {
      const refresh = new Subject<Application>();
      const router = { navigateByUrl: jasmine.createSpy('navigateByUrl') };
      Object.assign(component, { router });
      service.getApplicationById.and.returnValue(refresh);

      component.submitForLGINApproval();

      expect(router.navigateByUrl).not.toHaveBeenCalled();
      refresh.next(application);
      refresh.complete();
      expect(router.navigateByUrl).toHaveBeenCalledWith('/dashboard');
    });

    it('does not navigate after an incomplete application refresh fails', () => {
      const refresh = new Subject<Application>();
      const router = { navigateByUrl: jasmine.createSpy('navigateByUrl') };
      Object.assign(component, { router });
      service.getApplicationById.and.returnValue(refresh);

      component.resolveIncompleteness();
      refresh.error(new Error('Refresh failed'));

      expect(router.navigateByUrl).not.toHaveBeenCalled();
      expect(component.disableIncomplete).toBeFalse();
      expect(snackBar.open).toHaveBeenCalled();
    });
  });

  describe('renewal submission', () => {
    let component: ApplicationRenewalComponent;
    let payment: jasmine.SpyObj<PaymentDataService>;

    beforeEach(() => {
      payment = jasmine.createSpyObj<PaymentDataService>('PaymentDataService', ['getPaymentSubmissionUrl']);
      payment.getPaymentSubmissionUrl.and.returnValue(throwError(() => 'Payment already made'));
      component = Object.create(ApplicationRenewalComponent.prototype);
      Object.assign(component, {
        application,
        applicationId: application.id,
        applicationDataService: service,
        paymentDataService: payment,
        snackBar,
        store,
        componentActive: true,
        form: new FormGroup({}),
        savedFormData: null
      });
      spyOn(component, 'isValid').and.returnValue(true);
    });

    it('waits for the refresh before proceeding to payment', () => {
      const refresh = new Subject<Application>();
      const refreshed = { ...application, jobNumber: 'updated' };
      service.getApplicationById.and.returnValue(refresh);

      component.submitApplication();

      expect(payment.getPaymentSubmissionUrl).not.toHaveBeenCalled();
      refresh.next(refreshed);
      refresh.complete();
      expect(component.application).toBe(refreshed);
      expect(store.dispatch).toHaveBeenCalled();
      expect(payment.getPaymentSubmissionUrl).toHaveBeenCalled();
    });

    it('reports a refresh failure without proceeding to payment', () => {
      const refresh = new Subject<Application>();
      service.getApplicationById.and.returnValue(refresh);

      component.submitApplication();
      refresh.error(new Error('Refresh failed'));

      expect(payment.getPaymentSubmissionUrl).not.toHaveBeenCalled();
      expect(component.submitReqInProgress).toBeFalse();
      expect(snackBar.open).toHaveBeenCalled();
    });
  });
});
