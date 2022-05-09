using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VueStart.Services;

public class UserService
{
    private readonly ApplicationDbContext dbContext;

    public UserService(ApplicationDbContext dbContext){
        this.dbContext = dbContext;
    }

    public User Authenticate(string username, string password)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
            return null;
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        switch (result)
        {
            case PasswordVerificationResult.Failed:
                return null;
            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = passwordHasher.HashPassword(user, password);
                dbContext.SaveChanges();
                break;
        }

        return user;
    }

    internal void SetPassword(User user, string newPassword)
    {
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
        dbContext.SaveChanges();
    }
}
