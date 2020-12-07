using Exception = System.Exception;
using Random = System.Random;
using System.IO;
using Microsoft.Extensions.Configuration;
using ReadInSampleFile126.Utilities;
using ReadInSampleFile126.DAL;

namespace ReadInSampleFile126
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json")
                .Build();

            var targetFile = config.GetSection("InputFile1").Get<AppConfiguration.TargetFile>();
            
            ReadInputFile126(targetFile.path, targetFile.filename);
        }

        private static void ReadInputFile126( string path, string filename)
        {
            var sql = new SQLDataAccess();
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            StreamReader file =
                new StreamReader(path + filename);
            while ((line = file.ReadLine()) != null)
            {
                sql.InsertAuditDb(line);

                counter++;
                if (counter == 1)
                    ProcessHeader(sql, line);
                else
                {
                    if (string.Compare("003E", line.Substring(0, 4)) == 0)
                    {
                        break;
                    }
                    ProcessBodyRecord(sql, line);
                }
            }

            ProcessTrailer(sql, line);

            file.Close();
        }

        private static void ProcessBodyRecord(SQLDataAccess sql, string line)
        {
            //006E9155795930017 03    JJB   1L   274858       2003  401300575\
            var staticData1 = line.Substring(0, 4);
            var accountNumber = line.Substring(4, 9);
            var nameCode = line.Substring(24, 3);
            var opCode = line.Substring(30, 1);
            var loanCode = line.Substring(31, 1);
            var amount = line.Substring(35, 6);
            var fileDate = line.Substring(48, 4);
            var fileNumber = line.Substring(54, 9);
            sql.InsertBody(accountNumber, nameCode, opCode, loanCode,
                amount, fileDate, fileNumber);
            return;
        }

        private static void ProcessTrailer(SQLDataAccess sql, string line)
        {
            //003E    2001103//2  00001{        \
            var fileDate = line.Substring(8, 5);
            var fileNRecords = line.Substring(20, 6);

            sql.InsertTrailer(fileDate, fileNRecords);
            return;
        }

        private static void ProcessHeader(SQLDataAccess sql, string line)
        {
            //008E    2001103**2 \
            var staticData1 = line.Substring(0, 4);
            var fileDate = line.Substring(8, 5);
            var staticData2 = line.Substring(13, 2);

            sql.InsertHeader(fileDate);

            return;
        }

        private static char GetSignedNumeric(int input)
        {
            var cRet = '{';
            switch (input)
            {
                case 1:
                    cRet = 'A';
                    break;
                case 2:
                    cRet = 'B';
                    break;
                case 3:
                    cRet = 'C';
                    break;
                case 4:
                    cRet = 'D';
                    break;
                case 5:
                    cRet = 'E';
                    break;
                case 6:
                    cRet = 'F';
                    break;
                case 7:
                    cRet = 'G';
                    break;
                case 8:
                    cRet = 'H';
                    break;
                case 9:
                    cRet = 'I';
                    break;
                default:
                    cRet = '{';
                    break;
            }
            return cRet;
        }
    }
}

/*
 *
 * public static class Program
{
    private static IConfigurationRoot Configuration { get; set; }

    public static void Main()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
        Configuration = builder.Build();

        IServiceCollection services = new ServiceCollection();
        services.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IMyService, MyService>();
        IServiceProvider provider = services.BuildServiceProvider();

        IMyService myService = provider.GetService<IMyService>();
        myService.SomeMethod();
    }

    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            IConfigurationRoot configuration = configBuilder.Build();
            DbContextOptionsBuilder<MyDbContext> builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new MyDbContext(builder.Options);
        }
    }
 *
 *
 */
