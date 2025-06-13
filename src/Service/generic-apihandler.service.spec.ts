import { TestBed } from '@angular/core/testing';

import { GenericApiHandlerService } from './generic-apihandler.service';

describe('GenericApihandlerService', () => {
  let service: GenericApiHandlerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GenericApiHandlerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
