using Microsoft.EntityFrameworkCore;
using CalenderSystem.Domain.Entities;
using CalenderSystem.Infrastructure.Database;
using CalenderSystem.Infrastructure.Generic;
using CalenderSystem.Infrastructure.Repositories.EventRepositories;

namespace CalenderSystem.Infrastructure.Repositories.EventRepository
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {

        #region CTOR

        private readonly AppDbContext _db;
        public EventRepository(AppDbContext _appDbContext) : base(_appDbContext)
        {
            _db = _appDbContext;
        }
        #endregion

    }
}
