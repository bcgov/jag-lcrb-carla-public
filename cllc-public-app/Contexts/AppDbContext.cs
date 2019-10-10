using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    /// <summary>
    /// Database Context Factory Interface
    /// </summary>
    public interface IAppDbContextFactory
    {
        /// <summary>
        /// Create new database context
        /// </summary>
        /// <returns></returns>
        AppDbContext Create();
    }

    /// <summary>
    /// Database Context Factory
    /// </summary>
    public class DbAppContextFactory : IAppDbContextFactory
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Database Context Factory Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="options"></param>
        public DbAppContextFactory(IHttpContextAccessor httpContextAccessor, DbContextOptions<AppDbContext> options)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Create new database context
        /// </summary>
        /// <returns></returns>
        public AppDbContext Create()
        {
            return new AppDbContext(_httpContextAccessor, _options);
        }
    }


    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AppDbContext(IHttpContextAccessor httpContextAccessor, DbContextOptions<AppDbContext> options) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;

            // override the default timeout as some operations are time intensive
            Database?.SetCommandTimeout(180);
        }

        public AppDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables()
                   .Build();
                string connectionString = DatabaseTools.GetConnectionString(configuration);
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<Jurisdiction> Jurisdictions { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<PostSurveyResult> PostSurveyResults { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<VoteOption> VoteOptions { get; set; }

        public DbSet<VoteQuestion> VoteQuestions { get; set; }

        public DbSet<Newsletter> Newsletters { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }

        public DbSet<PolicyDocument> PolicyDocuments { get; set; }
    }
}
