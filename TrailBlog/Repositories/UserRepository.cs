using TrailBlog.Data;
using TrailBlog.Entities;

namespace TrailBlog.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) {}
    }
}
