using System;
using System.Linq;
using System.Windows;
using CarRentalApplication.Data;
using System.Security.Cryptography;
using System.Text;

namespace CarRentalApplication
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ResetAdminPassword(); //  Run password reset when the app starts
        }

        private void ResetAdminPassword()
        {
            using (var db = new AppDbContext())
            {
                var admin = db.Users.FirstOrDefault(u => u.Email == "admin@carrental.com");
                if (admin != null)
                {
                    admin.Password = HashPassword("admin123"); //  Reset password to hashed format
                    db.SaveChanges();
                  
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
