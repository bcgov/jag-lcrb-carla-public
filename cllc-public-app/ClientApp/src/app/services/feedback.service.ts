import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  apiPath = 'api/feedback';

  constructor(private http: HttpClient) {}

  saveFeedback(query: string): Observable<any> {
    const feedback = { feedback: query };
    return this.http.post<any>(`${this.apiPath}/save-feedback`, feedback);
  }
}
