import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-status-badge',
    templateUrl: './status-badge.component.html',
    styleUrls: ['./status-badge.component.scss']
})
/** statusbadge component*/
export class StatusBadgeComponent {
    @Input('status') status: string;
    /** statusbadge ctor */
    constructor() {

    }
}
