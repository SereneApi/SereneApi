using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using SereneApi.Benchmark.API;
using SereneApi.Extensions.Mocking;
using SereneApi.Factories;
using System;
using System.Diagnostics;

namespace SereneApi.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiFactory factory = new ApiFactory();

            factory.RegisterApi<IStudentApi, StudentApiHandler>(o =>
            {
                o.SetSource("http://localhost", "Student");
            }).WithMockResponse(m =>
            {
                m.AddMockResponse(new StudentDto
                {
                    Email = "John.Smith@gmail.com",
                    FirstName = "John",
                    LastName = "Smith",
                    Id = 0
                }).RespondsToRequestsWith(Method.GET);
            });

            Console.Write("Runs: ");

            int runs = int.Parse(Console.ReadLine() ?? "1");

            Stopwatch total = Stopwatch.StartNew();

            double runMs = 0;
            double buildMs = 0;
            double disposeMs = 0;

            for (int i = 0; i < runs; i++)
            {
                Stopwatch buildTime = Stopwatch.StartNew();

                IStudentApi studentApi = factory.Build<IStudentApi>();

                buildTime.Stop();

                Stopwatch runTime = Stopwatch.StartNew();

                var response = studentApi.GetStudentsAsync().GetAwaiter().GetResult();

                runTime.Stop();

                if (response.WasNotSuccessful() || response.Data == null)
                {
                    throw new NullReferenceException();
                }

                Stopwatch disposeTime = Stopwatch.StartNew();

                studentApi.Dispose();

                disposeTime.Stop();

                buildMs += buildTime.Elapsed.TotalMilliseconds;
                runMs += runTime.Elapsed.TotalMilliseconds;
                disposeMs += disposeTime.Elapsed.TotalMilliseconds;
            }

            total.Stop();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("--------------------------Benchmark Completed---------------------------");
            Console.WriteLine($"Total Time: {Math.Round(total.Elapsed.TotalMilliseconds, 3)}ms");
            Console.WriteLine($"Total Build Time: {Math.Round(buildMs, 3)}ms");
            Console.WriteLine($"Total Execution Time: {Math.Round(runMs, 3)}ms");
            Console.WriteLine($"Total Dispose Time: {Math.Round(disposeMs, 3)}ms");
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine($"Average Time: {Math.Round(total.Elapsed.TotalMilliseconds / runs, 3)}ms");
            Console.WriteLine($"Average Build Time: {Math.Round(buildMs / runs, 3)}ms");
            Console.WriteLine($"Average Execution Time: {Math.Round(runMs / runs, 3)}ms");
            Console.WriteLine($"Average Dispose Time: {Math.Round(disposeMs / runs, 3)}ms");
            Console.WriteLine("------------------------------------------------------------------------");

            Console.ReadKey();
        }
    }
}
