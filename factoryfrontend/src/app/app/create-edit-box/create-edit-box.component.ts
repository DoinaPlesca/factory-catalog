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
    public service : ServicesComponent

  ) {
    this.createEditBoxForm = this.fb.group({
      boxName: '',
      description: '',
      size: '',
      price: '',
      imageUrl: 'https://plus.unsplash.com/premium_photo-1661347900107-eee09e9ae234?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8OXx8Ym94fGVufDB8fDB8fHww&auto=format&fit=crop&w=500&q=60',
    });

  }
  ngOnInit(): void {
   
  }

  async saveForm() {

    // TODO:  before saving the form, we need to check if the form is valid

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


  cancel() {
    this.modalController.dismiss(null, 'cancel');
  }

  confirm() {
    this.modalController.dismiss('confirm');

    // method for createing box from service 
  }


  closeList() {
    this.modalController.dismiss();
  }
}
