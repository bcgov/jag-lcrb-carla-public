
import {filter} from 'rxjs/operators';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';


@Component({
  selector: 'breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {
  public breadcrumbs: Array<{}> = [];
  public visible = false;

  constructor (
    private router:Router,
    private route:ActivatedRoute
  ) { }

  ngOnInit() {
    const ROUTE_DATA_BREADCRUMB: string = 'breadcrumb';
    const PRIMARY_OUTLET: string = 'primary';

    function resolveBreadcrumbs(route, urlPrefix : string, prevName : string) {
      let ret = [];
      let children = route.children;
      if(children) {
        children.forEach(child => {

          // Verify this is the primary route
          if (child.outlet !== PRIMARY_OUTLET) {
            return;
          }

          //get the route's URL segment
          let routeURL: string = urlPrefix + child.snapshot.url
              .map(segment => segment.path)
              .join('/');

          // Verify the custom data property "breadcrumb" is specified on the route
          if (child.snapshot.data.hasOwnProperty(ROUTE_DATA_BREADCRUMB)) {
            let bcName = child.snapshot.data[ROUTE_DATA_BREADCRUMB];
            if(bcName !== null && bcName !== '' && bcName !== prevName) {
              ret.push({
                label: child.snapshot.data[ROUTE_DATA_BREADCRUMB],
                url: routeURL
              });
            }
            prevName = bcName;
          }

          ret = ret.concat(resolveBreadcrumbs(child, routeURL + '/', prevName));
        });
      }
      return ret;
    }

    this.router.events.pipe(
      filter( event => event instanceof NavigationEnd))
      .subscribe( event => {
        this.breadcrumbs = resolveBreadcrumbs(this.route.root, '', '');
        this.visible = this.breadcrumbs.length > 0;
    })
  }

}
