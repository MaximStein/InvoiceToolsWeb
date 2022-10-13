import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Shop } from 'src/types/shop/shop';
import { NotificationService } from '../shared/services/notification.service';
import { RepositoryService } from '../shared/services/repository.service';

@Component({
  selector: 'app-shops',
  templateUrl: './shops.component.html',
  styleUrls: ['./shops.component.scss']
})
export class ShopsComponent implements OnInit {

  shops: Shop[] = [];
  supportedApis: string[] = [];


  forms:FormGroup[] = [];

  constructor(private repoService: RepositoryService, private notificationService: NotificationService, 
    private route: ActivatedRoute, private router: Router, private fb: FormBuilder) { }

  ngOnInit(): void {

    var referrer = document.referrer;
    var queryParams = this.route.snapshot.queryParamMap;

    var shopId = queryParams.get('shopId');
  
    var state = queryParams.get('state');
    var code =  queryParams.get('code');

    var mockAuth = queryParams.get('mock-authorization');
    var returnUrl = queryParams.get('return-url');

    //https://localhost:4200/shops?code=tIawgwQ1zPukXp_d0ZFtVMzZbpSuGxK8wqZl8vXUdAUvJdUJ5WWLg7B0-h2hItOsa_4aiqrHsMcobI84WbmtA9l0WOeMpBQU7E2X&state=state

    if(mockAuth) {
      console.log(returnUrl);
      (<any>window.location) = (returnUrl+"&oauth_token=''&oauth_verifier=''");
    }
    
    if (code) {

      this.repoService.processShopAuthResponse({ state:state??undefined, code:code, referrer: referrer }).subscribe({
        next: r => {
          var index = this.shops.findIndex(s => s.id == shopId);
          if (index != -1)
            this.shops[index] = r;

            this.router.navigate([]);
            this.notificationService.showGenericSuccess();
        },
        error: e => {
          this.notificationService.showGenericError();
        }
      })
    }

    this.retrieveShops();

    this.repoService.getSupportedShopApis().subscribe(r => this.supportedApis = r);
  }

  retrieveShops() {
    this.repoService.getShops().subscribe({
      next: r => {
        this.shops = r;
        
        this.shops.forEach(s => {
          this.forms[s.id!] = this.fb.group({
            label:s.label
          });
        })

        this.router.navigate([]);        
      },
      error: e => {
        this.notificationService.showGenericError();
      }
    })
  }

  saveChanges(shop:Shop) {
    shop.label = this.forms[shop.id!].value.label;

    this.repoService.updateShopLabel(shop).subscribe({
      next: r => {
          
      },
      error: e => {
          this.notificationService.showGenericError();
      }
    });
  }

  startShopAuthorization(id: string, apiType: string) {
    this.repoService.getShopAuthorizationUrl(id, apiType).subscribe({
      next: r => {        
        //console.log(r);
        (<any>window.location) = (r);

      },
      error: e => {
        this.notificationService.showGenericError();
      }
    })
  }

  clearShopOrders(shop:Shop) {
    this.repoService.clearShopOrders(shop.id!).subscribe({
      next: r => {
        shop.ordersCount = 0;
      },
      error: e => {
        this.notificationService.showGenericError();
      }
    })
  }


  deleteApiAuth(shop:Shop) {
    
  }

  startImport(shop:Shop) {
    shop.isImportingOrders = true;

    this.repoService.importOrders(shop.id!).subscribe({
      next: r => {
        this.notificationService.showGenericSuccess();
        this.retrieveShops();
        shop.isImportingOrders = false;
      },
      error: e => {
        console.error(e);
        shop.isImportingOrders = false;
      }
    })
  }

  deleteShop(shop: Shop) {
    this.repoService.deleteShop(shop.id!).subscribe({
      next: r => { 
        this.shops = this.shops.filter(e => e.id != shop.id);
        
      },
      error: e => { this.notificationService.showGenericError(); }
    })
  }

  createShop() {
    var shop: Shop = { label: "Neuer Shop", id: null, oAuthAccess:null, currentApiProvider:null};
    this.repoService.createShop(shop).subscribe({
      next: r => {
        this.shops.push(r);
        this.notificationService.showGenericSuccess();
      },
      error: e => {
        this.notificationService.showGenericError();
      }
    })
  }

}
