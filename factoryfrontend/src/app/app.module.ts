import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { RouteReuseStrategy } from '@angular/router';
import { MatDialogModule } from '@angular/material/dialog';
import {AppComponent} from "./app.component";
import {FeedBoxComponent} from "./app/feed-box/feed-box.component";
import {CreateEditBoxComponent} from "./app/create-edit-box/create-edit-box.component";
import {AppRoutingModule} from "./app-routing.module";
import { BoxDetailIdComponent } from './box-detail-id/box-detail-id.component';




 @NgModule({
   declarations: [AppComponent, FeedBoxComponent, CreateEditBoxComponent,BoxDetailIdComponent,CreateEditBoxComponent],
   imports: [BrowserModule, IonicModule.forRoot({mode: 'md'}), AppRoutingModule, HttpClientModule, ReactiveFormsModule, MatDialogModule],
   providers: [{ provide: RouteReuseStrategy, useClass: IonicRouteStrategy }],
   bootstrap: [AppComponent],
 })
export class AppModule {}
