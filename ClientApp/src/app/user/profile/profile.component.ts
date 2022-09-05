import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, UrlSerializer } from '@angular/router';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { RepositoryService } from 'src/app/shared/services/repository.service';
import { StatusResponseDto } from 'src/types/response/statusResponseDto';
import { AppUser } from 'src/types/user/appUser';
import { UserSettings } from 'src/types/user/userSettings';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  public claims: [] = [];

  public userSettings: UserSettings;

  settingsForm: FormGroup;

  constructor(private _repository: RepositoryService, private _notificationService: NotificationService, private serializer: UrlSerializer, private router: Router) { }

  ngOnInit(): void {

    console.log(document.referrer);

    this._repository.getClaims().subscribe(res => {
      this.claims = res as [];
    });

    this._repository.getUserSettings().subscribe(res => {
      this.userSettings = res

      this.settingsForm = new FormGroup({       
        nextInvoiceNumber:new FormControl(res.nextInvoiceNumber),
        invoiceIssuerAddressLine:new FormControl(res.invoiceIssuerAddressLine),
        invoiceBodyText: new FormControl(res.invoiceBodyText),          
      //  invoiceFooterText: new FormControl(res.invoiceFooterText),
        defaultSalesTaxPercent: new FormControl(res.defaultSalesTaxPercent, [Validators.min(0), Validators.max(100)]),
      
      });

    });
  }

  updateProfile = () => {
    const values = { ... this.settingsForm.value };

    this._repository.updateUserProfile(values)
      .subscribe({
        next: (res: StatusResponseDto) => {
          this._notificationService.showGenericSuccess();
        },
        error: (err: HttpErrorResponse) => {
          if (err.message)
            this._notificationService.showNotification(err.message);
          else
            this._notificationService.showGenericError();
          
        }
      })
  }
}
