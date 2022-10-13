export interface OrdersFilter {
    //startTimeMillis: number;
    //endTimeMillis: number;
    //startTime:Date,
    //endTime:Date,
    minDateCreated?: Date,
    maxDateCreated?:Date,
    shopIds?: string[];
    onlyPaid?: boolean;
    onlyWithoutInvoice:boolean;
}