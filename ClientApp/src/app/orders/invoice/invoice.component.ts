import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { RepositoryService } from 'src/app/shared/services/repository.service';
import { Order } from 'src/types/order/order';
import { AppUser } from 'src/types/user/appUser';
import { jsPDF } from "jspdf";
import * as pdfMake from "pdfmake/build/pdfmake";
import * as pdfFonts from "pdfmake/build/vfs_fonts";
import { UserSettings } from 'src/types/user/userSettings';
(<any>pdfMake).vfs = pdfFonts.pdfMake.vfs;
const html2pdfmake = require("html-to-pdfmake");

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit {



  settings: UserSettings;
  orders:Order[]= [];
  //  @Input() returnUrl: string;

  @Output() goBackEvent = new EventEmitter<void>();


  constructor(private repoService: RepositoryService, private notificationService: NotificationService,
    private route: ActivatedRoute, private router: Router, private datePipe: DatePipe) {

  }


  get taxPercent() { return this.settings?.defaultSalesTaxPercent  }

  getNetTotal = (o:Order) => o.grandTotal! / (1+(this.taxPercent!/100));

  getVatTotal = (o:Order) => o.grandTotal! - this.getNetTotal(o);
  
  downloadPdf() {
    var container = document.getElementById("document-container");
    var html = html2pdfmake(container!.innerHTML, {tableAutoSize:true});

    const documentDefinition = { content: [html], 
      pageBreakAfter: function(currentNode:any) {
        console.log(currentNode);
      },
      pageBreakBefore: function(currentNode:any) {
        if(currentNode.style && currentNode.style.indexOf('pdf-pagebreak-before') > -1) {
          console.log(currentNode);
          return true;
        }
        return false;
      }
    };
    pdfMake.createPdf(documentDefinition).download();
  }

  ngOnInit(): void {

    var orderIds = this.route.snapshot.queryParamMap.get('orderIds');

    this.repoService.getUserSettings().subscribe(r => {
      this.settings = r;

      this.repoService.makeOrderInvoices(orderIds!.split(',')).subscribe(r => this.orders = r)
    });

  }

}
