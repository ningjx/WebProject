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
            builder.Services.AddSingleton<IConfigService, ConfigService>();
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
            //������С��־����
            .MinimumLevel.Information()
            //����־д���ļ�
            .WriteTo.File($"logs/{DateTime.Now:yyyy-MM-dd}_log.txt",
                outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",  // ���������ʽ����ʾ��ϸ�쳣��Ϣ
                rollingInterval: RollingInterval.Day, //��־���챣��
                rollOnFileSizeLimit: true,            // ���Ƶ����ļ�����󳤶�
                fileSizeLimitBytes: 102400,        // �����ļ���󳤶�100K
                encoding: Encoding.UTF8,              // �ļ��ַ�����
                retainedFileCountLimit: 10            // ��󱣴��ļ���,��������ļ������Զ�����ԭ���ļ�
            )
            .WriteTo.File($"logs/{DateTime.Now:yyyy-MM-dd}_error_log.txt",
                outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 102400,
                encoding: Encoding.UTF8,
                retainedFileCountLimit: 10,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
            )
            .CreateLogger();

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