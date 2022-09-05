export interface LineItem {
    title: string;
    description?: string;
    //buyerMessage?: string | null;
    price: number;
    quantity: number;
    
    variation?: string;
    imageFile?: string;
}