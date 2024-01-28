using JustMySocksService.Interfaces;
using JustMySocksService.Services;
using Serilog;
using System.Text;

namespace WebProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddSerilog();
            builder.Services.AddSingleton<IConfigService,ConfigService>();
            builder.Services.AddSingleton<ISubscribeConverterService, SubscribeConverterService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            Log.Logger = new LoggerConfiguration()
            //设置最小日志级别
            .MinimumLevel.Information()
            //将日志写到文件
            .WriteTo.File($"logs/{DateTime.Now:yyyy-MM-dd}/log.txt", //日志按照天为单位创建文件夹
                outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",  // 设置输出格式，显示详细异常信息
                rollingInterval: RollingInterval.Day, //日志按天保存
                rollOnFileSizeLimit: true,            // 限制单个文件的最大长度
                fileSizeLimitBytes: 100 * 1024,        // 单个文件最大长度10K
                encoding: Encoding.UTF8,              // 文件字符编码
                retainedFileCountLimit: 10            // 最大保存文件数,超过最大文件数会自动覆盖原有文件
            ).CreateLogger();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            //app.UseEndpoints(endpoints =>
            //{
            //    //endpoints.MapControllers();
            //    endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}");
            //});
            app.MapControllers();
            app.Run();
        }
    }
}