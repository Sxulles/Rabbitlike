using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Rabbitlike.Core.Domain.Model;

namespace Rabbitlike.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
    }
}
