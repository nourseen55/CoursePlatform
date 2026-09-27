using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlatform.Domain.Enums
{
    public enum ProgressActionResult
    {
        Success,
        CourseNotFound,
        LessonNotFound,
        NotEnrolled,
        InvalidLesson,
        Failed
    }
}
