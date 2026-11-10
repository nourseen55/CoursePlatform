import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NonoBastoot } from './nono-bastoot';

describe('NonoBastoot', () => {
  let component: NonoBastoot;
  let fixture: ComponentFixture<NonoBastoot>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NonoBastoot],
    }).compileComponents();

    fixture = TestBed.createComponent(NonoBastoot);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
