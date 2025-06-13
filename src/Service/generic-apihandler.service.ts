import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { environment } from '../enviroments/environment';
import { APIResponseVM } from '../ViewModels/apiresponse-vm';

@Injectable({
  providedIn: 'root'
})
export class GenericApiHandlerService {
  httpOptions;

  constructor(private hClient: HttpClient) {
    this.httpOptions = {
      headers: {
        'Content-Type': 'application/json'
      }
    };
  }


  private handleError(error: HttpErrorResponse): Observable<APIResponseVM>
   {

    if (error.status === 404) {
     return error.error;
    } else if (error.status === 500) {
      return error.error;
    }
    else{
      return error.error;

    }

  }

  // Fetch data from the API
  getAll(APIRoute: string): Observable<APIResponseVM> {
    return this.hClient.get<APIResponseVM>(`${environment.apiUrl}${APIRoute}`, this.httpOptions)
      .pipe(
        retry(2),
        catchError(this.handleError)
      );
  }

  // Fetch data by ID from the API
  getByID(APIRoute: string, id: number): Observable<APIResponseVM> {
    return this.hClient.get<APIResponseVM>(`${environment.apiUrl}${APIRoute}/${id}`, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }
  // Create data in the API
  createData(APIRoute: string, data: any): Observable<APIResponseVM> {
    return this.hClient.post<APIResponseVM>(`${environment.apiUrl}${APIRoute}`, data, this.httpOptions)
     .pipe(
        catchError(this.handleError)
      );
    }
}
