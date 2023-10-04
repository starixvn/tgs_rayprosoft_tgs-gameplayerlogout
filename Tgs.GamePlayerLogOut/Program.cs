using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GamePlayerLogOut;
using Tgs.GamePlayerLogOut.Core.Extensions;
using Tgs.GamePlayerLogOut.GrpcServices;
using Tgs.GamePlayerLogOut.MessageHandlers;
using Tgs.GameProviders.Domains;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.
builder.Services.AddSingleton<DiagnosticsInterceptorOptions>();
builder.Services.AddSingleton<LoggingInterceptor>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();
    //If true, detailed exception messages are returned to clients when an exception is thrown in a service method.
    //The default is false.
    //Setting EnableDetailedErrors to true can leak sensitive information.
    options.EnableDetailedErrors = true;
});
// Add services to the container.
builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames)
    {
        c.IncludeXmlComments(xmlFileName);
    }
});

builder.Services.AddGamePlayerLogOutCore((options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.BaseUrl = configuration.GetValue<Uri>("GamePlayerLogOutSettings:GameProvidersClientBaseUrl")!;
});

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(natsConfig =>
    {
        _ = natsConfig
        .ConfigQueueOptions((options, sp) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            options.Url = configuration.GetConnectionString("Nats")!;
        })
        .AddHandler<KYMessageHandler>("gp.player.logout")
        .AddHandler<KYMessageHandler>(GameProviderType.Kaiyuan.SubjectToString(), GameProviderType.Kaiyuan.SubjectToString())
        .AddHandler<VRMessageHandler>("gp.player.logout")
        .AddHandler<VRMessageHandler>(GameProviderType.VR.SubjectToString(), GameProviderType.VR.SubjectToString())
        .AddHandler<WGViaGamingMessageHandler>("gp.player.logout")
        .AddHandler<WGViaGamingMessageHandler>(GameProviderType.WGViaGaming.SubjectToString(), GameProviderType.WGViaGaming.SubjectToString())
        .AddHandler<WGFmGamingMessageHandler>("gp.player.logout")
        .AddHandler<WGFmGamingMessageHandler>(GameProviderType.WGFmGaming.SubjectToString(), GameProviderType.WGFmGaming.SubjectToString())
        .AddHandler<BigGamingMessageHandler>("gp.player.logout")
        .AddHandler<BigGamingMessageHandler>(GameProviderType.BigGaming.SubjectToString(), GameProviderType.BigGaming.SubjectToString())
        .AddHandler<TCGMessageHandler>("gp.player.logout")
        .AddHandler<TCGMessageHandler>(GameProviderType.TCG.SubjectToString(), GameProviderType.TCG.SubjectToString())
        .AddHandler<LEGamingMessageHandler>("gp.player.logout")
        .AddHandler<LEGamingMessageHandler>(GameProviderType.LEGaming.SubjectToString(), GameProviderType.LEGaming.SubjectToString())
        .AddHandler<SBOMessageHandler>("gp.player.logout")
        .AddHandler<SBOMessageHandler>(GameProviderType.SBOBET.SubjectToString(), GameProviderType.SBOBET.SubjectToString())
        .AddHandler<WMCasinoMessageHandler>("gp.player.logout")
        .AddHandler<WMCasinoMessageHandler>(GameProviderType.WMCasino.SubjectToString(), GameProviderType.WMCasino.SubjectToString())
        .AddHandler<PragmaticPlayMessageHandler>("gp.player.logout")
        .AddHandler<PragmaticPlayMessageHandler>(GameProviderType.PragmaticPlay.SubjectToString(), GameProviderType.PragmaticPlay.SubjectToString())
        .AddHandler<SabaMessageHandler>("gp.player.logout")
        .AddHandler<SabaMessageHandler>(GameProviderType.Saba.SubjectToString(), GameProviderType.Saba.SubjectToString())
        .AddHandler<AviaMessageHandler>("gp.player.logout")
        .AddHandler<AviaMessageHandler>(GameProviderType.AVIAGaming.SubjectToString(), GameProviderType.AVIAGaming.SubjectToString())
        .AddHandler<JiliMessageHandler>("gp.player.logout")
        .AddHandler<JiliMessageHandler>(GameProviderType.Jili.SubjectToString(), GameProviderType.Jili.SubjectToString())
        .AddHandler<TFGamingMessageHandler>("gp.player.logout")
        .AddHandler<TFGamingMessageHandler>(GameProviderType.TFGaming.SubjectToString(), GameProviderType.TFGaming.SubjectToString())
        .AddHandler<AllBetMessageHandler>("gp.player.logout")
        .AddHandler<AllBetMessageHandler>(GameProviderType.AllBet.SubjectToString(), GameProviderType.AllBet.SubjectToString())
        .AddHandler<HabaneroMessageHandler>("gp.player.logout")
        .AddHandler<HabaneroMessageHandler>(GameProviderType.Habanero.SubjectToString(), GameProviderType.Habanero.SubjectToString())
        .AddHandler<Card365MessageHandler>("gp.player.logout")
        .AddHandler<Card365MessageHandler>(GameProviderType.Card365.SubjectToString(), GameProviderType.Card365.SubjectToString())
        .AddHandler<JDBMessageHandler>("gp.player.logout")
        .AddHandler<JDBMessageHandler>(GameProviderType.JDB.SubjectToString(), GameProviderType.JDB.SubjectToString())
        .AddHandler<DreamGameMessageHandler>("gp.player.logout")
        .AddHandler<DreamGameMessageHandler>(GameProviderType.DreamGame.SubjectToString(), GameProviderType.DreamGame.SubjectToString())
        .AddHandler<JokerMessageHandler>("gp.player.logout")
        .AddHandler<JokerMessageHandler>(GameProviderType.Joker.SubjectToString(), $"gp.player.logout.{GameProviderType.Joker.SubjectToString()}")
        .AddHandler<PragmaticPlayTransferMessageHandler>("gp.player.logout")
        .AddHandler<PragmaticPlayTransferMessageHandler>(GameProviderType.PragmaticPlayTransfer.SubjectToString(), GameProviderType.PragmaticPlayTransfer.SubjectToString())
        .AddHandler<AwcAelottoMessageHandler>("gp.player.logout")
        .AddHandler<AwcAelottoMessageHandler>(GameProviderType.AWC_AELOTTO.SubjectToString(), GameProviderType.AWC_AELOTTO.SubjectToString())
        .AddHandler<AwcAwsMessageHandler>("gp.player.logout")
        .AddHandler<AwcAwsMessageHandler>(GameProviderType.AWC_AWS.SubjectToString(), GameProviderType.AWC_AWS.SubjectToString())
        .AddHandler<AwcCockfightMessageHandler>("gp.player.logout")
        .AddHandler<AwcCockfightMessageHandler>(GameProviderType.AWC_COCKFIGHT.SubjectToString(), GameProviderType.AWC_COCKFIGHT.SubjectToString())
        .AddHandler<AwcFacaiMessageHandler>("gp.player.logout")
        .AddHandler<AwcFacaiMessageHandler>(GameProviderType.AWC_FACAI.SubjectToString(), GameProviderType.AWC_FACAI.SubjectToString())
        .AddHandler<AwcFsMessageHandler>("gp.player.logout")
        .AddHandler<AwcFsMessageHandler>(GameProviderType.AWC_FS.SubjectToString(), GameProviderType.AWC_FS.SubjectToString())
        .AddHandler<AwcKmMessageHandler>("gp.player.logout")
        .AddHandler<AwcKmMessageHandler>(GameProviderType.AWC_KM.SubjectToString(), GameProviderType.AWC_KM.SubjectToString())
        .AddHandler<AwcLudoMessageHandler>("gp.player.logout")
        .AddHandler<AwcLudoMessageHandler>(GameProviderType.AWC_LUDO.SubjectToString(), GameProviderType.AWC_LUDO.SubjectToString())
        .AddHandler<AwcPgsMessageHandler>("gp.player.logout")
        .AddHandler<AwcPgsMessageHandler>(GameProviderType.AWC_PGS.SubjectToString(), GameProviderType.AWC_PGS.SubjectToString())
        .AddHandler<AwcRcbMessageHandler>("gp.player.logout")
        .AddHandler<AwcRcbMessageHandler>(GameProviderType.AWC_RCB.SubjectToString(), GameProviderType.AWC_RCB.SubjectToString())
        .AddHandler<AwcSgMessageHandler>("gp.player.logout")
        .AddHandler<AwcSgMessageHandler>(GameProviderType.AWC_SG.SubjectToString(), GameProviderType.AWC_SG.SubjectToString())
        .AddHandler<AwcSpadeMessageHandler>("gp.player.logout")
        .AddHandler<AwcSpadeMessageHandler>(GameProviderType.AWC_SPADE.SubjectToString(), GameProviderType.AWC_SPADE.SubjectToString())
        .AddHandler<AwcVenusMessageHandler>("gp.player.logout")
        .AddHandler<AwcVenusMessageHandler>(GameProviderType.AWC_VENUS.SubjectToString(), GameProviderType.AWC_VENUS.SubjectToString())
        .AddHandler<AwcYlMessageHandler>("gp.player.logout")
        .AddHandler<AwcYlMessageHandler>(GameProviderType.AWC_YL.SubjectToString(), GameProviderType.AWC_YL.SubjectToString())
        .AddHandler<JiliTransferMessageHandler>("gp.player.logout")
        .AddHandler<JiliTransferMessageHandler>(GameProviderType.JiliTransfer.SubjectToString(), GameProviderType.JiliTransfer.SubjectToString())
        .AddHandler<WMCasinoTransferMessageHandler>("gp.player.logout")
        .AddHandler<WMCasinoTransferMessageHandler>(GameProviderType.WMCasinoTransfer.SubjectToString(), GameProviderType.WMCasinoTransfer.SubjectToString())
        .AddHandler<EvolutionTransferMessageHandler>("gp.player.logout")
        .AddHandler<EvolutionTransferMessageHandler>(GameProviderType.EvolutionTransfer.SubjectToString(), GameProviderType.EvolutionTransfer.SubjectToString())
        .AddHandler<JDBTransferMessageHandler>("gp.player.logout")
        .AddHandler<JDBTransferMessageHandler>(GameProviderType.JDBTransfer.SubjectToString(), GameProviderType.JDBTransfer.SubjectToString())
        .AddHandler<SBOTransferMessageHandler>("gp.player.logout")
        .AddHandler<SBOTransferMessageHandler>(GameProviderType.SBOTransfer.SubjectToString(), GameProviderType.SBOTransfer.SubjectToString())
        .AddHandler<HabaneroTransferMessageHandler>("gp.player.logout")
        .AddHandler<HabaneroTransferMessageHandler>(GameProviderType.HabaneroTransfer.SubjectToString(), GameProviderType.HabaneroTransfer.SubjectToString())
        .AddHandler<SabaTransferMessageHandler>("gp.player.logout")
        .AddHandler<SabaTransferMessageHandler>(GameProviderType.SabaTransfer.SubjectToString(), GameProviderType.SabaTransfer.SubjectToString())
        .AddHandler<TFGamingTransferMessageHandler>("gp.player.logout")
        .AddHandler<TFGamingTransferMessageHandler>(GameProviderType.TFGamingTransfer.SubjectToString(), GameProviderType.TFGamingTransfer.SubjectToString())
        .AddHandler<DreamGamingTransferMessageHandler>("gp.player.logout")
        .AddHandler<DreamGamingTransferMessageHandler>(GameProviderType.DreamGamingTransfer.SubjectToString(), GameProviderType.DreamGamingTransfer.SubjectToString())
        .AddHandler<FachaiMessageHandler>("gp.player.logout")
        .AddHandler<FachaiMessageHandler>(GameProviderType.FaChai.SubjectToString(), GameProviderType.FaChai.SubjectToString())
        .AddHandler<EndorphinaMessageHandler>("gp.player.logout")
        .AddHandler<EndorphinaMessageHandler>(GameProviderType.Endorphina.SubjectToString(), GameProviderType.Endorphina.SubjectToString())
        .AddHandler<OBetMessageHandler>("gp.player.logout")
        .AddHandler<OBetMessageHandler>(GameProviderType.OBet.SubjectToString(), GameProviderType.OBet.SubjectToString())
        .AddHandler<YeeBetMessageHandler>("gp.player.logout")
        .AddHandler<YeeBetMessageHandler>(GameProviderType.YeeBet.SubjectToString(), GameProviderType.YeeBet.SubjectToString())
        .AddHandler<PinnacleMessageHandler>("gp.player.logout")
        .AddHandler<PinnacleMessageHandler>(GameProviderType.Pinnacle.SubjectToString(), GameProviderType.Pinnacle.SubjectToString())
        .AddHandler<WicketCricketMessageHandler>("gp.player.logout")
        .AddHandler<WicketCricketMessageHandler>(GameProviderType.WicketCricket.SubjectToString(), GameProviderType.WicketCricket.SubjectToString());
    })
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange("*");

builder.Services
    .AddOpenTelemetry()
    .WithTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddSource("Valhalla.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddGrpcCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
        .AddOtlpExporter())
    .WithMetrics(meterBuiler => meterBuiler
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
    .AddEnvironmentVariableDetector())
    .AddRuntimeInstrumentation()
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter())
    .Services
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !httpContext.Request.Path.StartsWithSegments("/hangfire")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !new[] { "html", "ico", "png", "jpg" }.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

// Configure the HTTP request pipeline.
app.MapGrpcService<GamePlayerLogOutGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
