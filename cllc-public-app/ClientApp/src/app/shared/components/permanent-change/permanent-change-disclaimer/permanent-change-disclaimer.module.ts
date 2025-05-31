import { NgModule } from '@angular/core';
import { PermanentChangeDisclaimerComponent } from '@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-disclaimer.component';
import { PermanentChangeHolderDetailsComponent } from '@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-holder-details/permanent-change-holder-details.component';

@NgModule({
  declarations: [PermanentChangeDisclaimerComponent, PermanentChangeHolderDetailsComponent],
  exports: [PermanentChangeDisclaimerComponent]
})
export class PermanentChangeDisclaimerModule {}
