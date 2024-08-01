using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GamePlayerLogOut.Extensions;
using Tgs.GamePlayerLogOut.MessageHandlers;
using Tgs.GameProviders.Domains;
using Tgs.MessageQueue.Nats.Serializers;

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
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
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
	.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
	{
		Url = builder.Configuration.GetConnectionString("Nats")!,
		SerializerRegistry = TgsNatsSerializerRegistration.Default
	}))
	.AddMessageQueue()
	.AddNatsMessageQueue(config => config
		.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
		.AddLogoutHandlerWithSubjectSuffix<KYMessageHandler>(GameProviderType.Kaiyuan)
		.AddLogoutHandlerWithSubjectSuffix<VRMessageHandler>(GameProviderType.VRGaming)
		.AddLogoutHandlerWithSubjectSuffix<WGViaGamingMessageHandler>(GameProviderType.WGViaGaming)
		.AddLogoutHandlerWithSubjectSuffix<WGFmGamingMessageHandler>(GameProviderType.WGFmGaming)
		.AddLogoutHandlerWithSubjectSuffix<TCGMessageHandler>(GameProviderType.TCG)
		.AddLogoutHandlerWithSubjectSuffix<LEGamingMessageHandler>(GameProviderType.LEGaming)
		.AddLogoutHandlerWithSubjectSuffix<Cards365TransferMessageHandler>(GameProviderType.Cards365Transfer)
		.AddLogoutHandlerWithSubjectSuffix<JokerMessageHandler>(GameProviderType.Joker)
		.AddLogoutHandlerWithSubjectSuffix<PragmaticPlayTransferMessageHandler>(GameProviderType.PragmaticPlayTransfer)
		.AddLogoutHandlerWithSubjectSuffix<AwcCockfightMessageHandler>(GameProviderType.AWC_COCKFIGHT)
		.AddLogoutHandlerWithSubjectSuffix<AwcFsMessageHandler>(GameProviderType.AWC_FS)
		.AddLogoutHandlerWithSubjectSuffix<AwcKmMessageHandler>(GameProviderType.AWC_KM)
		.AddLogoutHandlerWithSubjectSuffix<AwcLudoMessageHandler>(GameProviderType.AWC_LUDO)
		.AddLogoutHandlerWithSubjectSuffix<AwcPgsMessageHandler>(GameProviderType.AWC_PGS)
		.AddLogoutHandlerWithSubjectSuffix<AwcRcbMessageHandler>(GameProviderType.AWC_RCB)
		.AddLogoutHandlerWithSubjectSuffix<AwcSgMessageHandler>(GameProviderType.AWC_SG)
		.AddLogoutHandlerWithSubjectSuffix<AwcSpadeMessageHandler>(GameProviderType.AWC_SPADE)
		.AddLogoutHandlerWithSubjectSuffix<AwcVenusMessageHandler>(GameProviderType.AWC_VENUS)
		.AddLogoutHandlerWithSubjectSuffix<AwcYlMessageHandler>(GameProviderType.AWC_YL)
		.AddLogoutHandlerWithSubjectSuffix<AwcYesbingoMessageHandler>(GameProviderType.AWC_YESBINGO)
		.AddLogoutHandlerWithSubjectSuffix<JiliTransferMessageHandler>(GameProviderType.JiliTransfer)
		.AddLogoutHandlerWithSubjectSuffix<WMCasinoTransferMessageHandler>(GameProviderType.WMCasinoTransfer)
		.AddLogoutHandlerWithSubjectSuffix<EvolutionTransferMessageHandler>(GameProviderType.EvolutionTransfer)
		.AddLogoutHandlerWithSubjectSuffix<JDBTransferMessageHandler>(GameProviderType.JDBTransfer)
		.AddLogoutHandlerWithSubjectSuffix<SBOTransferMessageHandler>(GameProviderType.SBOTransfer)
		.AddLogoutHandlerWithSubjectSuffix<HabaneroTransferMessageHandler>(GameProviderType.HabaneroTransfer)
		.AddLogoutHandlerWithSubjectSuffix<SabaTransferMessageHandler>(GameProviderType.SabaTransfer)
		.AddLogoutHandlerWithSubjectSuffix<TFGamingTransferMessageHandler>(GameProviderType.TFGamingTransfer)
		.AddLogoutHandlerWithSubjectSuffix<DreamGamingTransferMessageHandler>(GameProviderType.DreamGamingTransfer)
		.AddLogoutHandlerWithSubjectSuffix<FachaiMessageHandler>(GameProviderType.FaChai)
		.AddLogoutHandlerWithSubjectSuffix<EndorphinaMessageHandler>(GameProviderType.Endorphina)
		.AddLogoutHandlerWithSubjectSuffix<OBetMessageHandler>(GameProviderType.OBet)
		.AddLogoutHandlerWithSubjectSuffix<YeeBetMessageHandler>(GameProviderType.YeeBet)
		.AddLogoutHandlerWithSubjectSuffix<PinnacleMessageHandler>(GameProviderType.Pinnacle)
		.AddLogoutHandlerWithSubjectSuffix<WicketCricketMessageHandler>(GameProviderType.WicketCricket)
		.AddLogoutHandlerWithSubjectSuffix<GKXGamingMessageHandler>(GameProviderType.GKXGaming)
		.AddLogoutHandlerWithSubjectSuffix<V8PokerMessageHandler>(GameProviderType.V8Poker)
		.AddLogoutHandlerWithSubjectSuffix<TPhereMessageHandler>(GameProviderType.TPhere)
		.AddLogoutHandlerWithSubjectSuffix<AwcBgMessageHandler>(GameProviderType.AWC_BG)
		.AddLogoutHandlerWithSubjectSuffix<CQ9GamingMessageHandler>(GameProviderType.CQ9Gaming)
		.AddLogoutHandlerWithSubjectSuffix<GTExpanseMessageHandler>(GameProviderType.GT_Expanse)
		.AddLogoutHandlerWithSubjectSuffix<GTPlaytechMessageHandler>(GameProviderType.GT_Playtech)
		.AddLogoutHandlerWithSubjectSuffix<CRSportsMessageHandler>(GameProviderType.CRSports)
	)
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
