﻿using ExamManagementSystem.Data;
using ExamManagementSystem.Data.DbContext;
using ExamManagementSystem.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamManagementSystem.Service
{
    public class CommonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        public CommonService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<List<User>> LoadStudents()
        {
            try
            {
                var students = await _userManager.GetUsersInRoleAsync(EnumUserRole.Student.ToString());
                return students.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<User>> LoadTeachers()
        {
            var teachers = await _userManager.GetUsersInRoleAsync(EnumUserRole.Teacher.ToString());
            return teachers.ToList();
        }

        public async Task<List<Exam>> LoadExams()
        {
            return await _context.Exams.ToListAsync();
        }
    }
}