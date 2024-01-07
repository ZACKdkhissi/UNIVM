namespace UniverMnagment.Entities
{
    public class CourseProfessor
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }
    }

}
