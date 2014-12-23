using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using XboxChaosApi.Models;

namespace XboxChaosApi.Database
{
	public class DatabaseContext : DbContext
	{

		public DbSet<Release> Releases { get; set; }
	}
}