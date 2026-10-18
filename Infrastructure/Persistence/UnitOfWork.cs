using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

internal sealed class UnitOfWork(Tabi3DbContext dbContext) : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _currentTransaction;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already active for this unit of work.");
        }

        _currentTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var transaction = GetCurrentTransaction();

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await DisposeCurrentTransactionAsync();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        var transaction = GetCurrentTransaction();

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeCurrentTransactionAsync();
        }
    }

    public async ValueTask DisposeAsync() => await DisposeCurrentTransactionAsync();

    private IDbContextTransaction GetCurrentTransaction() =>
        _currentTransaction ?? throw new InvalidOperationException("No active transaction exists for this unit of work.");

    private async ValueTask DisposeCurrentTransactionAsync()
    {
        if (_currentTransaction is null)
        {
            return;
        }

        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
}
