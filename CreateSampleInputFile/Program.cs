using System;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CreateSampleInputFile
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json");

            var config = builder.Build();
            var author = config.GetSection("author").Get<AppConfiguration.Person>();
            var connStr = config.GetSection("Connections").Get<AppConfiguration.Connections>();
            var targetFile = config.GetSection("DestinationFile1").Get<AppConfiguration.TargetFile>();

            //Console.WriteLine("Hello World!");

            CreateSampleInputFile126(targetFile.path, targetFile.filename, 10);

        }

        private static void CreateSampleInputFile126(string path, string filename, int numLines)
        {
            if(File.Exists(path + filename))
            {
                File.Delete(path + filename);
            }
            // file header
            //008E    yymmm03**2 \
            using (StreamWriter c = File.AppendText(path + filename))
            {
                c.WriteLine(@"008E    2001103**2 \");
            }

            // file Body
            try
            {
                Random r = new Random();
                using (StreamWriter w = File.AppendText(path + filename))
                {
                    for (int iteration = 0; iteration < numLines; iteration++)
                    {
                        //{0,04} outputs a number "   2"  taking up 4 spaces
                        // zero fill number: {0,5:d4} " 0003", 5 characters total, decimal 4 zero filled
                        w.WriteLine("006E{0,9:D9}0017 03    JJB   1L   {1,6:D6}       20{2,2:D2}  {3,9:D9}\\",
                            r.Next(999999999), // account number
                            r.Next(999999), // deduction 
                            r.Next(1, 12),
                            r.Next(999999999) // file number
                        );
                    }

                }
            }
            catch (Exception ex)
            {

            }

            //TRAILER
            // 003E    YYMMM03//2  000000        \
            // THE 000000 IS A SIGNED NUMERIC COUNT OF THE NUMBER OF DETAIL RECORDS (NUMLINES)
            // 00587{ = 5870, {=0, A=1, B=2, C=3, D=4, E=5, F=6, G=7, H=8, I=9
            var i = numLines / 10;
            var j = numLines % 10;
            try
            {
                using (StreamWriter x = File.AppendText(path + filename))
                {
                    x.WriteLine(@"003E    2001103//2  {0,5:D5}{1,1:C1}        \",
                        i,
                        GetSignedNumeric(j));
                }
            }
            catch (Exception ex)
            {

            }
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
