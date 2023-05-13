using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Service
{
    public class QuestionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public QuestionService(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<Question> GetQuestionById(int id)
        {
            return await _context.Questions.Include(x => x.Options).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ExamToQuestion>> GetExamsToQuestionByQuestion(int qId)
        {
            var examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == qId);
            return await examToQuestions.ToListAsync();
        }

        public async Task<List<Question>> GetQuestions()
        {
            var questions = _context.Questions.Include(x => x.Options);
            foreach (var item in questions)
            {
                var examToQuestions = _context.ExamToQuestions.Where(x => x.QuestionId == item.Id).Select(x => x.Exam);
                item.Exams.AddRange(examToQuestions);
            }
            return await questions.ToListAsync();
        }

        public int SaveQuestion(Question q)
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

        public int IncludeQuestionInExam(ExamToQuestion examToQuestion)
        {
            if (examToQuestion.Id > 0)
            {
                _context.ExamToQuestions.Update(examToQuestion);
            }
            else
            {
                _context.ExamToQuestions.Add(examToQuestion);
            }
            return _context.SaveChanges();
        }

        public async Task<int> DeleteQ(int id)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
