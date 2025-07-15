using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BusBuddy.WPF.Services
{
    /// <summary>
    /// Service for lazy loading ViewModels to improve startup performance
    /// </summary>
    public class LazyViewModelService : ILazyViewModelService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LazyViewModelService> _logger;
        private readonly ConcurrentDictionary<Type, object> _viewModelCache = new();
        private readonly ConcurrentDictionary<Type, Task<object>> _initializationTasks = new();

        public LazyViewModelService(IServiceProvider serviceProvider, ILogger<LazyViewModelService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Get or create a ViewModel instance asynchronously
        /// </summary>
        public async Task<T> GetViewModelAsync<T>() where T : class
        {
            var type = typeof(T);

            // Check if already cached
            if (_viewModelCache.TryGetValue(type, out var cached))
            {
                _logger.LogDebug("Retrieved cached ViewModel {ViewModelType}", type.Name);
                return (T)cached;
            }

            // Check if initialization is in progress
            if (_initializationTasks.TryGetValue(type, out var task))
            {
                _logger.LogDebug("Waiting for ongoing initialization of ViewModel {ViewModelType}", type.Name);
                return (T)await task;
            }

            // Create new initialization task
            var initTask = InitializeViewModelAsync<T>();
            var objectTask = initTask.ContinueWith(t => (object)t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
            _initializationTasks[type] = objectTask;

            try
            {
                var result = await initTask;
                _viewModelCache[type] = result;
                return result;
            }
            finally
            {
                _initializationTasks.TryRemove(type, out _);
            }
        }

        /// <summary>
        /// Get a ViewModel instance synchronously (creates if not exists)
        /// </summary>
        public T GetViewModel<T>() where T : class
        {
            var type = typeof(T);

            // Check if already cached
            if (_viewModelCache.TryGetValue(type, out var cached))
            {
                _logger.LogDebug("Retrieved cached ViewModel {ViewModelType}", type.Name);
                return (T)cached;
            }

            // Create synchronously
            var stopwatch = Stopwatch.StartNew();
            var viewModel = _serviceProvider.GetRequiredService<T>();
            stopwatch.Stop();

            _logger.LogInformation("Created ViewModel {ViewModelType} in {ElapsedMs}ms",
                type.Name, stopwatch.ElapsedMilliseconds);

            _viewModelCache[type] = viewModel;
            return viewModel;
        }

        /// <summary>
        /// Preload essential ViewModels in the background
        /// </summary>
        public async Task PreloadEssentialViewModelsAsync()
        {
            _logger.LogInformation("Starting preload of essential ViewModels");

            var stopwatch = Stopwatch.StartNew();

            // Preload only essential ViewModels that are likely to be used immediately
            var preloadTasks = new[]
            {
                GetViewModelAsync<ViewModels.DashboardViewModel>(),
                // Add other essential ViewModels here as needed
            };

            await Task.WhenAll(preloadTasks);

            stopwatch.Stop();
            _logger.LogInformation("Preloaded {Count} essential ViewModels in {ElapsedMs}ms",
                preloadTasks.Length, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Clear the ViewModel cache
        /// </summary>
        public void ClearCache()
        {
            _logger.LogInformation("Clearing ViewModel cache");
            _viewModelCache.Clear();
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public (int CachedCount, int InitializingCount) GetCacheStats()
        {
            return (_viewModelCache.Count, _initializationTasks.Count);
        }

        private async Task<T> InitializeViewModelAsync<T>() where T : class
        {
            var type = typeof(T);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Initializing ViewModel {ViewModelType}", type.Name);

            // Create the ViewModel
            var viewModel = _serviceProvider.GetRequiredService<T>();

            // Check if the ViewModel has an async initialization method
            var initMethod = type.GetMethod("InitializeAsync");
            if (initMethod != null && initMethod.ReturnType == typeof(Task))
            {
                _logger.LogDebug("Calling InitializeAsync on {ViewModelType}", type.Name);
                await (Task)initMethod.Invoke(viewModel, null)!;
            }

            stopwatch.Stop();
            _logger.LogInformation("Initialized ViewModel {ViewModelType} in {ElapsedMs}ms",
                type.Name, stopwatch.ElapsedMilliseconds);

            return viewModel;
        }
    }

    /// <summary>
    /// Interface for lazy ViewModel service
    /// </summary>
    public interface ILazyViewModelService
    {
        Task<T> GetViewModelAsync<T>() where T : class;
        T GetViewModel<T>() where T : class;
        Task PreloadEssentialViewModelsAsync();
        void ClearCache();
        (int CachedCount, int InitializingCount) GetCacheStats();
    }
}
