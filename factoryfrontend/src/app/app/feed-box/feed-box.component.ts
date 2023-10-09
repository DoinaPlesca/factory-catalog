import { Component, OnInit} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {firstValueFrom} from "rxjs";
import {Box, ResponseDto} from "../../../models";
import {ModalController, SearchbarChangeEventDetail, ToastController} from "@ionic/angular";
import {State} from "../../../state";
import {CreateEditBoxComponent} from "../create-edit-box/create-edit-box.component";
import {ServicesComponent} from "../services/services.component";
import {MatDialog} from "@angular/material/dialog";
import { ActivatedRoute, Router } from '@angular/router';




@Component({
  selector: 'app-feed-box',
  templateUrl: './feed-box.component.html',
  styleUrls: ['./feed-box.component.scss'],
})

export class FeedBoxComponent implements OnInit {

  searchTerm: string = '';
  boxes: Box[] = [];
  selectedBox: any;
  filteredBoxes: Box[] = [];


  constructor
  (private http: HttpClient,
   private modalController: ModalController,
   public state: State,
   private toastController: ToastController,
   private service: ServicesComponent,
   private dialog: MatDialog,
   private router: Router,
   private boxService: ServicesComponent,
   private route: ActivatedRoute
   ) {
    this.route.queryParams.subscribe((queryParams) => {
      if (queryParams['refresh'] === 'true') {
        // Refresh the boxes data
        this.fetchBoxes();
      }
    });
  }

  async fetchBoxes() {
    await this.boxService.getBoxes();
  }

  ngOnInit(): void {
   this.fetchBoxes();
  }


  navigateToOwnPage(boxId: number | undefined) {
    this.router.navigate(['/boxes', boxId]);
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

  filterBoxes(event: EventTarget) {
    this.searchTerm = (event as SearchbarChangeEventDetail).value;

    if (this.searchTerm) {
      this.filteredBoxes = this.state.boxes.filter((box) => {
        return (
          (box.boxName &&
            box.boxName.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
          (box.description &&
            box.description.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
          (box.size && box.size.toLowerCase().includes(this.searchTerm.toLowerCase()))
        );
      });
    } else {
      // If the search term is empty, show all boxes
      this.filteredBoxes = this.state.boxes;
    }
  }

  getDisplayedBoxes() {
    if (this.searchTerm === '') {
      return this.state.boxes;
    } else {
      return this.filteredBoxes.length > 0 ? this.filteredBoxes : [];
    }
  }



}

