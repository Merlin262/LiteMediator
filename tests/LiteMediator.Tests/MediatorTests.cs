using LiteMediator.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LiteMediator.Tests;

public class MediatorTests
{
    private static ServiceProvider BuildProvider(Action<MediatorConfiguration>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLiteMediator(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<MediatorTests>();
            // Singleton só para os testes conseguirem inspecionar o estado dos handlers depois do Publish/Send.
            cfg.HandlerLifetime = ServiceLifetime.Singleton;
            configure?.Invoke(cfg);
        });
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_DespachaParaOHandlerCorreto_EReturnaAResposta()
    {
        using var provider = BuildProvider();
        var sender = provider.GetRequiredService<ISender>();

        var response = await sender.Send(new Ping("oi"));

        Assert.Equal("Pong: oi", response);
    }

    [Fact]
    public async Task Send_ComIRequestSemRetorno_ChamaOHandlerViaRequestHandlerBase()
    {
        var handler = new DeleteThingHandler();

        var services = new ServiceCollection();
        services.AddLiteMediator(cfg => { });
        services.AddSingleton<IRequestHandler<DeleteThing, Unit>>(handler);

        using var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ISender>();

        await sender.Send(new DeleteThing(42));

        Assert.Equal(new[] { 42 }, handler.DeletedIds);
    }

    [Fact]
    public async Task Publish_ChamaTodosOsHandlersRegistradosParaANotificacao()
    {
        using var provider = BuildProvider();
        var publisher = provider.GetRequiredService<IPublisher>();

        await publisher.Publish(new ThingCreated("widget"));

        var handlers = provider.GetServices<INotificationHandler<ThingCreated>>().ToList();
        var auditLog = Assert.Single(handlers.OfType<AuditLogHandler>());
        var email = Assert.Single(handlers.OfType<EmailHandler>());

        Assert.Contains("audit:widget", auditLog.Logged);
        Assert.Contains("email:widget", email.Sent);
    }

    [Fact]
    public async Task PipelineBehaviors_SaoExecutadosNaOrdemDeRegistro_EnvolvendoOHandler()
    {
        RecordingBehavior<Ping, string>.CallOrder.Clear();

        using var provider = BuildProvider(cfg =>
        {
            cfg.AddOpenBehavior(typeof(FirstBehavior<,>));
            cfg.AddOpenBehavior(typeof(SecondBehavior<,>));
        });

        var sender = provider.GetRequiredService<ISender>();

        await sender.Send(new Ping("oi"));

        Assert.Equal(
            new[] { "first:before", "second:before", "second:after", "first:after" },
            RecordingBehavior<Ping, string>.CallOrder);
    }
}
