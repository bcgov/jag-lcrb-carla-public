import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { KeyValue } from '../../../node_modules/@angular/common';
import { AbstractControl, ValidatorFn } from '@angular/forms';

@Injectable({
    providedIn: 'root'
})
export class EstablishmentWatchWordsService extends DataService {
    private _forbiddenWords: Array<string> = [];
    private _problematicWords: Array<string> = [];

    apiPath = 'api/establishmentwatchwords';
    constructor(private http: HttpClient) {
        super();
    }

    public getEstablishmentWatchWords(): Observable<KeyValue<string, boolean>[]> {
      return this.http.get<KeyValue<string, boolean>[]>(this.apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
    }

    initialize() {
      this.getEstablishmentWatchWords()
      .subscribe(watchWords => {
        this._forbiddenWords = watchWords['forbidden'];
        this._problematicWords = watchWords['problematic'];
      });
    }

    public forbiddenNameValidator(): ValidatorFn {
      return (control: AbstractControl): {[key: string]: any} | null => {
        const nameWordList = control.value.toLowerCase().split(' ');
        const wordsUnioned = this._forbiddenWords.filter(x => nameWordList.includes(x));
        if (wordsUnioned.length > 0) {
          return {'forbiddenName': {value: control.value}};
        }
        return null;
      };
    }

    public potentiallyProblematicValidator(name: string) {
      const nameWordList = name.toLowerCase().split(' ');
      const wordsUnioned = this._problematicWords.filter(x => nameWordList.includes(x));
      return wordsUnioned.length > 0;
    }
}
