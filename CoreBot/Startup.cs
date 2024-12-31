// Generated with CoreBot .NET Template version v4.22.0

using CoreBot.Bots;
using CoreBot.Dialogs;
using CoreBot.Models;
using CoreBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreBot;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient().AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth; });

        services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

        services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

        services.AddSingleton<IStorage, MemoryStorage>();

        services.AddSingleton<UserState>();

        services.AddSingleton<ConversationState>();

        services.AddSingleton<MedichatCluRecognizer>();

        services.AddSingleton<MainDialog>();
        services.AddSingleton<PlaceAppointmentDialog>();
        services.AddSingleton<ViewAppointmentsDialog>();
        services.AddSingleton<ViewDoctorsDialog>();
        services.AddSingleton<RequestPrescriptionDialog>();
        services.AddSingleton<ViewPrescriptionsDialog>();

        services.AddSingleton<ApiService>();

        services.AddSingleton<AppointmentDataService>();
        services.AddSingleton<DoctorDataService>();
        services.AddSingleton<MedicineDataService>();
        services.AddSingleton<PrescriptionDataService>();

        services.AddTransient<IBot, DialogBot<MainDialog>>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseWebSockets()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}