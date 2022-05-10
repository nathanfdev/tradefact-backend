import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpRequest } from '@angular/common/http';
import { Pallet } from '../core/models/pallet';
import { from } from 'rxjs/internal/observable/from';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ShipmentLineItem } from '../core/models/shipment-line-item';

@Injectable({
  providedIn: 'root'
})
export class PackingListService {

  private palletDataSource: BehaviorSubject<Pallet[]> = new BehaviorSubject<Pallet[]>([]);
  private shipmentItemsDataSource: BehaviorSubject<ShipmentLineItem[]> = new BehaviorSubject<ShipmentLineItem[]>([]);

  private pallets: Pallet[] = []; 

  constructor(private http: HttpClient) { }


  loadShipment(shipmentid: string): void {

    const req = new HttpRequest('GET', '/packinglist?handler=PackingListItems&shipmentId=' + shipmentid, {
      reportProgress: true
    });  

    this.http.get<ShipmentLineItem[]>('packinglist?handler=PackingListItems&shipmentId='+shipmentid)
		.subscribe (
			(data: ShipmentLineItem[]) => {
				this.shipmentItemsDataSource.next(data);
			},
			(err: any) => console.error("loadAllPackages: ERROR"),
			() => console.log("loadAllPackages: always")
		);

    // let headers = new Headers();
    // headers.append('Content-Type', 'application/json');

    // let params = new HttpParams()
    // .set('handler', 'PackingListItems')
    // .set('shipmentid', shipmentid);

    // console.log('getting shipment');
    // console.log(params.toString());
    // console.log("Getting Shipment", shipmentid);

    // this.http
		// .request(req)
    // .pipe(
    //   tap( // Log the result or error
    //     data => console.log(data),
    //     error => console.error(error)
    //   )
    // );
		// .map((res: any) => {
		// 	return res.json();
		// })
		// .subscribe (
		// 	(data: any) => {
		// 		this.palletDataSource.next(data);
		// 	},
		// 	(err: any) => console.error("loadAllPackages: ERROR"),
		// 	() => console.log("loadAllPackages: always")
		// );

    this.palletDataSource.next(Object.assign([], this.pallets));
  }

  getShipmentItems(): Observable<ShipmentLineItem[]> {
    return this.shipmentItemsDataSource.asObservable();
  }

  getPalletList(): Observable<Pallet[]> {
    return this.palletDataSource.asObservable();
  }

  addPallet(pallet: Pallet) {
    this.palletDataSource.next(this.palletDataSource.value.concat(pallet));
  }

  editPallet(pallet: Pallet, index: number) {
    if (~index) {
      this.pallets[index] = pallet;
    }
    this.palletDataSource.next(Object.assign([], this.pallets));
  }

  removePallet(pallet: Pallet) {
    this.palletDataSource.next(this.palletDataSource.value.filter(obj => obj !== pallet));
  }

}