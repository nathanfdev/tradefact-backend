import { enableProdMode } from "@angular/core";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { environment } from "src/environments/environment.prod";
import { PackingListModule } from "./packing-list.module";

if (environment.production) {
    enableProdMode();
}

const bootstrap = () => platformBrowserDynamic().bootstrapModule(PackingListModule);
bootstrap().catch(err => console.error(err))