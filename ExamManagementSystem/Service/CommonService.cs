using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace ExamManagementSystem.Service
{
    public class CommonService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CommonService> _logger;
        private readonly ApplicationDbContext _context;
        public CommonService(UserManager<User> userManager, ILogger<CommonService> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<List<User>> LoadStudents()
        {
            try
            {
                IList<User> students = await _userManager.GetUsersInRoleAsync(EnumUserRole.Student.ToString());
                return students.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<User>> LoadTeachers()
        {
            try
            {
                IList<User> teachers = await _userManager.GetUsersInRoleAsync(EnumUserRole.Teacher.ToString());
                return teachers.ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<Exam>> LoadExams(params int[] exclude)
        {
            try
            {
                var exams = await _context.Exams.Include(e => e.ExamToQuestions).ThenInclude(eToq => eToq.Question).ThenInclude(q => q.Options).ToListAsync();
                if (exclude.Length > 0)
                {
                    exams = exams.Where(x => !exclude.Contains(x.Id)).ToList();
                }
                return exams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task VerifyUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.Verified = true;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
