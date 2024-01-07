namespace UniverMnagment.Entities
{
    public class Professor
    {
        public int ProfessorId { get; set; }
        public string Name { get; set; }

        public ICollection<CourseProfessor> CourseProfessors { get; set; }
    }

}
