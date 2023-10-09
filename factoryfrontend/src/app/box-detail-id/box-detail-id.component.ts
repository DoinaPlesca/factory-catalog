import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ServicesComponent } from '../app/services/services.component';
import { State } from 'src/state';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ModalController, ToastController } from '@ionic/angular';
import { HttpClient } from '@angular/common/http';
import { Box } from 'src/models';

@Component({
  selector: 'app-box-detail-id',
  templateUrl: './box-detail-id.component.html',
  styleUrls: ['./box-detail-id.component.scss'],
})
export class BoxDetailIdComponent implements OnInit {
  formGroup: FormGroup;
  box: Box;

  constructor(
    public fb: FormBuilder,
    public modalController : ModalController,
    public http: HttpClient,
    public toastController : ToastController,
    public service : ServicesComponent,
    private route: ActivatedRoute,private boxService: ServicesComponent,
    public state: State,
    private router: Router,
    public formBuilder: FormBuilder) {

      this.formGroup = this.fb.group({
        boxName: ['', [Validators.required, Validators.minLength(5)]],
        description: ['', Validators.required],
        size: ['', Validators.required],
        price: ['', Validators.required],
        imageUrl: ['', Validators.required],
      });
    }

   async ngOnInit() {
      try {
        const boxId = parseInt(this.route.snapshot.paramMap.get('id'));
        this.box = await this.boxService.getBoxById(boxId);

        if (this.box) {
          this.formGroup.patchValue({
            boxName: this.box.boxName,
            description: this.box.description,
            size: this.box.size,
            price: this.box.price,
            imageUrl: this.box.imageUrl
          });
        }
      } catch (error) {
        console.error('Error fetching box data', error);
      }
    }

  updateForm() {
    if (this.formGroup.valid) {
      this.boxService.updateBoxById(
        this.box.boxId,
        this.formGroup.value
        );

        this.router.navigate(['/boxes']);
     }
  }
}
