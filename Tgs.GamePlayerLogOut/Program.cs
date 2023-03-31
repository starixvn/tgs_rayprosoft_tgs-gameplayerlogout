using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GamePlayerLogOut;
using Tgs.GamePlayerLogOut.GrpcServices;
using Tgs.GamePlayerLogOut.MessageHandlers;

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
    options.BaseUrl = configuration.GetValue<Uri>("GamePlayerLogOut:GameProvidersClientBaseUrl")!;
});

builder.Services
    .AddMessageQueue()
    .AddRabbitMessageQueue(config =>
    {
        _ = config
            .ConfigQueueOptions((options, sp) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                configuration.GetSection("RabbitMQ").Bind(options);
                options.OnSetupQueueAndExchange = channel =>
                {
                    _ = channel.QueueDeclare(
                        "gp.kickplayer.ky",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.ky",
                        "amq.fanout",
                        "gp.kickplayer.ky",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.vr",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.vr",
                        "amq.fanout",
                        "gp.kickplayer.vr",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.wgvia",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.wgvia",
                        "amq.fanout",
                        "gp.kickplayer.wgvia",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.wgfm",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.wgfm",
                        "amq.fanout",
                        "gp.kickplayer.wgfm",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.bg",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.bg",
                        "amq.fanout",
                        "gp.kickplayer.bg",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.tcg",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.tcg",
                        "amq.fanout",
                        "gp.kickplayer.tcg",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.ly",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.ly",
                        "amq.fanout",
                        "gp.kickplayer.ly",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.sbo",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.sbo",
                        "amq.fanout",
                        "gp.kickplayer.sbo",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.wm",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.wm",
                        "amq.fanout",
                        "gp.kickplayer.wm",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.pp",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.pp",
                        "amq.fanout",
                        "gp.kickplayer.pp",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.saba",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.saba",
                        "amq.fanout",
                        "gp.kickplayer.saba",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.avia",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.avia",
                        "amq.fanout",
                        "gp.kickplayer.avia",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.jili",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.jili",
                        "amq.fanout",
                        "gp.kickplayer.jili",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.tf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.tf",
                        "amq.fanout",
                        "gp.kickplayer.tf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.ab",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.ab",
                        "amq.fanout",
                        "gp.kickplayer.ab",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.haba",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.haba",
                        "amq.fanout",
                        "gp.kickplayer.haba",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.365",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.365",
                        "amq.fanout",
                        "gp.kickplayer.365",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.jdb",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.jdb",
                        "amq.fanout",
                        "gp.kickplayer.jdb",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.dg",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.dg",
                        "amq.fanout",
                        "gp.kickplayer.dg",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.joker",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.joker",
                        "amq.fanout",
                        "gp.kickplayer.joker",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.pptf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.pptf",
                        "amq.fanout",
                        "gp.kickplayer.pptf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.aelotto",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.aelotto",
                        "amq.fanout",
                        "gp.kickplayer.awc.aelotto",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.aws",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.aws",
                        "amq.fanout",
                        "gp.kickplayer.awc.aws",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.cockfight",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.cockfight",
                        "amq.fanout",
                        "gp.kickplayer.awc.cockfight",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.facai",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.facai",
                        "amq.fanout",
                        "gp.kickplayer.awc.facai",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.fs",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.fs",
                        "amq.fanout",
                        "gp.kickplayer.awc.fs",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.km",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.km",
                        "amq.fanout",
                        "gp.kickplayer.awc.km",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.ludo",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.ludo",
                        "amq.fanout",
                        "gp.kickplayer.awc.ludo",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.pgs",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.pgs",
                        "amq.fanout",
                        "gp.kickplayer.awc.pgs",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.rcb",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.rcb",
                        "amq.fanout",
                        "gp.kickplayer.awc.rcb",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.sg",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.sg",
                        "amq.fanout",
                        "gp.kickplayer.awc.sg",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.spade",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.spade",
                        "amq.fanout",
                        "gp.kickplayer.awc.spade",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.venus",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.venus",
                        "amq.fanout",
                        "gp.kickplayer.awc.venus",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.awc.yl",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.awc.yl",
                        "amq.fanout",
                        "gp.kickplayer.awc.yl",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.jilitf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.jilitf",
                        "amq.fanout",
                        "gp.kickplayer.jilitf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.wmtf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.wmtf",
                        "amq.fanout",
                        "gp.kickplayer.wmtf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.evotf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.evotf",
                        "amq.fanout",
                        "gp.kickplayer.evotf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.jdbtf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.jdbtf",
                        "amq.fanout",
                        "gp.kickplayer.jdbtf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.sbotf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.sbotf",
                        "amq.fanout",
                        "gp.kickplayer.sbotf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.habatf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.habatf",
                        "amq.fanout",
                        "gp.kickplayer.habatf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.sabatf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.sabatf",
                        "amq.fanout",
                        "gp.kickplayer.sabatf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.tftf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.tftf",
                        "amq.fanout",
                        "gp.kickplayer.tftf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.dgtf",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.dgtf",
                        "amq.fanout",
                        "gp.kickplayer.dgtf",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.fachai",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.fachai",
                        "amq.fanout",
                        "gp.kickplayer.fachai",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.endor",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.endor",
                        "amq.fanout",
                        "gp.kickplayer.endor",
                        new Dictionary<string, object>());

                    _ = channel.QueueDeclare(
                        "gp.kickplayer.obet",
                        true,
                        false,
                        false,
                        new Dictionary<string, object>());

                    channel.QueueBind(
                        "gp.kickplayer.obet",
                        "amq.fanout",
                        "gp.kickplayer.obet",
                        new Dictionary<string, object>());
                };
            })
            .AddHandler<KYMessageHandler>("gp.kickplayer.ky", true)
            .AddHandler<VRMessageHandler>("gp.kickplayer.vr", true)
            .AddHandler<WGHorseMessageHandler>("gp.kickplayer.wgvia", true)
            .AddHandler<WGSportsMessageHandler>("gp.kickplayer.wgfm", true)
            .AddHandler<BigGamingMessageHandler>("gp.kickplayer.bg", true)
            .AddHandler<TCGMessageHandler>("gp.kickplayer.tcg", true)
            .AddHandler<LEGamingMessageHandler>("gp.kickplayer.ly", true)
            .AddHandler<SBOMessageHandler>("gp.kickplayer.sbo", true)
            .AddHandler<WMCasinoMessageHandler>("gp.kickplayer.wm", true)
            .AddHandler<PragmaticPlayMessageHandler>("gp.kickplayer.pp", true)
            .AddHandler<SabaMessageHandler>("gp.kickplayer.saba", true)
            .AddHandler<AviaMessageHandler>("gp.kickplayer.avia", true)
            .AddHandler<JiliMessageHandler>("gp.kickplayer.jili", true)
            .AddHandler<TFGamingMessageHandler>("gp.kickplayer.tf", true)
            .AddHandler<AllBetMessageHandler>("gp.kickplayer.ab", true)
            .AddHandler<HabaneroMessageHandler>("gp.kickplayer.haba", true)
            .AddHandler<Card365MessageHandler>("gp.kickplayer.365", true)
            .AddHandler<JDBMessageHandler>("gp.kickplayer.jdb", true)
            .AddHandler<DreamGameMessageHandler>("gp.kickplayer.dg", true)
            .AddHandler<JokerMessageHandler>("gp.kickplayer.joker", true)
            .AddHandler<PragmaticPlayTransferMessageHandler>("gp.kickplayer.pptf", true)
            .AddHandler<AwcAelottoMessageHandler>("gp.kickplayer.awc.aelotto", true)
            .AddHandler<AwcAwsMessageHandler>("gp.kickplayer.awc.aws", true)
            .AddHandler<AwcCockfightMessageHandler>("gp.kickplayer.awc.cockfight", true)
            .AddHandler<AwcFacaiMessageHandler>("gp.kickplayer.awc.facai", true)
            .AddHandler<AwcFsMessageHandler>("gp.kickplayer.awc.fs", true)
            .AddHandler<AwcKmMessageHandler>("gp.kickplayer.awc.km", true)
            .AddHandler<AwcLudoMessageHandler>("gp.kickplayer.awc.ludo", true)
            .AddHandler<AwcPgsMessageHandler>("gp.kickplayer.awc.pgs", true)
            .AddHandler<AwcRcbMessageHandler>("gp.kickplayer.awc.rcb", true)
            .AddHandler<AwcSgMessageHandler>("gp.kickplayer.awc.sg", true)
            .AddHandler<AwcSpadeMessageHandler>("gp.kickplayer.awc.spade", true)
            .AddHandler<AwcVenusMessageHandler>("gp.kickplayer.awc.venus", true)
            .AddHandler<AwcYlMessageHandler>("gp.kickplayer.awc.yl", true)
            .AddHandler<JiliTransferMessageHandler>("gp.kickplayer.jilitf", true)
            .AddHandler<WMCasinoTransferMessageHandler>("gp.kickplayer.wmtf", true)
            .AddHandler<EvolutionTransferMessageHandler>("gp.kickplayer.evotf", true)
            .AddHandler<JDBTransferMessageHandler>("gp.kickplayer.jdbtf", true)
            .AddHandler<SBOTransferMessageHandler>("gp.kickplayer.sbotf", true)
            .AddHandler<HabaneroTransferMessageHandler>("gp.kickplayer.habatf", true)
            .AddHandler<SabaTransferMessageHandler>("gp.kickplayer.sabatf", true)
            .AddHandler<TFGamingTransferMessageHandler>("gp.kickplayer.tftf", true)
            .AddHandler<DreamGamingTransferMessageHandler>("gp.kickplayer.dgtf", true)
            .AddHandler<FachaiMessageHandler>("gp.kickplayer.fachai", true)
            .AddHandler<EndorphinaMessageHandler>("gp.kickplayer.endor", true)
            .AddHandler<OBetMessageHandler>("gp.kickplayer.obet", true);
    })
    .ClearAllExchanges()
    .AddRabbitGlobPatternExchange("gp.kickplayer", "amq.fanout")
    .AddRabbitGlobPatternExchange("gp.kickplayer.*", "amq.fanout");

builder.Services
    .AddOpenTelemetry()
    .WithTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
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
