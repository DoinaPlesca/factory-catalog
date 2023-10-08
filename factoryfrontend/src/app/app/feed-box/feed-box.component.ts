import { Component, OnInit} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {Box, ResponseDto} from "../../../models";
import {ModalController, ToastController} from "@ionic/angular";
import {State} from "../../../state";
import {CreateEditBoxComponent} from "../create-edit-box/create-edit-box.component";
import {ServicesComponent} from "../services/services.component";
import {MatDialog} from "@angular/material/dialog";




@Component({
  selector: 'app-feed-box',
  templateUrl: './feed-box.component.html',
  styleUrls: ['./feed-box.component.scss'],
})

export class FeedBoxComponent implements OnInit {
  searchTerm: string = '';
  boxes: any[] = [];
  selectedBox: any;


  constructor
  (public http: HttpClient,
   public modalController: ModalController,
   public state: State,
   public toastController: ToastController,
   public service: ServicesComponent,
   public dialog: MatDialog) {
  }

 /* async searchBoxes() {
    try {
      const result = await this.service.searchBoxes(this.searchTerm, 10).toPromise();

      if (result) {
        this.boxes = result.filter((box) => {
              return (box.boxName && box.boxName.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
                  (box.description && box.description.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
                  (box.size && box.size.toLowerCase().includes(this.searchTerm.toLowerCase()));
            }
        );
      } else {
        this.boxes = [];
      }

      const toast = await this.toastController.create({
        message: 'Search completed successfully.',
        duration: 2000,
      });
      toast.present();
    } catch (error) {
      const toast = await this.toastController.create({
        message: 'Error searching boxes. Please try again later.',
        duration: 2000,
      });
      toast.present();
    }
  } */


  async fetchBoxes() {
    try {
      const result = await firstValueFrom(
          this.http.get<ResponseDto<Box[]>>(environment.baseUrl + '/factory/catalog')
      );

      this.state.boxes = result.responseData || [];

      const toast = await this.toastController.create({
        message: result.messageToClient,
        duration: 2000,
      });
      toast.present();
    } catch (error) {
      const toast = await this.toastController.create({
        message:
            'Error fetching boxes. Please try again later.',
        duration: 2000,
      });
      toast.present();
    }
  }

  ngOnInit(): void {
    this.fetchBoxes();
  }

  async deleteBox(boxId: number | undefined) {
    try {
      await firstValueFrom(this.http.delete(environment.baseUrl + '/catalog/boxes/' + boxId))
      this.state.boxes = this.state.boxes.filter(b => b.boxId != boxId)
      const toast = await this.toastController.create({
        message: 'The article was successfully deleted!',
        duration: 1233,
        color: "success"
      })
      toast.present();

    } catch (e) {
      if (e instanceof HttpErrorResponse) {
        const toast = await this.toastController.create({
          message: e.error.messageToClient,
          color: "danger"
        })
        toast.present();

      }

    }
  }

  openCreateModal() {
    const dialogRef = this.dialog.open(CreateEditBoxComponent, {
      data: null,
    });

    dialogRef.afterClosed().subscribe(result => {
      // Handle the result when the dialog is closed, if needed.
    });
  }

  selectAndEditBox(box: any) {
    this.selectedBox = box;

    if (this.selectedBox) {
      const dialogRef = this.dialog.open(CreateEditBoxComponent, {
        data: this.selectedBox,
      });

      dialogRef.afterClosed().subscribe(result => {
        // Handle the result when the dialog is closed, if needed.
      });
    }
  }




}

