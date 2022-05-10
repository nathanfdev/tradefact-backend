import { TestBed } from '@angular/core/testing';

import { PackingListService } from './packing-list.service';

describe('PackingListService', () => {
  let service: PackingListService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PackingListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
