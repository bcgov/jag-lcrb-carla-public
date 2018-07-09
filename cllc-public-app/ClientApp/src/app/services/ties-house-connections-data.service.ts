import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";

@Injectable()
export class TiedHouseConnectionsDataService {
   constructor(private http: Http) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getTiedHouse(id: string) {

    let apiPath = `api/tiedhouseconnections/${id}`;
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    // call API
    return this.http.get(apiPath, {headers: headers});
  }


  /**
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createTiedHouse(data: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    return this.http.post("api/tiedhouseconnections/", data, { headers: headers });
  }

  /**
   * update a  legal entity in Dynamics
   * @param data - legal entity data
   */
  updateTiedHouse(data: any, id: string) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    return this.http.put(`api/tiedhouseconnections/${id}`, data, { headers: headers });
  }

  /**
   * delete a  legal entity in Dynamics
   * @param data - legal entity data
   */
  deleteTiedHouse(id: string) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    return this.http.post(`api/tiedhouseconnections/${id}/delete`,{}, { headers: headers });
  }




  /**
   * Handle error
   * @param error
   */
  private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || "";
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
