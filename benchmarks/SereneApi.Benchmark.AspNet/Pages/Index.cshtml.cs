using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Response;
using SereneApi.Benchmark.AspNet.API;
using System;
using System.Diagnostics;

namespace SereneApi.Benchmark.AspNet.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IStudentApi _studentApi;

        public int Iterations { get; private set; }

        public double RunTime { get; private set; }

        public double ExecutionTime { get; private set; }

        public double AverageRunTime { get; private set; }

        public double AverageExecutionTime { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, IStudentApi studentApi)
        {
            _logger = logger;

            _studentApi = studentApi;
        }

        public void OnGet(int iterations = 1000)
        {
            Stopwatch total = Stopwatch.StartNew();

            double runMs = 0;

            for (int i = 0; i < iterations; i++)
            {
                Stopwatch runTime = Stopwatch.StartNew();

                var response = _studentApi.GetStudents().GetAwaiter().GetResult();

                runTime.Stop();

                if (response.WasNotSuccessful || response.HasNullData())
                {
                    throw new NullReferenceException();
                }

                runMs += runTime.Elapsed.TotalMilliseconds;
            }

            total.Stop();

            Iterations = iterations;
            RunTime = Math.Round(total.Elapsed.TotalMilliseconds, 3);
            ExecutionTime = Math.Round(runMs, 3);
            AverageRunTime = Math.Round(total.Elapsed.TotalMilliseconds / iterations, 3);
            AverageExecutionTime = Math.Round(runMs / iterations, 3);
        }
    }
}
