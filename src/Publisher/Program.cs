using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Handlers;
using Publisher.Model;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Publisher
{
    class Program
    {
        private static Publisher _publisher;
        private static PingContext _dbContext;

        static async Task Main()
        {
            ConfigureServices();

            _dbContext.Database.EnsureCreated();
            
            await LauchEvent();
            await LauchEventWithCommitedTransaction();
            await LauchEventWithUncommitedTransaction();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static async Task LauchEvent()
        {
            var pinged = new Pinged("First event", 1500);

            foreach (PublishStrategy strategy in Enum.GetValues(typeof(PublishStrategy)))
            {
                Console.WriteLine("----------");
                Console.WriteLine($"Strategy: {strategy}");
                Console.WriteLine("--------------------------");

                try
                {
                    await _publisher.Publish(pinged, strategy);
                    Console.WriteLine("In main.... event already published");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("In main.... exception caught");
                    Console.WriteLine($"{ex.GetType()}: {ex.Message}");
                }
            }
        }

        private static async Task LauchEventWithCommitedTransaction()
        {
            var pinged = new Pinged("Second event", 1500);

            Console.WriteLine("---------- ");
            Console.WriteLine($"Strategy: {PublishStrategy.SyncStopOnException} launched inside a transaction");
            Console.WriteLine("--------------------------");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            { 
                try
                {
                    await _publisher.Publish(pinged, PublishStrategy.SyncStopOnException);
                    Console.WriteLine("In main.... event already published");                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine("In main.... exception caught");
                    Console.WriteLine($"{ex.GetType()}: {ex.Message}");
                }
                finally
                {
                    transaction.Complete();
                }
            }

            Console.WriteLine("---------- ");
            Console.WriteLine($"Strategy: {PublishStrategy.ParallelWait} launched inside a transaction");
            Console.WriteLine("--------------------------");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _publisher.Publish(pinged, PublishStrategy.ParallelWait);
                    Console.WriteLine("In main.... event already published");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("In main.... exception caught");
                    Console.WriteLine($"{ex.GetType()}: {ex.Message}");
                }
                finally
                {
                    transaction.Complete();
                }
            }
        }

        private static async Task LauchEventWithUncommitedTransaction()
        {
            var pinged = new Pinged("Third event", 1500);

            Console.WriteLine("----------");
            Console.WriteLine($"Strategy: {PublishStrategy.SyncStopOnException} launched inside a transaction that won't be commited");
            Console.WriteLine("--------------------------");

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _publisher.Publish(pinged, PublishStrategy.SyncStopOnException);
                    Console.WriteLine("In main.... event already published");                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("In main.... exception caught");
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }

            Console.WriteLine("----------");
            Console.WriteLine($"Strategy: {PublishStrategy.ParallelWait} launched inside a transaction that won't be commited");
            Console.WriteLine("--------------------------");

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _publisher.Publish(pinged, PublishStrategy.ParallelWait);
                    Console.WriteLine("In main.... event already published");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("In main.... exception caught");
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<PingContext>(options => 
                                                    options.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Ping;Trusted_Connection=True;")
                                                    , ServiceLifetime.Transient);

            services.AddScoped<ServiceFactory>(p => p.GetService);
            services.AddSingleton<Publisher>();
            services.AddTransient<INotificationHandler<Pinged>, AsyncPingedHandler>();
            services.AddTransient<INotificationHandler<Pinged>, FaultyPingedHandler>();
            services.AddTransient<INotificationHandler<Pinged>, AsyncPingedSupressTransactionHandler>();

            var provider = services.BuildServiceProvider();
            _publisher = provider.GetRequiredService<Publisher>();
            _dbContext = provider.GetRequiredService<PingContext>();
        }
    }
}
