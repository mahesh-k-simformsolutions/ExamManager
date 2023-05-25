using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ExamManagementSystem.Service
{
    public class CommonService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CommonService> _logger;
        public CommonService(UserManager<User> userManager, ILogger<CommonService> logger)
        {
            _userManager = userManager;
            _logger = logger;
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
    }
}
