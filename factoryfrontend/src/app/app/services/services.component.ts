import { Component, Injectable, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { Box, ResponseDto } from '../../../models';
import { environment } from '../../../environments/environment';
import { State } from 'src/state';
import { ToastController } from '@ionic/angular';

@Injectable({
  providedIn: 'root',
})
export class ServicesComponent {
  constructor(
    private http: HttpClient,
    public state: State,
    private toastController: ToastController
  ) {}

  private selectedBoxSubject = new BehaviorSubject<Box | null>(null);
  selectedBox$ = this.selectedBoxSubject.asObservable();

  setSelectedBox(box: Box | null) {
    this.selectedBoxSubject.next(box);
  }

  async getBoxes(): Promise<ResponseDto<Box[]>> {
    const res: any = await firstValueFrom(
      this.http.get<ResponseDto<Box[]>>(environment.baseUrl + '/catalog/boxes')
    );

    this.state.boxes = res.responseData;
    return res;
  }

  async getBoxById(id: number): Promise<Box> {
    try {
      const res: any = await firstValueFrom(
        this.http.get<ResponseDto<Box[]>>(
          environment.baseUrl + '/catalog/boxes/' + id
        )
      );

      this.state.currentBox = res.responseData;
      return res.responseData;

    } catch (error) {
      const toast = await this.toastController.create({
        message: 'Error fetching boxes. Please try again later.',
        duration: 2000,
      });
      toast.present();
    }
  }

  // createBox(data: any): Observable<any> {
  //   const url = environment.baseUrl + '/catalog/boxes';
  //   return this.http.post<any>(url, data);
  // }

  async updateBoxById(id: number, data: any) {
    try {
      const res: any = await firstValueFrom(this.http.put<ResponseDto<Box>>(environment.baseUrl+ '/catalog/boxes/' + id, data));

      this.state.currentBox = res.responseData;

      const toast = await this.toastController.create({
        message: res.messageToClient,
        duration: 2000,
      });
      toast.present();

      await this.getBoxes();

    } catch (error) {
      const toast = await this.toastController.create({
        message: 'Error fetching boxes. Please try again later.',
        duration: 2000,
      });
      toast.present();
    }
  }

  // deleteBox(id: number): Observable<any> {
  //   const url = environment.baseUrl + '/catalog/boxes';
  //   return this.http.delete<any>(url);
  // }
}
