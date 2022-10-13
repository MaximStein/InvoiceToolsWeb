import { SelectionModel } from '@angular/cdk/collections';
import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Order } from 'src/types/order/order';
import { OrdersFilter } from 'src/types/order/ordersRequest';
import { Shop } from 'src/types/shop/shop';
import { AppUser } from 'src/types/user/appUser';
import { UserSettings } from 'src/types/user/userSettings';
import { NotificationService } from '../shared/services/notification.service';
import { RepositoryService } from '../shared/services/repository.service';


@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {

  shops: Shop[];
  //detailsViewOrder?:Order;  


  settings: UserSettings;

  filterShopSelection = new SelectionModel<Shop>(true, []);
  orderSelection = new SelectionModel<string>(true, []);
  
  ordersFilter:OrdersFilter = {
    onlyWithoutInvoice: true,
    onlyPaid: true,
    minDateCreated: undefined,
    maxDateCreated:undefined
  }

  totalOrders = 0;
  pageIndex = 0;
  pageSize = 50;
  //orders:Order[] = []

  dataSource = new MatTableDataSource<Order>();
  displayedColumns: string[] = [
    'select', 'shop', 'orderNumber', 'orderDate', 'grandTotal', 'shortDescription', 'shippedTime', 
    //'buyer',
    'invoiceNumber', 'buyerMessage', 'isPaid', 'actions'];
  @ViewChild(MatPaginator) paginator: MatPaginator;

  get selectedShopsString() {
    return this.filterShopSelection.selected.map(s => s.label).join(", ");
  }


  constructor(private repoService: RepositoryService, private notificationService: NotificationService,
    private route: ActivatedRoute, private router: Router, private datePipe: DatePipe) {

    this.filterShopSelection.changed.subscribe(e => {this.onFilterChanged();})

    this.orderSelection.changed.subscribe(e => {
      this.router.navigate(['.'], {
        relativeTo: this.route,
        queryParams: { 'selection': this.orderSelection.selected.join(",") },
        queryParamsHandling: 'merge'
      })
    });
  }

  onDateRangeFilterChanged(type: string, event: MatDatepickerInputEvent<Date>) {
    if(type =="start")
      this.ordersFilter.minDateCreated = event.value??undefined;
    else if(type == "end")
      this.ordersFilter.maxDateCreated = event.value??undefined;

    this.onFilterChanged();
  }

  onFilterChanged() {
    this.ordersFilter.shopIds = this.filterShopSelection.selected.map(s => s.id!);
    this.router.navigate(['.'], {
      relativeTo: this.route,
      queryParams: { 
        'filterShops': this.ordersFilter.shopIds.join(","), 
        'filterOnlyPaid':this.ordersFilter.onlyPaid, 
        'filterOnlyWithoutInvoice':this.ordersFilter.onlyWithoutInvoice,
        'filterMinDateCreated': this.ordersFilter.minDateCreated?.toDateString(),
        'filterMaxDateCreated': this.ordersFilter.maxDateCreated?.toDateString()
      },
      queryParamsHandling: 'merge'
    })
  }

  goToInvoiceView(orders: Order[]) {
    this.router.navigate(['/invoice'], { queryParams: { 'orderIds': orders.map(o => o.id) } });
  }


  isAllSelected() {
    for (var i = 0; i < this.dataSource.data.length; i++) {
      if (!this.orderSelection.isSelected(this.dataSource.data[i].id!))
        return false;
    }

    return true;
  }

  toggleAllRows() {
    if (this.isAllSelected()) {
      this.orderSelection.clear();
      return;
    }
    // this.orderSelection.select(...this.dataSource.data.map(d => d.id!));
    this.orderSelection.select(...this.dataSource.data.map(d => d.id!));
  }

  deleteOrders(orderIds: string[]) {

  }

  orderSaved(order: Order) {
    this.loadOrders();
  }

  ngAfterViewInit() {
  }


  getShopString(o: Order): string {
    var i = this.shops?.findIndex(shop => shop.id == o.shopId);
    return i == -1 || i === undefined ? '-' : this.shops[i].label;
  }


  loadOrders() {
    
    this.repoService.getOrdersPage(this.ordersFilter, this.pageIndex, this.pageSize).subscribe(r => {

      this.dataSource.data = r.items;
      //this.paginator.length = r.totalItemsCount;
      this.totalOrders = r.totalItemsCount;

      this.restoreStateFromUrl();
    })


  }

  onPageChange(event: PageEvent) {
    //this._loadOrders([], event.pageIndex, event.pageSize);
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParamsHandling: 'merge',
      queryParams: {
        pageIndex: event.pageIndex,
        pageSize: event.pageSize
      }
    })

    this.loadOrders();
  }


  private restoreStateFromUrl() {
    var params = this.route.snapshot.queryParamMap;
    this.pageIndex = Number(params.get('pageIndex') ?? 0);
    this.pageSize = Number(params.get('pageSize') ?? 50);

    var selected = (params.get('selection') ?? '').split(',');
    selected.forEach(id => {
      if (id.length > 0)
        this.orderSelection.select(id);
    });

    var paramsMinDate = params.get('filterMinDateCreated');
    this.ordersFilter.minDateCreated = (paramsMinDate ? new Date(paramsMinDate) : undefined);

    var paramsMaxDate = params.get('filterMaxDateCreated');
    this.ordersFilter.maxDateCreated = (paramsMaxDate ? new Date(paramsMaxDate) : undefined);

    /* this.orderSelection.selected.forEach(s => {
            var item = r.items.find(i => i.id == s.id);
            if(item != null)
            {
              this.orderSelection.deselect(s);
              this.orderSelection.select(item);
            }
    
          }); */


    //var invoiceViewOrders = params.get()
  }

  _renderGrandTotal = (o: Order) => o.grandTotal;

  ngOnInit(): void {

    this.restoreStateFromUrl();

    this.repoService.getUserSettings().subscribe(r => {
      this.settings = r;
    });

    this.repoService.getShops().subscribe(r => {
      this.shops = r;
      this.filterShopSelection.select(...this.shops);
      this.loadOrders();

      /*this.selectedOrder = {
        id: undefined,
        shopId: this.shops[0].id!,
        buyer: undefined,
        orderTime: new Date(),
        shippedTime: undefined,
        shippingAddress: undefined,
        billingsAddress: undefined,
        orderNumber: undefined,
        invoiceNumber: undefined,
        totalFees: undefined,
        buyerMessage: undefined,
        isPaid: undefined,
        refundAmount: undefined,
        vatPercent: 0,
        giftOptionsIsGift: undefined,
        giftOptionsGiftMessage: undefined,
        currencyCode: '',
        items: [],
        isCancelled: undefined,
        isUserCreated:true
      };*/
    });

    //this._loadOrders([], 0, 5);


  }



}
