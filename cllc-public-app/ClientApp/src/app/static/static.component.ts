import { Component, Injector, Input, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'static-content',
  templateUrl: './static.component.html'
})
export class StaticComponent implements OnInit {
  @Input() href: string;
  content: string;
  loading = true;
  error: string;

  constructor(private http: HttpClient, private injector: Injector) {
    // href will be passed by the injector when instantiated by InsertComponent
    this.href = this.injector.get('href', this.href || null);
  }

  ngOnInit() {
    if (this.href) {
      this.http.get(this.href, {
        params: new HttpParams().set('t', (new Date().getTime()).toString())
      }).subscribe(
        response => { this.content = response.toString(); this.loading = false; },
        err => this.handleLoadError(err));
    }
  }

  handleLoadError(err) {
    this.error = 'Content could not be loaded: ' + err.statusText;
  }

}
