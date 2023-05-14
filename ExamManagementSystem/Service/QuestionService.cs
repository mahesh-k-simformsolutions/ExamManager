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
                return await _context.Questions.Include(x => x.Options).FirstOrDefaultAsync(x => x.Id == id);
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
                return await _context.Questions.Include(q => q.Options).ToListAsync();
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
                var examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == qId);
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
                var questions = _context.Questions.Include(x => x.Options);
                foreach (var item in questions)
                {
                    var examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == item.Id).Select(x => x.Exam);
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
                if (q.Id > 0)
                {
                    _context.Questions.Update(q);
                }
                else
                {
                    _context.Questions.Add(q);
                }
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

                foreach (var examToQuestion in examToQuestions)
                {
                    if (examToQuestion.Id > 0)
                    {
                        _context.ExamToQuestions.Update(examToQuestion);
                    }
                    else
                    {
                        _context.ExamToQuestions.Add(examToQuestion);
                    }
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
                var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
                if (question != null)
                {
                    _context.Questions.Remove(question);
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
