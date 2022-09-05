import { Person } from "../person";
import { Address } from "../address";
import { LineItem } from "./lineItem";


export interface Order {
    id?:string ;
    shopId?: string ;
    buyer?: Person ;
    timeOrdered: Date;
//    updatedTime?:Date;
    isPaid?:boolean;
    
    timeShipped?: Date ;
    shippingAddress?: string;
    billingsAddress?: string;
    orderNumber: string ;
    invoiceNumber?: number;
    invoiceDate?:Date;
    buyerMessage?: string ;
    vatPercent?: number;
    giftOptionsIsGift?: boolean;
    giftOptionsGiftMessage?: string;
    currencyCode: string;
    paymentMethod?:string;
    items: LineItem[];
    isCancelled?: boolean;
    
    refundAmount?: number;
    totalFees?: number ;
    
    importedFrom?: string;

    grandTotal?:number;

}
