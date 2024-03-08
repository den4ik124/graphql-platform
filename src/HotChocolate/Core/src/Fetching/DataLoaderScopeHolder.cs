using System;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Linq;
#endif
using System.Collections.Generic;
using System.Threading;
using GreenDonut;
using GreenDonut.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Fetching;

/// <summary>
/// This instance holds the current DataLoader scope and allows to create a new scope.
/// </summary>
public sealed class DataLoaderScopeHolder
{
    private static readonly AsyncLocal<InstanceHolder> _currentScope = new();
#if NET8_0_OR_GREATER
    private readonly FrozenDictionary<Type, DataLoaderRegistration> _registrations;
#else
    private readonly Dictionary<Type, DataLoaderRegistration> _registrations;
#endif

    public DataLoaderScopeHolder(IEnumerable<DataLoaderRegistration> registrations)
    {
#if NET8_0_OR_GREATER
        _registrations = CreateRegistrations().ToFrozenDictionary(t => t.Item1, t => t.Item2);
#else
        _registrations = CreateRegistrations().ToDictionary(t => t.Item1, t => t.Item2);
#endif
        
        IEnumerable<(Type, DataLoaderRegistration)> CreateRegistrations()
        {
            foreach (var reg in registrations)
            {
                if (reg.ServiceType == reg.InstanceType)
                {
                    yield return (reg.ServiceType, reg);
                }
                else
                {
                    yield return (reg.ServiceType, reg);
                    yield return (reg.InstanceType, reg);
                }
            }
        }
    }

    /// <summary>
    /// Creates and pins a new <see cref="IDataLoaderContext"/>.
    /// </summary>
    public IDataLoaderContext PinNewScope(IServiceProvider scopedServiceProvider, IBatchScheduler? scheduler = null)
    {
        scheduler ??= scopedServiceProvider.GetRequiredService<IBatchScheduler>();
        return CurrentContext = new ExecutionDataLoaderContext(scopedServiceProvider, scheduler, _registrations);
    }
    
    public IDataLoaderContext GetOrCreateScope(IServiceProvider scopedServiceProvider, IBatchScheduler? scheduler = null)
    {
        if(_currentScope.Value?.Context is null)
        {
            CurrentContext = PinNewScope(scopedServiceProvider, scheduler);
        }
        return CurrentContext;
    }

    /// <summary>
    /// Gets access to the current <see cref="IDataLoaderContext"/> instance.
    /// </summary>
    /// <exception cref="InvalidCastException">
    /// The instance was not initialized.
    /// </exception>
    public IDataLoaderContext CurrentContext
    {
        get => _currentScope.Value?.Context ??
            throw new InvalidOperationException("No DataLoader scope exists.");
        set
        {
            var holder = _currentScope.Value;

            if (holder is null)
            {
                holder = new InstanceHolder();
                _currentScope.Value = holder;
            }

            holder.Context = value;
        }
    }

    private sealed class InstanceHolder
    {
        public IDataLoaderContext Context = default!;
    }
}