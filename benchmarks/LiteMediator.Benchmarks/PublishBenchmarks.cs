using BenchmarkDotNet.Attributes;
using LiteMediator.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiteMediator.Benchmarks;

/// <summary>
/// Compara o custo de publicar uma notification para 3 handlers via LiteMediator vs MediatR
/// vs chamar os 3 handlers diretamente em sequência.
/// </summary>
[MemoryDiagnoser]
public class PublishBenchmarks
{
    private ServiceProvider _liteMediatorProvider = null!;
    private ServiceProvider _mediatRProvider = null!;
    private IPublisher _litePublisher = null!;
    private IMediator _mediatr = null!;
    private DirectAuditHandler[] _directHandlers = null!;

    [GlobalSetup]
    public void Setup()
    {
        var liteServices = new ServiceCollection();
        liteServices.AddLiteMediator(cfg => cfg.RegisterServicesFromAssemblyContaining<PublishBenchmarks>());
        _liteMediatorProvider = liteServices.BuildServiceProvider();
        _litePublisher = _liteMediatorProvider.GetRequiredService<IPublisher>();

        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PublishBenchmarks>());
        _mediatRProvider = mediatRServices.BuildServiceProvider();
        _mediatr = _mediatRProvider.GetRequiredService<IMediator>();

        _directHandlers = [new DirectAuditHandler(), new DirectAuditHandler(), new DirectAuditHandler()];
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _liteMediatorProvider.Dispose();
        _mediatRProvider.Dispose();
    }

    [Benchmark(Baseline = true, Description = "Direct calls (sem mediator)")]
    public async Task DirectCall()
    {
        foreach (var handler in _directHandlers)
        {
            await handler.Handle("widget", CancellationToken.None);
        }
    }

    [Benchmark(Description = "LiteMediator.Publish")]
    public Task LiteMediatorPublish() => _litePublisher.Publish(new LiteMediatorThingCreated("widget"));

    [Benchmark(Description = "MediatR.Publish")]
    public Task MediatRPublish() => _mediatr.Publish(new MediatRThingCreated("widget"));
}
