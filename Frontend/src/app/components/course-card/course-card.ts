import { Component, input, output } from '@angular/core';
import { Course } from '../../Models/course';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-course-card',
  imports: [RouterLink],
  templateUrl: './course-card.html',
  styleUrl: './course-card.css',
})
export class CourseCard {

    course = input.required<Course>();
  remove = output<number>();

  onRemove() {
    this.remove.emit(this.course().id);
}
}