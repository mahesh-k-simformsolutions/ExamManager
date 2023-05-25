using DinkToPdf;
using DinkToPdf.Contracts;
using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace ExamManagementSystem.Service
{
    public class ResultService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConverter _converter;
        private readonly ILogger<ResultService> _logger;
        public ResultService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IConverter converter, ILogger<ResultService> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _converter = converter;
            _logger = logger;
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
                    .Include(er => er.Exam).ThenInclude(e => e.ExamToQuestions).ThenInclude(eToq => eToq.Question)
                    .Include(er => er.Exam).ThenInclude(e => e.Teacher)
                    .Include(er => er.Student)
                    .FirstOrDefaultAsync(x => x.Id == examResultId);

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
                _ = scoreCard.Replace("$TotalMarks$", examResult?.Exam.ExamToQuestions.Select(eToq => eToq.Question).Sum(q => q.Marks).ToString());
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
