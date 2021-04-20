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
        public DbSet<User_Corp_Permission> UCP { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Konti> Kontis { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Corporation> Corporations{ get; set; }
        public DbSet<FinanceEntry> FinanceEntries { get; set; }
        public DbSet<RepFinanceEntry> RepFinanceEntries { get; set; }


    }
}
