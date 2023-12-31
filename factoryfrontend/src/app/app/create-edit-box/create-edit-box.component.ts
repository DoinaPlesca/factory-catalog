import {Component, EventEmitter, Inject, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, Validators} from "@angular/forms";
import {ModalController, ToastController} from "@ionic/angular";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {State} from "../../../state";
import {Box, ResponseDto} from "../../../models";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {ServicesComponent} from "../services/services.component";
import { MAT_DIALOG_DATA } from '@angular/material/dialog';


function sizeValidator(control: AbstractControl): { [key: string]: boolean } | null {
  const validSizes = ['small', 'medium', 'large', 'extra-large'];
  const sizeValue = control.value && control.value.toLowerCase();

  if (!sizeValue || validSizes.indexOf(sizeValue) === -1) {
    return { invalidSize: true };
  }
  if (sizeValue !== sizeValue.toLowerCase() && sizeValue !== sizeValue.toUpperCase()) {
    return { mixedCaseSize: true };
  }

  return null;
}
@Component({
  selector: 'app-create-edit-box',
  templateUrl: './create-edit-box.component.html',
  styleUrls: ['./create-edit-box.component.scss'],
})
export class CreateEditBoxComponent  implements OnInit {

  @Output() recordEdited: EventEmitter<void> = new EventEmitter<void>();

  PLACEHOLDER_IMAGE: string = 'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.salonlfc.com%2Fwp-content%2Fuploads%2F2018%2F01%2Fimage-not-found-1-scaled-1150x647.png&f=1&nofb=1&ipt=57742facc9cf7c327c76e0af8c43ffc254627689e82cd3701848b2f966fcc0ae&ipo=images';


  createEditBoxForm = this.fb.group({
    boxName: ['', [Validators.required, Validators.minLength(2)]],
    description : ['' ,  Validators.required],
    size: ['', [Validators.required, sizeValidator]],
    price: ['', [Validators.required, Validators.pattern(/^[1-9]\d*$/),],],
    imageUrl : ['' , Validators.required],
  })
  get errorControl() {
    return this.createEditBoxForm.controls;
  }

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
      imageUrl: this.PLACEHOLDER_IMAGE,
    });

  }
  ngOnInit(): void {

  }

  async saveForm() {
    if (this.createEditBoxForm.valid) {
      try {
        const observable = this.http.post<ResponseDto<Box>>(
          environment.baseUrl + '/catalog/boxes',
          this.createEditBoxForm.getRawValue()
        );

        const response = await firstValueFrom(observable);

        if (response && response.responseData) {
          this.state.boxes.push(response.responseData);
        }

        const toast = await this.toastController.create({
          message: 'Box was successfully saved',
          duration: 2000,
          color: 'success',
        });

        toast.present();
        this.modalController.dismiss();
      } catch (e) {
        if (e instanceof HttpErrorResponse) {
          const toast = await this.toastController.create({
            message: e.error.messageToClient,
            color: 'danger',
            duration:2000,
          });

          toast.present();
        }
      }
    } else {
      // Handle form validation errors
      const toast = await this.toastController.create({
        message: 'Please provide all the required values!',
        color: 'danger',
        duration:2000,
      });

      toast.present();
    }
  }


  cancel() {
    this.modalController.dismiss(null, 'cancel');
  }
  confirm() {
    this.modalController.dismiss('confirm');

  }
  closeList() {
    this.modalController.dismiss();
  }
}
