using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private ApplicationDbContext _dbcontext;

        public CoverTypeRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
            _dbcontext = dbContext;
        }
        public void Update(CoverType coverType)
        {
            _dbcontext.CoverTypes.Update(coverType);
        }
    }
}
