using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniverMnagment.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniverMnagment.Data;

namespace UniverMnagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _context;

        public StudentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                .ToListAsync();
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // POST: api/Student
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.ID }, student);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.ID)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{studentId}/addcourse/{courseId}")]
        public async Task<IActionResult> AddCourseToStudent(int studentId, int courseId)
        {
            // Vérifier si le cours et l'étudiant existent
            var studentExists = await _context.Students.AnyAsync(s => s.ID == studentId);
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);

            if (!studentExists || !courseExists)
            {
                return NotFound("L'étudiant ou le cours spécifié n'existe pas.");
            }

            // Vérifier si l'association existe déjà
            var existingAssociation = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (existingAssociation)
            {
                return BadRequest("L'étudiant est déjà inscrit à ce cours.");
            }

            // Créer et ajouter la nouvelle association
            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId
            };

            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();

            return Ok("L'étudiant a été inscrit au cours avec succès.");
        }



    }
}

