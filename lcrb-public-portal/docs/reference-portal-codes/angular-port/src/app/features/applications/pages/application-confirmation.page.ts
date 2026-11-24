/** Payment confirmation page; `id` arrives as a route-param input. */
import { ChangeDetectionStrategy, Component, input } from "@angular/core"
import { PaymentReceipt } from "../components/payment-receipt"
import { BCGovFooter } from "@core/layout/footer"
import { BCGovHeader } from "@core/layout/header"

@Component({
  selector: "application-confirmation-page",
  imports: [BCGovHeader, BCGovFooter, PaymentReceipt],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: "display: contents" },
  template: `
    <div class="flex min-h-screen flex-col">
      <bc-gov-header currentPage="/applications" />
      <main id="main-content" class="flex-1 bg-muted/20">
        <payment-receipt [id]="id()" />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class ApplicationConfirmationPage {
  readonly id = input.required<string>()
}
