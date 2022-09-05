import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { RepositoryService } from 'src/app/shared/services/repository.service';
import { LineItem } from 'src/types/order/lineItem';
import { Order } from 'src/types/order/order';
import { StatusResponseDto } from 'src/types/response/statusResponseDto';
import { Shop } from 'src/types/shop/shop';

@Component({
  selector: 'app-order-details',
  templateUrl: './upsert-order.component.html',
  styleUrls: ['./upsert-order.component.css']
})
export class UpsertOrderComponent implements OnInit, OnChanges {

  @Input() order: Order;
  availableShops: Shop[];

  @Output() itemSavedEvent = new EventEmitter<Order>();
  

  form: FormGroup;

  constructor(private fb: FormBuilder, private repoService: RepositoryService, private notifService: NotificationService,
    private route: ActivatedRoute, private router: Router,) { }

  ngOnInit(): void {


    this.repoService.getShops().subscribe(r => {
      this.availableShops = r;
      if (this.order == null) {
        var orderId = this.route.snapshot.queryParamMap.get('orderId');

        if (orderId != null) {
          this.repoService.getOrders([orderId!]).subscribe(r => {
            this.order = r[0];
            this.initForm();
          });
          
        }
        else {
          this.order = {
            timeOrdered: new Date(),
            orderNumber: "",
            currencyCode: "EUR",
            vatPercent: 19,
            items: []
          }
          
          this.initForm();
        }
      }
    });

    
  }

  private initForm() {
    this.form = this.fb.group({
      id: [this.order.id],
      shopId: [this.order.shopId],
      timeOrdered: this.order.timeOrdered,
      timeShipped: this.order.timeShipped,
      orderNumber: this.order.orderNumber,
      invoiceNumber:this.order.invoiceNumber,
      invoiceDate:this.order.invoiceDate,
      isPaid: this.order.isPaid,
      isCancelled: this.order.isCancelled,
      vatPercent: this.order.vatPercent,
      currencyCode: this.order.currencyCode,
      shippingAddress: this.order.shippingAddress,
      billingAddress: this.order.billingsAddress,
      items: this.fb.array([])
    });

    this.order.items.forEach(i => this.addLineItem(i));

    //this.addLineItemClick();

  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['item'] && changes['item'].currentValue) {
      this.initForm();
    }
  }

  get lineItems() {
    return this.form.controls["items"] as FormArray;
  }


  deleteLineItem(index: number) {
    this.lineItems.removeAt(index);
  }

  addLineItemClick() {
    this.addLineItem({
      title: "Artikel " + (this.lineItems.length + 1),
      price: 10,
      quantity: 1
    })
  }

  addLineItem(i: LineItem) {
    const lineItemForm = this.fb.group({
      title: [i.title, Validators.required],
      price: i.price,
      quantity: i.quantity,
      variation: i.variation
    });

    this.lineItems.push(lineItemForm);
  }

  onSubmit(): void {
    const values: Order = { ... this.form.value };

    this.repoService.upsertOrder(values)
      .subscribe({
        next: (res) => {
          this.notifService.showGenericSuccess();
          this.itemSavedEvent.emit(res);
        },
        error: (err: HttpErrorResponse) => {
          if (err.message)
            this.notifService.showNotification(err.message);
          else
            this.notifService.showGenericError();

        }
      })
  }

}
