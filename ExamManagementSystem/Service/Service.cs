using ExamManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Service
{
    public class Service
    {
        private readonly ApplicationDbContext _context;

        public Service(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Exam
        public async Task<Exam> GetExamById(int id)
        {
            return await _context.Exams.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Exam>> GetExams()
        {
            return await _context.Exams.ToListAsync();
        }
        public List<Exam> GetExamsByStudent(string studentId)
        {
            return _context.ExamToStudents.Where(x => x.StudentId == studentId).Select(x => x.Exam).ToList();
        }
        public int SaveExam(Exam exam)
        {
            if (exam.Id > 0)
            {
                _context.Exams.Update(exam);
            }
            else
            {
                _context.Exams.Add(exam);
            }
                return _context.SaveChanges();
        }

        #endregion

        public async Task<List<Question>> GetQuestions()
        {
            return await _context.Questions.Include(x=>x.Options).ToListAsync();
        }
        public int SaveQuestion(Question q)
        {
            if (q.QuestionId > 0)
            {
                _context.Questions.Update(q);
            }
            else
            {
                _context.Questions.Add(q);
            }
            return _context.SaveChanges();
        }

    }
}
