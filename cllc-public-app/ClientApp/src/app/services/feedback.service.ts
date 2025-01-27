import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

 

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  apiPath = "api/feedback";

  constructor(private http: HttpClient) { }

  saveFeedback(query: string): Observable<any> {
    const feedback = {feedback: query};
    return this.http.post<any>(`${this.apiPath}/save-feedback`, feedback);
  }

  

}