using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            PizzaTaskAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }
        static async Task MakePizza(int i)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    if(i == 4)
                    {
                        cancellationTokenSource.Cancel();
                    }
                    await PreparePizza(i, cancellationTokenSource.Token);
                    await BakePizza(i, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"Order for pizza {i} was cancelled by customer");
                }
            }
        }
        static async Task PizzaTaskAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int totalPizza = 10;
            Console.WriteLine($"Stared preparing {totalPizza} pizza");
           
            var tasks = Enumerable.Range(1, totalPizza).Select(i => MakePizza(i));
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Finished preparing {totalPizza} pizza");
            Console.WriteLine($"Elasped time {stopwatch.Elapsed}");
        }
        static async Task PreparePizza(int n, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();
            Console.WriteLine("Start preparing pizza " + n);
            await Task.Delay(5000);
            Console.WriteLine("Finish preparing pizza " + n);
        }
        static async Task BakePizza(int n, CancellationToken cancellationToken)
        {
             Console.WriteLine("Start baking pizza " + n);
            await Task.Delay(10000);
            Console.WriteLine("Finish baking pizza " + n);
        }
    }
}
