using DinkToPdf;
using DinkToPdf.Contracts;
using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Hubs;
using ExamManagementSystem.Pages.ExamPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Security.Claims;
using System.Text;

namespace ExamManagementSystem.Service
{
    public class ExamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ExamService> _logger;
        private readonly NotificationHub _hub;
        public ExamService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, ILogger<ExamService> logger, NotificationHub hub)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _hub = hub;
        }

        public async Task<Exam> GetExamById(int id)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(x => x.Teacher)
                    .Include(x => x.ExamToQuestions).ThenInclude(x => x.Question).ThenInclude(x => x.Options)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return exam;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Exam> AppearExam(int id)
        {
            try
            {
                string userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                Exam? exam = await _context.Exams.Include(x => x.Results)
                    .Include(x => x.Teacher)
                    .Include(x => x.ExamToQuestions).ThenInclude(x => x.Question).ThenInclude(x => x.Options)
                    .FirstOrDefaultAsync(x => x.Id == id);

                exam.IsAppearedByCurrentStudent = exam.Results.Any(x => x.StudentId == userId && x.ExamId == id);

                return exam;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task SubmitExam(int examId, List<Answer> answers)
        {
            try
            {
                string? studentId = answers.FirstOrDefault()?.StudentId;
                await _context.Answers.AddRangeAsync(answers);

                // calculate score
                float score = 0F;
                foreach (Answer item in answers)
                {
                    if (item.Option.IsCorrect)
                    {
                        score += item.Question.Marks;
                    }
                }
                float totalMarks = answers.Select(x => x.Question).Sum(x => x.Marks);
                ExamResult result = new()
                {
                    StudentId = studentId,
                    ExamId = examId,
                    Score = score,
                    Status = (100 * score / totalMarks) >= 33 ? Enums.ExamResultStatus.Pass : Enums.ExamResultStatus.Fail
                };
                _ = _context.ExamResults.Add(result);

                _ = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<List<ExamToStudent>> GetExamsToStudentByExam(int examId)
        {
            IQueryable<ExamToStudent> examToStudents = _context.ExamToStudents.Include(x => x.Student).Where(x => x.ExamId == examId);
            return await examToStudents.ToListAsync();
        }

        public async Task<List<Exam>> GetExams()
        {
            try
            {
                IOrderedQueryable<Exam> exams = _context.Exams
                    .Include(x => x.Teacher)
                    .Include(x => x.ExamToQuestions).ThenInclude(x => x.Question)
                    .OrderBy(x => x.ExamStatus).ThenByDescending(x => x.Date);
                return await exams.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<Exam>> MyExams()
        {
            try
            {
                string userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                IQueryable<Exam> exams = _context.ExamToStudents.Where(x => x.StudentId == userId)
                                    .Include(e => e.Exam).ThenInclude(x => x.ExamToQuestions).ThenInclude(x => x.Question)
                                    .Include(e => e.Exam).ThenInclude(e => e.Teacher).Select(x => x.Exam).OrderBy(x => x.ExamStatus).ThenByDescending(x => x.Date);

                return await exams.ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> SaveExam(Exam exam)
        {
            try
            {
                if (exam.Id > 0)
                {
                    _context.Exams.Update(exam);

                    // Notify students if exam status is changed
                    if (exam.ExamStatus != Enums.EnumExamStatus.NotStarted && _context.Entry(exam).Properties.Any(x => x.IsModified && x.Metadata.Name == nameof(exam.ExamStatus)))
                    {
                        var studentsToNotify = await _context.ExamToStudents.Include(x => x.Student).Where(x => x.ExamId == exam.Id).Select(x => x.Student.Email).ToListAsync();
                        studentsToNotify?.ForEach(async student => { await _hub.NotifyExamStatus(student, exam); });
                    }
                }
                else
                {
                    _context.Exams.Add(exam);
                }
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> AssignExamToStudent(List<ExamToStudent> examToStudent)
        {
            try
            {
                var result = 0;
                if (examToStudent.Count > 0)
                {
                    int examId = examToStudent.FirstOrDefault()!.ExamId;
                    IEnumerable<string> inputStudentIds = examToStudent.Select(x => x.StudentId);

                    List<ExamToStudent> existing = _context.ExamToStudents.Where(x => x.ExamId == examId).ToList();
                    List<string> existingStudentIds = _context.ExamToStudents.Where(x => x.ExamId == examId).Select(x => x.StudentId).ToList();

                    IEnumerable<string> newStudents = inputStudentIds.Except(existingStudentIds).ToList();
                    IEnumerable<string> deleting = existingStudentIds.AsEnumerable().Except(inputStudentIds);

                    foreach (ExamToStudent? item in examToStudent.Where(x => newStudents.Contains(x.StudentId)))
                    {
                        _ = _context.ExamToStudents.Add(item);

                        // Notify new students
                        var studentsToNotify = _context.Users.Where(x => x.Id == item.StudentId).Select(x => x.Email).ToList();
                        studentsToNotify?.ForEach(async student => { await _hub.NotifyExamStatus(student, item.Exam); });
                    }

                    foreach (ExamToStudent? item in existing.Where(eTos => deleting.Contains(eTos.StudentId)))
                    {
                        _ = _context.ExamToStudents.Remove(item);
                    }
                    result = await _context.SaveChangesAsync();
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> DeleteExam(int id)
        {
            try
            {
                Exam? exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id);
                if (exam != null)
                {
                    _ = _context.Exams.Remove(exam);
                    return await _context.SaveChangesAsync();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
