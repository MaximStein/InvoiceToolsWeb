import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { UserComponent } from './user/user.component';
import { RegisterComponent } from './user/register/register.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginComponent } from './user/login/login.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgMaterialModule } from './ng-material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ProfileComponent } from './user/profile/profile.component';
import { MenuComponent } from './menu/menu.component';
import { ErrorHandlerService } from './shared/services/error-handler.service';
import { NotFoundComponent } from './error-pages/not-found/not-found.component';
import { JwtHelperService, JwtModule, JWT_OPTIONS } from "@auth0/angular-jwt";
import { AuthGuard } from './shared/guards/auth.guard';
import { ForbiddenComponent } from './error-pages/forbidden/forbidden.component';
import { AdminComponent } from './admin/admin.component';
import { AdminGuard } from './shared/guards/admin.guard';
import { ShopsComponent } from './shops/shops.component';
import { FormsModule } from '@angular/forms';
import { OrdersComponent } from './orders/orders.component';
import { UpsertOrderComponent } from './orders/upsert-order/upsert-order.component';
import { DatePipe } from '@angular/common';
import { InvoiceComponent } from './orders/invoice/invoice.component';
import { registerLocaleData } from '@angular/common';
import localeDe from '@angular/common/locales/de';
import { IndexComponent } from './index/index.component';

registerLocaleData(localeDe);


export function tokenGetter() {
  if(!localStorage.getItem("token"))
    localStorage.setItem("token","");

  return  localStorage.getItem("token");
}


@NgModule({
  declarations: [
    AppComponent,
    UserComponent,    
    RegisterComponent, LoginComponent, ProfileComponent, MenuComponent, NotFoundComponent, ForbiddenComponent, AdminComponent, ShopsComponent, OrdersComponent, UpsertOrderComponent, InvoiceComponent, IndexComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,ReactiveFormsModule, FormsModule,
    RouterModule.forRoot([
      { path: '', component: IndexComponent },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },      
      { path: 'admin', component: AdminComponent, canActivate: [AuthGuard, AdminGuard] },      
      { path: 'shops', component: ShopsComponent,  canActivate: [AuthGuard] },
      { path: 'orders', component:  OrdersComponent,  canActivate: [AuthGuard] },
      { path: 'invoice', component:  InvoiceComponent,  canActivate: [AuthGuard] },
      { path: 'upsert-order', component:  UpsertOrderComponent,  canActivate: [AuthGuard] },
      { path: 'register', component: RegisterComponent },
      { path: 'login', component: LoginComponent },
    ]),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ["localhost:7268"],
      //  disallowedRoutes: [],
        throwNoTokenError: false,
      }
    }),
    BrowserAnimationsModule,
    NgMaterialModule,
    FlexLayoutModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorHandlerService,
      multi: true
    }, 
    { provide: LOCALE_ID, useValue: 'de' },
    JwtHelperService,
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
