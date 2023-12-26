using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GamePlayerLogOut.Core.Extensions;
using Tgs.GamePlayerLogOut.MessageHandlers;
using Tgs.GameProviders.Domains;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddGamePlayerLogOutCore(
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GamePlayerLogOutSettings:GameCommandClientBaseUrl")!;
    },
    (options, sp) =>
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
        .AddHandler<KYMessageHandler>(GameProviderType.Kaiyuan.ToGamePlayerLogOutSubject(), GameProviderType.Kaiyuan.ToGamePlayerLogOutSubject())
        .AddHandler<VRMessageHandler>("gp.player.logout")
        .AddHandler<VRMessageHandler>(GameProviderType.VRGaming.ToGamePlayerLogOutSubject(), GameProviderType.VRGaming.ToGamePlayerLogOutSubject())
        .AddHandler<WGViaGamingMessageHandler>("gp.player.logout")
        .AddHandler<WGViaGamingMessageHandler>(GameProviderType.WGViaGaming.ToGamePlayerLogOutSubject(), GameProviderType.WGViaGaming.ToGamePlayerLogOutSubject())
        .AddHandler<WGFmGamingMessageHandler>("gp.player.logout")
        .AddHandler<WGFmGamingMessageHandler>(GameProviderType.WGFmGaming.ToGamePlayerLogOutSubject(), GameProviderType.WGFmGaming.ToGamePlayerLogOutSubject())
        .AddHandler<BigGamingMessageHandler>("gp.player.logout")
        .AddHandler<BigGamingMessageHandler>(GameProviderType.BigGaming.ToGamePlayerLogOutSubject(), GameProviderType.BigGaming.ToGamePlayerLogOutSubject())
        .AddHandler<TCGMessageHandler>("gp.player.logout")
        .AddHandler<TCGMessageHandler>(GameProviderType.TCG.ToGamePlayerLogOutSubject(), GameProviderType.TCG.ToGamePlayerLogOutSubject())
        .AddHandler<LEGamingMessageHandler>("gp.player.logout")
        .AddHandler<LEGamingMessageHandler>(GameProviderType.LEGaming.ToGamePlayerLogOutSubject(), GameProviderType.LEGaming.ToGamePlayerLogOutSubject())
        .AddHandler<Cards365TransferMessageHandler>("gp.player.logout")
        .AddHandler<Cards365TransferMessageHandler>(GameProviderType.Cards365Transfer.ToGamePlayerLogOutSubject(), GameProviderType.Cards365Transfer.ToGamePlayerLogOutSubject())
        .AddHandler<JokerMessageHandler>("gp.player.logout")
        .AddHandler<JokerMessageHandler>(GameProviderType.Joker.ToGamePlayerLogOutSubject(), $"gp.player.logout.{GameProviderType.Joker.ToGamePlayerLogOutSubject()}")
        .AddHandler<PragmaticPlayTransferMessageHandler>("gp.player.logout")
        .AddHandler<PragmaticPlayTransferMessageHandler>(GameProviderType.PragmaticPlayTransfer.ToGamePlayerLogOutSubject(), GameProviderType.PragmaticPlayTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<AwcCockfightMessageHandler>("gp.player.logout")
        .AddHandler<AwcCockfightMessageHandler>(GameProviderType.AWC_COCKFIGHT.ToGamePlayerLogOutSubject(), GameProviderType.AWC_COCKFIGHT.ToGamePlayerLogOutSubject())
        .AddHandler<AwcFsMessageHandler>("gp.player.logout")
        .AddHandler<AwcFsMessageHandler>(GameProviderType.AWC_FS.ToGamePlayerLogOutSubject(), GameProviderType.AWC_FS.ToGamePlayerLogOutSubject())
        .AddHandler<AwcKmMessageHandler>("gp.player.logout")
        .AddHandler<AwcKmMessageHandler>(GameProviderType.AWC_KM.ToGamePlayerLogOutSubject(), GameProviderType.AWC_KM.ToGamePlayerLogOutSubject())
        .AddHandler<AwcLudoMessageHandler>("gp.player.logout")
        .AddHandler<AwcLudoMessageHandler>(GameProviderType.AWC_LUDO.ToGamePlayerLogOutSubject(), GameProviderType.AWC_LUDO.ToGamePlayerLogOutSubject())
        .AddHandler<AwcPgsMessageHandler>("gp.player.logout")
        .AddHandler<AwcPgsMessageHandler>(GameProviderType.AWC_PGS.ToGamePlayerLogOutSubject(), GameProviderType.AWC_PGS.ToGamePlayerLogOutSubject())
        .AddHandler<AwcRcbMessageHandler>("gp.player.logout")
        .AddHandler<AwcRcbMessageHandler>(GameProviderType.AWC_RCB.ToGamePlayerLogOutSubject(), GameProviderType.AWC_RCB.ToGamePlayerLogOutSubject())
        .AddHandler<AwcSgMessageHandler>("gp.player.logout")
        .AddHandler<AwcSgMessageHandler>(GameProviderType.AWC_SG.ToGamePlayerLogOutSubject(), GameProviderType.AWC_SG.ToGamePlayerLogOutSubject())
        .AddHandler<AwcSpadeMessageHandler>("gp.player.logout")
        .AddHandler<AwcSpadeMessageHandler>(GameProviderType.AWC_SPADE.ToGamePlayerLogOutSubject(), GameProviderType.AWC_SPADE.ToGamePlayerLogOutSubject())
        .AddHandler<AwcVenusMessageHandler>("gp.player.logout")
        .AddHandler<AwcVenusMessageHandler>(GameProviderType.AWC_VENUS.ToGamePlayerLogOutSubject(), GameProviderType.AWC_VENUS.ToGamePlayerLogOutSubject())
        .AddHandler<AwcYlMessageHandler>("gp.player.logout")
        .AddHandler<AwcYlMessageHandler>(GameProviderType.AWC_YL.ToGamePlayerLogOutSubject(), GameProviderType.AWC_YL.ToGamePlayerLogOutSubject())
        .AddHandler<AwcYesbingoMessageHandler>("gp.player.logout")
        .AddHandler<AwcYesbingoMessageHandler>(GameProviderType.AWC_YESBINGO.ToGamePlayerLogOutSubject(), GameProviderType.AWC_YESBINGO.ToGamePlayerLogOutSubject())
        .AddHandler<JiliTransferMessageHandler>("gp.player.logout")
        .AddHandler<JiliTransferMessageHandler>(GameProviderType.JiliTransfer.ToGamePlayerLogOutSubject(), GameProviderType.JiliTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<WMCasinoTransferMessageHandler>("gp.player.logout")
        .AddHandler<WMCasinoTransferMessageHandler>(GameProviderType.WMCasinoTransfer.ToGamePlayerLogOutSubject(), GameProviderType.WMCasinoTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<EvolutionTransferMessageHandler>("gp.player.logout")
        .AddHandler<EvolutionTransferMessageHandler>(GameProviderType.EvolutionTransfer.ToGamePlayerLogOutSubject(), GameProviderType.EvolutionTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<JDBTransferMessageHandler>("gp.player.logout")
        .AddHandler<JDBTransferMessageHandler>(GameProviderType.JDBTransfer.ToGamePlayerLogOutSubject(), GameProviderType.JDBTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<SBOTransferMessageHandler>("gp.player.logout")
        .AddHandler<SBOTransferMessageHandler>(GameProviderType.SBOTransfer.ToGamePlayerLogOutSubject(), GameProviderType.SBOTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<HabaneroTransferMessageHandler>("gp.player.logout")
        .AddHandler<HabaneroTransferMessageHandler>(GameProviderType.HabaneroTransfer.ToGamePlayerLogOutSubject(), GameProviderType.HabaneroTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<SabaTransferMessageHandler>("gp.player.logout")
        .AddHandler<SabaTransferMessageHandler>(GameProviderType.SabaTransfer.ToGamePlayerLogOutSubject(), GameProviderType.SabaTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<TFGamingTransferMessageHandler>("gp.player.logout")
        .AddHandler<TFGamingTransferMessageHandler>(GameProviderType.TFGamingTransfer.ToGamePlayerLogOutSubject(), GameProviderType.TFGamingTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<DreamGamingTransferMessageHandler>("gp.player.logout")
        .AddHandler<DreamGamingTransferMessageHandler>(GameProviderType.DreamGamingTransfer.ToGamePlayerLogOutSubject(), GameProviderType.DreamGamingTransfer.ToGamePlayerLogOutSubject())
        .AddHandler<FachaiMessageHandler>("gp.player.logout")
        .AddHandler<FachaiMessageHandler>(GameProviderType.FaChai.ToGamePlayerLogOutSubject(), GameProviderType.FaChai.ToGamePlayerLogOutSubject())
        .AddHandler<EndorphinaMessageHandler>("gp.player.logout")
        .AddHandler<EndorphinaMessageHandler>(GameProviderType.Endorphina.ToGamePlayerLogOutSubject(), GameProviderType.Endorphina.ToGamePlayerLogOutSubject())
        .AddHandler<OBetMessageHandler>("gp.player.logout")
        .AddHandler<OBetMessageHandler>(GameProviderType.OBet.ToGamePlayerLogOutSubject(), GameProviderType.OBet.ToGamePlayerLogOutSubject())
        .AddHandler<YeeBetMessageHandler>("gp.player.logout")
        .AddHandler<YeeBetMessageHandler>(GameProviderType.YeeBet.ToGamePlayerLogOutSubject(), GameProviderType.YeeBet.ToGamePlayerLogOutSubject())
        .AddHandler<PinnacleMessageHandler>("gp.player.logout")
        .AddHandler<PinnacleMessageHandler>(GameProviderType.Pinnacle.ToGamePlayerLogOutSubject(), GameProviderType.Pinnacle.ToGamePlayerLogOutSubject())
        .AddHandler<WicketCricketMessageHandler>("gp.player.logout")
        .AddHandler<WicketCricketMessageHandler>(GameProviderType.WicketCricket.ToGamePlayerLogOutSubject(), GameProviderType.WicketCricket.ToGamePlayerLogOutSubject())
        .AddHandler<GKXGamingMessageHandler>("gp.player.logout")
        .AddHandler<GKXGamingMessageHandler>(GameProviderType.GKXGaming.ToGamePlayerLogOutSubject(), GameProviderType.GKXGaming.ToGamePlayerLogOutSubject())
        .AddHandler<V8PokerMessageHandler>("gp.player.logout")
        .AddHandler<V8PokerMessageHandler>(GameProviderType.V8Poker.ToGamePlayerLogOutSubject(), GameProviderType.V8Poker.ToGamePlayerLogOutSubject());
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
        .AddAspNetCoreInstrumentation(options => options.Filter =
            httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !httpContext.Request.Path.StartsWithSegments("/hangfire")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
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

app.Run();
