using BenchmarkDotNet.Attributes;
using LiteMediator.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiteMediator.Benchmarks;

/// <summary>
/// Compara o custo de despachar um request com retorno via LiteMediator vs MediatR vs
/// chamada direta ao handler (sem mediator nenhum, apenas para medir o "chão" teórico).
/// </summary>
[MemoryDiagnoser]
public class SendBenchmarks
{
    private ServiceProvider _liteMediatorProvider = null!;
    private ServiceProvider _mediatRProvider = null!;
    private ISender _liteSender = null!;
    private IMediator _mediatr = null!;
    private DirectPingHandler _direct = null!;

    [GlobalSetup]
    public void Setup()
    {
        var liteServices = new ServiceCollection();
        liteServices.AddLiteMediator(cfg => cfg.RegisterServicesFromAssemblyContaining<SendBenchmarks>());
        _liteMediatorProvider = liteServices.BuildServiceProvider();
        _liteSender = _liteMediatorProvider.GetRequiredService<ISender>();

        var mediatRServices = new ServiceCollection();
        mediatRServices.AddLogging();
        mediatRServices.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SendBenchmarks>());
        _mediatRProvider = mediatRServices.BuildServiceProvider();
        _mediatr = _mediatRProvider.GetRequiredService<IMediator>();

        _direct = new DirectPingHandler();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _liteMediatorProvider.Dispose();
        _mediatRProvider.Dispose();
    }

    [Benchmark(Baseline = true, Description = "Direct call (sem mediator)")]
    public Task<string> DirectCall() => _direct.Handle("oi", CancellationToken.None);

    [Benchmark(Description = "LiteMediator.Send")]
    public Task<string> LiteMediatorSend() => _liteSender.Send(new LiteMediatorPing("oi"));

    [Benchmark(Description = "MediatR.Send")]
    public Task<string> MediatRSend() => _mediatr.Send(new MediatRPing("oi"));
}
