import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; 
import { AppUser } from 'src/types/user/appUser';
import { map, Observable } from 'rxjs';
import { StatusResponseDto } from 'src/types/response/statusResponseDto';
import { Shop } from 'src/types/shop/shop';
import { ShopAuthResponseDto } from 'src/types/shop/shopAuthResponseDto';
import { Order } from 'src/types/order/order';
import { OrdersFilter } from 'src/types/order/ordersRequest';
import { ListResult } from 'src/types/listResult';
import { UserSettings } from 'src/types/user/userSettings';

@Injectable({
  providedIn: 'root'
})
export class RepositoryService {

  constructor(private http: HttpClient) { }

  


  public getShopAuthorizationUrl(shopId:string, apiType:string):Observable<any> { 
    return this.http.get("api/shoplink/auth-url", 
        { responseType:"text", params: {shopId, shopApiType:apiType}  }); 
  };
  //returnUrl: window.location.protocol+"//"+window.location.host+"/shops?shopId="+

  public processShopAuthResponse(shopAuthResponse:ShopAuthResponseDto) {return  this.http.put<Shop>("api/shoplink/process-auth-response", shopAuthResponse); }
    
  public importOrders = (shopId:string) => this.http.get("api/shoplink/import-orders/"+shopId);

  public deleteOrders = (orderIds:string[]) => this.http.delete("api/shop/orders", { body:orderIds });

  public clearShopOrders = (shopId:string) => this.http.delete("api/shop/clear-orders/"+shopId);

  public getOrdersPage = (filter:OrdersFilter, pageIndex:number, pageSize:number) => this.http.post<ListResult<Order>>("api/shop/orders?pageIndex="+pageIndex+"&pageSize="+pageSize,filter);

  public getOrders = (ids:string[]) => this.http.get<Order[]>("api/shop/orders", {params:{ids:ids}})

  public makeOrderInvoices = (ids:string[]) => this.http.put<Order[]>("api/shop/make-invoices", ids)

  public getMockOrders = (filter:OrdersFilter, pageIndex:number, pageSize:number) => 
    this.http.post<ListResult<Order>>("api/shop/mock-orders?pageIndex="+pageIndex+"&pageSize="+pageSize,filter)
    .pipe(map(data => {
          data.items.forEach(i => {i.timeOrdered = new Date(i.timeOrdered); });
          return data;
    }));

  public upsertOrder = (order:Order) => this.http.post<Order>("api/shop/upsert-order", order)

  public getSupportedShopApis = () => this.http.get<string[]>("api/shop/supported-apis");

  public getClaims = () => this.http.get("api/user/claims");
  
  public getShops = () => this.http.get<Shop[]>("api/shop/shops");
 
  public deleteShop = (shopId:string) => this.http.delete("api/shop/"+shopId);

  public updateShopLabel = (shop:Shop) =>this.http.get("api/shop/update-label?shopId="+shop.id+"&label="+shop.label);

  public createShop = (group:Shop) => this.http.post<Shop>("api/shop/create-shop", group);

  public getUserSettings():Observable<UserSettings>  { return this.http.get<UserSettings>("api/user") };

  public updateUserProfile(profileData:AppUser):Observable<StatusResponseDto> { return this.http.put<StatusResponseDto>("/api/user/", profileData) }

  private createCompleteRoute = (route: string, envAddress: string) => {
    return `${envAddress}/${route}`;
  }
}
