import {Component, EventEmitter, Inject, OnInit, Output} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {ModalController, ToastController} from "@ionic/angular";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {State} from "../../../state";
import {Box, ResponseDto} from "../../../models";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {ServicesComponent} from "../services/services.component";
import { MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-create-edit-box',
  templateUrl: './create-edit-box.component.html',
  styleUrls: ['./create-edit-box.component.scss'],
})
export class CreateEditBoxComponent  implements OnInit {

  @Output() recordEdited: EventEmitter<void> = new EventEmitter<void>();

  createEditBoxForm = this.fb.group({
    boxName: ['' , Validators.minLength(1)],
    description : ['' ,  Validators.required],
    size : ['' , Validators.required],
    price : ['' , Validators.required],
    imageUrl : ['' , Validators.required],
  })

  constructor(
    public fb: FormBuilder,
    public modalController : ModalController,
    public http: HttpClient,
    public state : State,
    public toastController : ToastController,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public service : ServicesComponent

  ) {
    this.createEditBoxForm = this.fb.group({
      boxName: '',
      description: '',
      size: '',
      price: '',
      imageUrl: '',
    });

  }

  ngOnInit() {
    this.createEditBoxForm.patchValue(this.data)
  }


  async saveForm() {
    try{
      const observable = this.http.post<ResponseDto<Box>>(environment.baseUrl + '/catalog/boxes', this. createEditBoxForm.getRawValue())

      const response = await firstValueFrom(observable);
      this.state.boxes.push(response.responseData !);

      const toast = await this.toastController.create({
        message: 'Box was successfully saved' ,
        duration: 1233,
        color: "success"
      })
      toast.present();
      this.modalController.dismiss();
    }catch (e) {
      if(e instanceof HttpErrorResponse) {
        const toast = await this.toastController.create({
          message: e.error.messageToClient,
          color: "danger"
        });
        toast.present();
      }
    }

  }

  async updateBox(boxId: number) {
    try {

      const observable = this.http.put<ResponseDto<Box>>(
          `${environment.baseUrl}/catalog/boxes/${boxId}`,
          this.createEditBoxForm.value
      );

      const response = await firstValueFrom(observable);

      if (response.responseData) {
        const index = this.state.boxes.findIndex(box => box.boxId === boxId);
        if (index !== -1) {
          this.state.boxes[index] = response.responseData;
        }
      }

      const toast = await this.toastController.create({
        message: 'Box was successfully updated',
        duration: 1233,
        color: 'success'
      });
      toast.present();
      this.modalController.dismiss();

    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        const toast = await this.toastController.create({
          message: e.error.messageToClient,
          color: 'danger'
        });
        toast.present();
      }
    }
  }
  closeList() {
    this.modalController.dismiss();
  }
}
