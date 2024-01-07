using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverMnagment.Data;
using UniverMnagment.Entities;

namespace UniverMnagment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController : ControllerBase
    {
        private readonly DataContext _context;

        public ProfessorController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Professor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Professor>>> GetProfessors()
        {
            return await _context.Professors
                .Include(p => p.CourseProfessors)
                    .ThenInclude(cp => cp.Course)
                .ToListAsync();
        }


        // GET: api/Professor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Professor>> GetProfessor(int id)
        {
            var professor = await _context.Professors
                .Include(p => p.CourseProfessors)
                    .ThenInclude(cp => cp.Course)
                .FirstOrDefaultAsync(p => p.ProfessorId == id);

            if (professor == null)
            {
                return NotFound();
            }

            return professor;
        }


        // POST: api/Professor
        [HttpPost]
        public async Task<ActionResult<Professor>> PostProfessor(Professor professor)
        {
            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfessor), new { id = professor.ProfessorId }, professor);
        }

        // PUT: api/Professor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfessor(int id, Professor professor)
        {
            if (id != professor.ProfessorId)
            {
                return BadRequest();
            }

            _context.Entry(professor).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Professors.Any(e => e.ProfessorId == id))
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

        // DELETE: api/Professor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfessor(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }

            _context.Professors.Remove(professor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{professorId}/addcourse/{courseId}")]
        public async Task<IActionResult> AddCourseToProfessor(int professorId, int courseId)
        {
            // Vérification de l'existence de la relation
            var existingRelation = await _context.CourseProfessors
                .AnyAsync(cp => cp.CourseId == courseId && cp.ProfessorId == professorId);

            if (existingRelation)
            {
                return BadRequest("Cette relation existe déjà.");
            }

            // Vérifier si le professeur et le cours existent
            var professorExists = await _context.Professors.AnyAsync(p => p.ProfessorId == professorId);
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);

            if (!professorExists || !courseExists)
            {
                return NotFound();
            }

            // Création de la nouvelle relation
            var courseProfessor = new CourseProfessor
            {
                CourseId = courseId,
                ProfessorId = professorId
            };

            _context.CourseProfessors.Add(courseProfessor);
            await _context.SaveChangesAsync();

            return NoContent(); // ou retourner un statut de succès avec des détails si nécessaire
        }


    }
}
