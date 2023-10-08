import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { FeedBoxComponent} from "./app/feed-box/feed-box.component";
import { BoxDetailIdComponent } from './box-detail-id/box-detail-id.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'boxes',
    pathMatch: 'full'
  },
  {
    path:'boxes',
    component : FeedBoxComponent
  },
  {
    path:'boxes/:id',
    component : BoxDetailIdComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
