export interface Shop {
    id:string|null;
    label:string;
    currentApiProvider:string|null,
    ordersCount?:number,
    isImportingOrders?:boolean,

    lastImportTime?:Date,
    lastImportOrdersUpdated?:Date,
    lastImportOrdersCreated?:Date,
    lastImportSuccessful?:boolean,

    oAuthAccess: {
        isActive:boolean,
        isTokenInvalid: boolean,
        shopType:string
    } | null
}