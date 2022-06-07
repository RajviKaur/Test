using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly DataContext _dbContext;

        public DbInitializer(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async void Initialize()
        {
            try
            {
                //Apply Migration - if not applied 
                if (_dbContext.Database.GetPendingMigrations().Any())
                
                    _dbContext.Database.Migrate();
                
            }
            catch(Exception ex){

            }

            //Create Seeding data
          await Seed.SeedData(_dbContext);
        }
    }
}