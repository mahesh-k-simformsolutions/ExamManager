using DinkToPdf;
using DinkToPdf.Contracts;
using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
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
        private readonly IConverter _converter;
        private readonly ILogger<ExamService> _logger;
        public ExamService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IConverter converter, ILogger<ExamService> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _converter = converter;
            _logger = logger;
        }

        public async Task<Exam> GetExamById(int id)
        {
            try
            {
                return await _context.Exams.FirstOrDefaultAsync(x => x.Id == id);
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
                Exam? exam = await _context.Exams.Include(x => x.Teacher).FirstOrDefaultAsync(x => x.Id == id);
                exam.Questions = await _context.ExamToQuestions.Include(x => x.Question).ThenInclude(x => x.Options).Where(x => x.ExamId == id).Select(x => x.Question).ToListAsync();
                exam.IsAppearedByCurrentStudent = _context.ExamResults.Any(x => x.StudentId == userId && x.ExamId == id);
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
            IQueryable<ExamToStudent> examToStudents = _context.ExamToStudents.Where(x => x.ExamId == examId);
            return await examToStudents.ToListAsync();
        }

        public async Task<List<Exam>> GetExams()
        {
            try
            {
                IOrderedQueryable<Exam> exams = _context.Exams.Include(x => x.Teacher).OrderBy(x => x.ExamStatus).ThenByDescending(x => x.Date);
                foreach (Exam? item in exams)
                {
                    IQueryable<Question> examToQuestions = _context.ExamToQuestions.Where(x => x.ExamId == item.Id).Select(x => x.Question);
                    item.Questions = await examToQuestions.ToListAsync();
                }
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
                                    .Include(e => e.Exam)
                                    .ThenInclude(e => e.Teacher).Select(x => x.Exam);

                foreach (Exam? item in exams)
                {
                    IQueryable<Question> examToQuestions = _context.ExamToQuestions.Where(x => x.ExamId == item.Id).Select(x => x.Question);
                    item.Questions = await examToQuestions.ToListAsync();
                }
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
                _ = exam.Id > 0 ? _context.Exams.Update(exam) : _context.Exams.Add(exam);
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
                if (examToStudent.Count > 0)
                {

                    int examId = examToStudent.FirstOrDefault()!.ExamId;
                    IEnumerable<string> inputStudentIds = examToStudent.Select(x => x.StudentId);

                    IQueryable<ExamToStudent> existing = _context.ExamToStudents.Where(x => x.ExamId == examId);
                    IQueryable<string> existingStudentIds = _context.ExamToStudents.Where(x => x.ExamId == examId).Select(x => x.StudentId);

                    IEnumerable<string> newStudents = inputStudentIds.Except(existingStudentIds);
                    IEnumerable<string> deleting = existingStudentIds.AsEnumerable().Except(inputStudentIds);

                    foreach (ExamToStudent? item in examToStudent.Where(x => newStudents.Contains(x.StudentId)))
                    {
                        _ = _context.ExamToStudents.Add(item);
                    }

                    foreach (ExamToStudent? item in existing.Where(eTos => deleting.Contains(eTos.StudentId)))
                    {
                        _ = _context.ExamToStudents.Remove(item);
                    }
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

        public async Task<List<ExamResult>> GetResults()
        {
            return await _context.ExamResults
                .Include(er => er.Exam).ThenInclude(e => e.Teacher)
                .Include(er => er.Exam).ThenInclude(e => e.Results)
                .Include(er => er.Student).ToListAsync();
        }
        public async Task<List<ExamResult>> MyScorecard()
        {
            try
            {
                string userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                return await _context.ExamResults
                    .Include(er => er.Exam)
                    .Include(er => er.Student)
                    .Where(x => x.StudentId == userId).ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<byte[]> DownloadScoreCard(int examResultId)
        {
            try
            {
                ExamResult? examResult = await _context.ExamResults
                    .Include(er => er.Exam)
                    .Include(er => er.Student)
                    .FirstOrDefaultAsync(x => x.Id == examResultId);
                IQueryable<Question> questions = _context.ExamToQuestions.Where(x => x.ExamId == examResult.ExamId).Include(x => x.Question).Select(x => x.Question);
                examResult.Exam.Questions = await questions.ToListAsync();

                StringBuilder scoreCard = new(File.ReadAllText(@"./Helpers/Template/Scorecard.html"));
                _ = scoreCard.Replace("$StudentName$", examResult?.Student.Name);
                _ = scoreCard.Replace("$Email$", examResult?.Student.Email);
                _ = scoreCard.Replace("$Date$", examResult?.Exam.Date.ToString("dd/MM/yyyy"));
                _ = scoreCard.Replace("$Time$", examResult?.Exam.StartTime.ToString("h:mm tt"));
                _ = scoreCard.Replace("$Exam$", examResult?.Exam.ExamName);
                _ = scoreCard.Replace("$Code$", examResult?.Exam.ExamCode);
                _ = scoreCard.Replace("$Date$", examResult?.Exam.Date.ToString("dd/MM/yyyy"));
                _ = scoreCard.Replace("$Duration$", examResult?.Exam.Duration);
                _ = scoreCard.Replace("$Obtained$", examResult?.Score.ToString());
                _ = scoreCard.Replace("$TotalMarks$", examResult?.Exam.Questions.Sum(x => x.Marks).ToString());
                _ = scoreCard.Replace("$Pass/Fail$", examResult?.Status.ToString());
                _ = scoreCard.Replace("$Teacher$", examResult?.Exam?.Teacher?.Name);

                GlobalSettings globalSettings = new()
                {
                    ColorMode = DinkToPdf.ColorMode.Color,
                    Orientation = DinkToPdf.Orientation.Portrait,
                    PaperSize = DinkToPdf.PaperKind.A3,
                    Margins = new MarginSettings { Top = 5, Right = 1.5, Left = 1.5 },
                    DocumentTitle = $"{examResult?.Student.Name}_{examResult?.Exam.ExamName}_Scorecard",
                };

                ObjectSettings objectSettings = new()
                {
                    PagesCount = true,
                    HtmlContent = scoreCard.ToString(),
                    HeaderSettings = { FontName = "Calibri", FontSize = 5, Line = true },
                    FooterSettings = { FontName = "Calibri", FontSize = 5, Right = "Page [page] of [toPage]", Line = true },
                };

                HtmlToPdfDocument pdf = new()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings },
                };
                return _converter.Convert(pdf);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
