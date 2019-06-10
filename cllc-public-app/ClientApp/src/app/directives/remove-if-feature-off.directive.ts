import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { FeatureFlagService } from '../services/feature-flag.service';

@Directive({
    selector: '[appRemoveIfFeatureOff]'
})
export class AppRemoveIfFeatureOffDirective implements OnInit {
    @Input('appRemoveIfFeatureOff') featureName: string;

    constructor(private el: ElementRef,
        private featureFlagService: FeatureFlagService) {
    }

    ngOnInit() {
        if (this.featureFlagService.featureOff(this.featureName)) {
            this.el.nativeElement.parentNode.removeChild(this.el.nativeElement);
        }
    }
}
