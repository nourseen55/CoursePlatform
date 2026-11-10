import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageCourse } from './manage-course';

describe('ManageCourse', () => {
  let component: ManageCourse;
  let fixture: ComponentFixture<ManageCourse>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageCourse],
    }).compileComponents();

    fixture = TestBed.createComponent(ManageCourse);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
