import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { PackingListService } from 'src/app/services/packing-list.service';
import { BehaviorSubject, Subject,  } from 'rxjs';
import { filter, startWith, takeUntil } from 'rxjs/operators';
import { Pallet } from 'src/app/core/models/pallet';
import { ShipmentLineItem } from 'src/app/core/models/shipment-line-item';
import { PackingList } from 'src/app/core/models/packinglist';


@Component({
  templateUrl: './packing-list.component.html',
  styleUrls: ['./packing-list.component.scss'],
  // encapsulation: ViewEncapsulation.ShadowDom
})
export class PackingListComponent implements OnInit, OnDestroy {
  @Input() shipmentid: string;

  model: PackingList = { dimensions: "mm", weight:"kg", shippingmethod: "Sea"}
  palletList$ = new BehaviorSubject<Pallet[]>([]);
  shipmentItemList$ = new BehaviorSubject<ShipmentLineItem[]>([]);

  shipmentItemList = new Array<ShipmentLineItem>();
  
  private ngUnsubscribe = new Subject();

  constructor(private packingListService: PackingListService) { }

  ngOnInit(): void {

    this.packingListService.getPalletList()
    .pipe(
       startWith([]),
       filter(pallets => pallets.length > 0),
       takeUntil(this.ngUnsubscribe)
    )
    .subscribe(pallets => { 
      console.log(pallets);
      this.palletList$.next(pallets) 
    });

    this.packingListService.getShipmentItems()
    .pipe(
       startWith([]),
       filter(items => items.length > 0),
       takeUntil(this.ngUnsubscribe)
    )
    .subscribe(items => {
      console.log(items);
      this.shipmentItemList = items;
      this.shipmentItemList$.next(items)
    });

    // Populate Packing List Data;
    this.packingListService.loadShipment(this.shipmentid);
  }

  categoryOnChange(e: any) {  
    console.log(e.target.value);  
  }  

  // private populateUsers() {
  //   this.dataService.getData().subscribe((event: HttpEvent<any>) => {
  //     switch (event.type) {
  //       case HttpEventType.Sent:
  //         console.log('Request sent!');
  //         break;
  //       case HttpEventType.ResponseHeader:
  //         console.log('Response header received!');
  //         break;
  //       case HttpEventType.DownloadProgress:
  //         const kbLoaded = Math.round(event.loaded / 1024);
  //         console.log(`Download in progress! ${kbLoaded}Kb loaded`);
  //         break;
  //       case HttpEventType.Response:
  //         console.log('Done!', event.body);
  //         //this.users = event.body;
  //     }
  //   });
  // }

  // private populatePackingList() {
  //   this.dataService.getPackingList(this.shipmentid).subscribe((event: HttpEvent<any>) => {
  //     switch (event.type) {
  //       case HttpEventType.Sent:
  //         console.log('Request sent!');
  //         break;
  //       case HttpEventType.ResponseHeader:
  //         console.log('Response header received!');
  //         break;
  //       case HttpEventType.DownloadProgress:
  //         const kbLoaded = Math.round(event.loaded / 1024);
  //         console.log(`Download in progress! ${kbLoaded}Kb loaded`);
  //         break;
  //       case HttpEventType.Response:
  //         console.log('Done!', event.body);
  //         this.items = event.body;
  //     }
  //   });
  // }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  addPallet(): void {
    console.log('Add Pallet');
    this.activePallet = { id:0, label:'label', width:0, depth:0, height:0, weight:0 }; 
    this.toggleModal('add');
  }

  editPallet(pallet: Pallet, index: number): void {
    console.log('Edit Pallet');
    this.activePallet = pallet; 
    this.activePalletIndex = index;
    this.toggleModal('edit');
  }

  savePallet(): void {
    console.dir('Save Pallet', this.activePallet);
    if (this.modalAction === 'add') {
      this.packingListService.addPallet(this.activePallet);  
    } else {
      this.packingListService.editPallet(this.activePallet, this.activePalletIndex);  
    }
    this.toggleModal();
  }

  handleSave(event: Event) { 
    console.log("Click", event)
    console.log("starting")
    //Iterating through data values and trying to display onto table
    for(var i = 0; i < this.shipmentItemList.length; i++){
      const item:ShipmentLineItem = this.shipmentItemList[i];
      console.log(item);
    }
    console.log("starting")
  }

  activePalletIndex: number = 0;
  activePallet: Pallet;

  showModal = false;
  modalAction = "add";

  toggleModal(action?: string){
    this.modalAction = action ?? "add";
    this.showModal = !this.showModal;
  }

}
