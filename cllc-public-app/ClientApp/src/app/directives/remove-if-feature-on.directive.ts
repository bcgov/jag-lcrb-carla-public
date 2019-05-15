import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { FeatureFlagService } from '../services/feature-flag.service';


@Directive({
    selector: '[appRemoveIfFeatureOn]'
})
export class AppRemoveIfFeatureOnDirective implements OnInit {
    @Input('appRemoveIfFeatureOn') featureName: string;

    constructor(private el: ElementRef,
        private featureFlagService: FeatureFlagService) {
    }

    ngOnInit() {
        if (this.featureFlagService.featureOn(this.featureName)) {
            this.el.nativeElement.parentNode.removeChild(this.el.nativeElement);
        }
    }
}
