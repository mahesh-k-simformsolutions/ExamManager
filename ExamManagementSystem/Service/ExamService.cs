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

                throw;
            }
        }

        public async Task<Exam> AppearExam(int id)
        {
            try
            {
                var userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var exam = await _context.Exams.Include(x => x.Teacher).FirstOrDefaultAsync(x => x.Id == id);
                exam.Questions = await _context.ExamToQuestions.Include(x => x.Question).ThenInclude(x => x.Options).Where(x => x.ExamId == id).Select(x => x.Question).ToListAsync();
                exam.IsAppearedByCurrentStudent = _context.ExamResults.Any(x => x.StudentId == userId && x.ExamId == id);
                return exam;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task SubmitExam(int examId, List<Answer> answers)
        {
            try
            {
                var studentId = answers.FirstOrDefault()?.StudentId;
                await _context.Answers.AddRangeAsync(answers);

                // calculate score
                var score = 0F;
                foreach (var item in answers)
                {
                    if(item.Option.IsCorrect)
                    {
                        score += item.Question.Marks;
                    }
                }
                var totalMarks = answers.Select(x => x.Question).Sum(x => x.Marks);
                var result = new ExamResult()
                {
                    StudentId = studentId,
                    ExamId = examId,
                    Score = score,
                    Status = ((100 * score) / totalMarks) >= 33 ? Enums.ExamResultStatus.Pass : Enums.ExamResultStatus.Fail
                };
                _context.ExamResults.Add(result);

                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {

            }
        }
        public async Task<List<ExamToStudent>> GetExamsToStudentByExam(int examId)
        {
            var examToStudents = _context.ExamToStudents.Where(x => x.ExamId == examId);
            return await examToStudents.ToListAsync();
        }

        public async Task<List<Exam>> GetExams()
        {
            try
            {
                var exams = _context.Exams.Include(x => x.Teacher).OrderBy(x => x.ExamStatus).ThenByDescending(x => x.Date);
                foreach (var item in exams)
                {
                    var examToQuestions = _context.ExamToQuestions.Where(x => x.ExamId == item.Id).Select(x => x.Question);
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
                var userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var exams = _context.ExamToStudents.Where(x => x.StudentId == userId)
                                    .Include(e => e.Exam)
                                    .ThenInclude(e => e.Teacher).Select(x => x.Exam);

                foreach (var item in exams)
                {
                    var examToQuestions = _context.ExamToQuestions.Where(x => x.ExamId == item.Id).Select(x => x.Question);
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
                if (exam.Id > 0)
                {
                    _context.Exams.Update(exam);
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
                if (examToStudent.Count > 0)
                {

                    var examId = examToStudent.FirstOrDefault()!.ExamId;
                    var inputStudentIds = examToStudent.Select(x => x.StudentId);

                    var existing = _context.ExamToStudents.Where(x => x.ExamId == examId);
                    var existingStudentIds = _context.ExamToStudents.Where(x => x.ExamId == examId).Select(x => x.StudentId);

                    var newStudents = inputStudentIds.Except(existingStudentIds);
                    var deleting = existingStudentIds.AsEnumerable().Except(inputStudentIds);

                    foreach (var item in examToStudent.Where(x => newStudents.Contains(x.StudentId)))
                    {
                        _context.ExamToStudents.Add(item);
                    }

                    foreach (var item in existing.Where(eTos => deleting.Contains(eTos.StudentId)))
                    {
                        _context.ExamToStudents.Remove(item);
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
                var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id);
                if (exam != null)
                {
                    _context.Exams.Remove(exam);
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
                var userId = _contextAccessor.HttpContext!.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
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
                var examResult = await _context.ExamResults
                    .Include(er => er.Exam)
                    .Include(er => er.Student)
                    .FirstOrDefaultAsync(x => x.Id == examResultId);
                var questions = _context.ExamToQuestions.Where(x => x.ExamId == examResult.ExamId).Include(x => x.Question).Select(x => x.Question);
                examResult.Exam.Questions = await questions.ToListAsync();
                
                StringBuilder scoreCard = new StringBuilder(File.ReadAllText(@"./Helpers/Template/Scorecard.html"));
                scoreCard.Replace("$StudentName$", examResult?.Student.Name);
                scoreCard.Replace("$Email$", examResult?.Student.Email);
                scoreCard.Replace("$Date$", examResult?.Exam.Date.ToString("dd/MM/yyyy"));
                scoreCard.Replace("$Time$", examResult?.Exam.StartTime.ToString("h:mm tt"));
                scoreCard.Replace("$Exam$", examResult?.Exam.ExamName);
                scoreCard.Replace("$Code$", examResult?.Exam.ExamCode);
                scoreCard.Replace("$Date$", examResult?.Exam.Date.ToString("dd/MM/yyyy"));
                scoreCard.Replace("$Duration$", examResult?.Exam.Duration);
                scoreCard.Replace("$Obtained$", examResult?.Score.ToString());
                scoreCard.Replace("$TotalMarks$", examResult?.Exam.Questions.Sum(x => x.Marks).ToString());
                scoreCard.Replace("$Pass/Fail$", examResult?.Status.ToString());
                scoreCard.Replace("$Teacher$", examResult?.Exam?.Teacher?.Name);

                var globalSettings = new GlobalSettings
                {
                    ColorMode = DinkToPdf.ColorMode.Color,
                    Orientation = DinkToPdf.Orientation.Portrait,
                    PaperSize = DinkToPdf.PaperKind.A3,
                    Margins = new MarginSettings { Top = 5, Right = 1.5, Left = 1.5 },
                    DocumentTitle = $"{examResult?.Student.Name}_{examResult?.Exam.ExamName}_Scorecard",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = scoreCard.ToString(),
                    HeaderSettings = { FontName = "Calibri", FontSize = 5, Line = true },
                    FooterSettings = { FontName = "Calibri", FontSize = 5, Right = "Page [page] of [toPage]", Line = true },
                };

                var pdf = new HtmlToPdfDocument()
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
