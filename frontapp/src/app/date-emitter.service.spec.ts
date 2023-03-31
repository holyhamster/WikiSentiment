import { TestBed } from '@angular/core/testing';

import { DateEmitterService } from './date-emitter.service';

describe('DateEmitterService', () => {
  let service: DateEmitterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DateEmitterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
