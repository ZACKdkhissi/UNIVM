namespace UniverMnagment.Entities
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

}
