using ClassLibrary1.Data;
using Microsoft.AspNetCore.Mvc;

namespace ProjectMVC.Models
{
    public class LoginModel
    {
        private readonly HospitalDbContext _context;

        public LoginModel(HospitalDbContext context)
        {
            _context = context;
        }
    }
}
