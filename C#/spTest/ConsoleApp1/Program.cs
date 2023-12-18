using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        var host = new HostBuilder()
          .ConfigureHostConfiguration(configHost =>
          {
              configHost.SetBasePath(AppContext.BaseDirectory);
              configHost.AddJsonFile("appsettings.json");
          })
          .ConfigureAppConfiguration((hostContext, configApp) =>
          {
              // 預設情況下，已經包含了 appsettings.json 的設定
              // 這裡可以添加額外的設定方式，如命令列參數、環境變數等
          })
          .ConfigureServices((hostContext, services) =>
          {
              services.AddSingleton<IConfiguration>(hostContext.Configuration);

              var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

              //services.AddDbContext<testSPContext>(options => options.UseSqlServer(connectionString));
          })
          .Build();

        //dotnet ef dbcontext scaffold "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -o ..\DataAccess\Models 


    }
}
