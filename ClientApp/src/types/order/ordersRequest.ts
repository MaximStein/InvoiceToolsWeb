export interface OrdersFilter {
    //startTimeMillis: number;
    //endTimeMillis: number;
    //startTime:Date,
    //endTime:Date,
    shopIds?: string[];
    onlyPaid?: boolean;
    onlyWithoutInvoice:boolean;
}