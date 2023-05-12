using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamManagementSystem.Service
{
    public class ExamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public ExamService(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
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
                var exam = await _context.Exams.Include(x=>x.Teacher).FirstOrDefaultAsync(x => x.Id == id);
                exam.Questions = await _context.ExamToQuestions.Include(x => x.Question).ThenInclude(x=>x.Options).Where(x => x.ExamId == id).Select(x=>x.Question).ToListAsync();
                return exam;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<ExamToStudent>> GetExamsToStudentByExam(int examId)
        {
            var examToStudents = _context.ExamToStudents.Where(x => x.ExamId == examId);
            return await examToStudents.ToListAsync();
        }

        public async Task<List<Exam>> GetExams()
        {
            var exams = _context.Exams.Include(x => x.Teacher).OrderBy(x => x.ExamStatus).ThenByDescending(x=>x.Date);
            foreach (var item in exams)
            {
                var examToQuestions = _context.ExamToQuestions.Where(x => x.ExamId == item.Id).Select(x => x.Question);
                item.Questions = await examToQuestions.ToListAsync();
            }
            return await exams.ToListAsync();
        }

        public async Task<List<Exam>> MyExams()
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

        public async Task<int> SaveExam(Exam exam)
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

        public async Task<int> AssignExamToStudent(List<ExamToStudent> examToStudent)
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

        public async Task<int> DeleteExam(int id)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<List<ExamResult>> GetResults()
        {
            return await _context.ExamResults
                .Include(er => er.Exam).ThenInclude(e => e.Teacher)
                .Include(er => er.Exam).ThenInclude(e => e.Results)
                .Include(er => er.Student).ToListAsync();
        }
    }
}
