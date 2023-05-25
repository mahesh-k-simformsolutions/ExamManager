using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Service
{
    public class QuestionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuestionService> _logger;
        public QuestionService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, ILogger<QuestionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Question> GetQuestionById(int id)
        {
            try
            {
                return await _context.Questions.Include(x => x.Options).FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new Exception("Question not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<Question>> LoadQuestionsByExam(int examId)
        {
            try
            {
                return await _context.ExamToQuestions.Include(x => x.Question).ThenInclude(x => x.Options).Where(x => x.ExamId == examId).Select(x => x.Question).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<ExamToQuestion>> GetExamsToQuestionByQuestion(int qId)
        {
            try
            {
                IQueryable<ExamToQuestion> examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == qId);
                return await examToQuestions.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<Question>> GetQuestions()
        {
            try
            {
                Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Question, ICollection<Option>> questions = _context.Questions.Include(x => x.Options);
                foreach (Question item in questions)
                {
                    IQueryable<Exam> examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == item.Id).Select(x => x.Exam);
                    item.Exams.AddRange(examToQuestions);
                }
                return await questions.ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public int SaveQuestion(Question q)
        {
            try
            {
                _ = q.Id > 0 ? _context.Questions.Update(q) : _context.Questions.Add(q);
                return _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public int IncludeQuestionInExam(List<ExamToQuestion> examToQuestions)
        {
            try
            {

                foreach (ExamToQuestion examToQuestion in examToQuestions)
                {
                    _ = examToQuestion.Id > 0 ? _context.ExamToQuestions.Update(examToQuestion) : _context.ExamToQuestions.Add(examToQuestion);
                }
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> DeleteQ(int id)
        {
            try
            {
                Question? question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
                if (question != null)
                {
                    _ = _context.Questions.Remove(question);
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
