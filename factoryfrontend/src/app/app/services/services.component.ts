import {Component, Injectable, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {Box} from "../../../models";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root',
})
export class ServicesComponent  {


  constructor(private http: HttpClient) {}

  private selectedBoxSubject = new BehaviorSubject<Box | null>(null);
  selectedBox$ = this.selectedBoxSubject.asObservable();

  setSelectedBox(box: Box | null) {
    this.selectedBoxSubject.next(box);
  }
}

