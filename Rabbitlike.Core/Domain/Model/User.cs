using Microsoft.AspNetCore.Identity;

namespace Rabbitlike.Core.Domain.Model
{
    public class User : IdentityUser<Guid>
    {
        public int? RedmineId { get; set; }
    }
}
