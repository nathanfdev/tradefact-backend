export interface ShipmentLineItem {
    itemId: string,
    sku: string,
    desc: string,
    pallet: string,
    cartonBarcode: string,
    qty: number,
    cartonQty: number,
    cartonDimensions: number,
    cartonWeight: number,
    unitsCommited: number,    
    unitsPacked: number,    
    purchaseOrders: string[],    
}
