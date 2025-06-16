using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.DTO.ScoreDTO;
using SchoolManagement.Helpers;
using SchoolManagement.Hub;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ScoresController(SchoolContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Scores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreOuputDTO>>> GetScores()
        {
          if (_context.Scores == null)
          {
              return NotFound();
          }
          var scores = await _context.Scores
                .Include(sc => sc.Course)
                .Select(sc => new ScoreOuputDTO
                {
                    UserId = sc.UserId,
                    StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                    CourseName = sc.Course.CourseName,
                    Year = sc.Course.AcademicYear.Year,
                    Process1 = sc.Process1,
                    Process2 = sc.Process2,
                    Midterm = sc.Midterm,
                    Final = sc.Final,
                    AverageScore = sc.AverageScore,
                    Grade = sc.Grade
                })
                .ToListAsync();
            var totalScores = scores.Count;
            return Ok(new { data = scores, totalCount = totalScores });
        }

        // GET: api/Scores
        //[AllowAnonymous]
        //[HttpGet("byYear/{year}")]
        //public async Task<ActionResult<IEnumerable<ScoreOuputDTO>>> GetScoresByYear([FromRoute]int year)
        //{
        //    if (_context.Scores == null)
        //    {
        //        return NotFound();
        //    }
        //    var scores = await _context.Scores
        //          .Include(sc => sc.Course)
        //          .Where(predicate: sc => sc.Course.StartDate.Year == year)
        //          .Select(sc => new ScoreOuputDTO
        //          {
        //              UserId = sc.UserId,
        //              StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
        //              CourseName = sc.Course.CourseName,
        //              Process1 = sc.Process1,
        //              Process2 = sc.Process2,
        //              Midterm = sc.Midterm,
        //              Final = sc.Final,
        //              AverageScore = sc.AverageScore,
        //              Grade = sc.Grade
        //          })
        //          .ToListAsync();
        //    //return await _context.Scores.ToListAsync();
        //    var totalScores = scores.Count;
        //    return Ok(new { data = scores, totalCount = totalScores });
        //}

        // GET: api/Scores/userId/courseId
        [HttpGet("{userId}/{courseId}")]
        public async Task<ActionResult<ScoreOuputDTO>> GetScore([FromRoute]String userId, [FromRoute]int courseId, CancellationToken cancellationToken = default)
        {
          if (_context.Scores == null)
          {
              return NotFound();
          }
            var score = await _context.Scores
                .Include(sc => sc.User)
                .Include(sc => sc.Course)
                .Where(sc => sc.UserId == userId && sc.CourseId == courseId)
                .Select(sc => new ScoreOuputDTO
                {
                    UserId = sc.UserId,
                    StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                    CourseName = sc.Course.CourseName,
                    Year = sc.Course.AcademicYear.Year,
                    Process1 = sc.Process1,
                    Process2 = sc.Process2,
                    Midterm = sc.Midterm,
                    Final = sc.Final,
                    AverageScore = sc.AverageScore,
                    Grade = sc.Grade
                })
                .FirstOrDefaultAsync();

            if (score == null)
            {
                return NotFound();
            }

            return score;
        }

        // GET: api/Scores/userId/courseId
        [HttpGet("{userId}")]
        public async Task<ActionResult<ScoreOuputDTO>> GetScoresUser([FromRoute] String userId, CancellationToken cancellationToken = default)
        {
            if (_context.Scores == null)
            {
                return NotFound();
            }
            var score = await _context.Scores
                .Include(sc => sc.User)
                .Include(sc => sc.Course)
                .Where(sc => sc.UserId == userId)
                .Select(sc => new ScoreOuputDTO
                {
                    UserId = sc.UserId,
                    StudentName = sc.User.FirstName + ' ' + sc.User.LastName,
                    CourseName = sc.Course.CourseName,
                    Year = sc.Course.AcademicYear.Year,
                    Process1 = sc.Process1,
                    Process2 = sc.Process2,
                    Midterm = sc.Midterm,
                    Final = sc.Final,
                    AverageScore = sc.AverageScore,
                    Grade = sc.Grade
                })
                .FirstOrDefaultAsync();

            if (score == null)
            {
                return NotFound();
            }

            return score;
        }

        // PATCH: api/Scores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Policy = "RequireLecturerOrAdmin")]
        [HttpPatch]
        public async Task<ActionResult<ScoreOuputDTO>> PatchScore([FromBody]ScoreInputPutDTO scoreDTO, CancellationToken cancellationToken = default)
        {
            if (scoreDTO == null)
            {
                return BadRequest();
            }
            var score = await _context.Scores.FirstOrDefaultAsync(c => c.CourseId == scoreDTO.CourseId && c.UserId == scoreDTO.UserId);
            Console.WriteLine("=========== FINDDDDD: ", score);
            if (score == null)
            {
                return NotFound();
            }

            if (scoreDTO.Process1 != null) 
            {
                score.Process1 = (float) scoreDTO.Process1;
            }
            if (scoreDTO.Process2 != null)
            {
                score.Process2 = (float)scoreDTO.Process2;
            }
            if (scoreDTO.Midterm != null)
            {
                score.Midterm = (float)scoreDTO.Midterm;
            }
            if (scoreDTO.Final != null)
            {
                score.Final = (float)scoreDTO.Final;
            }
            if (scoreDTO.Average != null)
            {
                score.AverageScore = (float)scoreDTO.Average;
            }
            else
            {
                score.AverageScore = (float)(score.Process1 * 0.1 + score.Process2 * 0.2 + score.Midterm * 0.2 + score.Final * 0.5);
            }
            if (scoreDTO.Grade != null)
            {
                score.Grade = (Grade)scoreDTO.Grade;
            }
            else
            {
                var average = score.Process1 * 0.1 + score.Process2 * 0.2 + score.Midterm * 0.2 + score.Final * 0.5;
                switch (Math.Floor(average))
                {
                    case 9.0:
                        score.Grade = Grade.Excellent;
                        break;
                    case 8.0:
                        score.Grade = Grade.VeryGood;
                        break;
                    case 7.0:
                        score.Grade = Grade.Good;
                        break;
                    case 6.0:
                        score.Grade = Grade.Average;
                        break;
                    default:
                        score.Grade = Grade.BelowAverge;
                        break;
                }
            }

            _context.Entry(score).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreExists(score.UserId, scoreDTO.CourseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var scoreOutput = await GetScore(score.UserId, score.CourseId, cancellationToken);
            if (scoreOutput.Result is NotFoundResult)
            {
                return NotFound();
            }
            await NotificationHelper.NotifyAsync(
                _context,
                _hubContext,
                score.UserId, // người nhận
                $"Your grade in course {score.Course.CourseName} has been updated."
            );

            return CreatedAtAction("GetScore", new { userId = score.UserId, courseId = score.CourseId }, scoreOutput.Value);
        }

        // POST: api/Scores
        [HttpPost]
        public async Task<IActionResult> PostScore(ScoreInputPostDTO scoreDTO, CancellationToken cancellationToken = default)
        {
          if (_context.Scores == null)
          {
              return Problem("Entity set 'SchoolContext.Scores'  is null.");
          }
            //_context.Scores.Add(score);
            var score = new Score
            {
                UserId = scoreDTO.UserId,
                CourseId = scoreDTO.CourseId,
                Process1 = 0,
                Process2 = 0,
                Midterm = 0,
                Final = 0,
                AverageScore = 0,
                Grade = Grade.NotGraded,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,        
            };

            _context.Scores.Add(score);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        // DELETE: api/Scores/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteScore(int? id, CancellationToken cancellationToken = default)
        //{
        //    if (_context.Scores == null)
        //    {
        //        return NotFound();
        //    }
        //    var score = await _context.Scores.FindAsync(id);
        //    if (score == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Scores.Remove(score);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool ScoreExists(string? id, int? courseId)
        {
            return (_context.Scores?.Any(e => e.UserId == id && e.CourseId == courseId)).GetValueOrDefault();
        }
    }
}
