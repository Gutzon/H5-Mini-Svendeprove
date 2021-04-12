using Microsoft.EntityFrameworkCore;
using ServerSideData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideData
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) 
            :base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
