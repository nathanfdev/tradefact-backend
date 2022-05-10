import { NgModule, CUSTOM_ELEMENTS_SCHEMA, Injector, DoBootstrap, ɵCompiler_compileModuleAndAllComponentsSync__POST_R3__ } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { HttpClientModule } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { PackingListComponent } from './packing-list.component';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
  declarations: [
    PackingListComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [
    CommonModule, BrowserModule, HttpClientModule, FormsModule, ReactiveFormsModule, BrowserAnimationsModule,
    FlexLayoutModule, 
    MatInputModule, 
    MatButtonModule,
    MatFormFieldModule,
    MatDatepickerModule 
  ],
  providers: []
})

export class PackingListModule implements DoBootstrap  { 
  constructor(private injector: Injector) {}

  public ngDoBootstrap(): void {
    const elPackingList = createCustomElement(PackingListComponent, {
      injector: this.injector
    });
    customElements.define('packing-list', elPackingList);
  }
}