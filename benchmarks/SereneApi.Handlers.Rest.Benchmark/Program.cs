using ApiCommon.Core.Handler.Factories;
using ApiCommon.Core.Requests;
using ApiCommon.Core.Response;
using ApiCommon.Core.Responses;
using ApiCommon.Extensions.Mocking.Rest;
using ApiCommon.Handlers.Rest.Benchmark.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ApiCommon.Handlers.Rest.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ApiFactory factory = new ApiFactory();

            factory.RegisterApi<IStudentApi, StudentApiHandler>(o =>
            {
                o.SetSource("http://localhost:52279", "Students");
            });

            factory.ExtendApi<StudentApiHandler>().EnableMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .RespondsWith(new List<StudentDto>
                    {
                        new StudentDto
                        {
                            Email = "John.Smith@gmail.com",
                            GivenName = "John",
                            LastName = "Smith",
                            Id = 0
                        }
                    }
                );
            });

            Console.Write("Runs: ");

            int iterations = int.Parse(Console.ReadLine() ?? "1");

            decimal buildTime = 0;
            decimal executionTime = 0;
            decimal disposeTime = 0;

            for (int i = 0; i < iterations; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                IStudentApi studentApi = factory.Build<IStudentApi>();

                stopwatch.Stop();

                buildTime += stopwatch.ElapsedTicks;

                stopwatch.Restart();

                IApiResponse<List<StudentDto>> response = studentApi.GetStudentsAsync().GetAwaiter().GetResult();

                stopwatch.Stop();

                executionTime += stopwatch.ElapsedTicks;

                if (response.WasNotSuccessful || response.HasNullData())
                {
                    throw new NullReferenceException();
                }

                stopwatch.Restart();

                studentApi.Dispose();

                stopwatch.Stop();

                disposeTime += stopwatch.ElapsedTicks;
            }

            decimal totalTime = buildTime + executionTime + disposeTime;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("--------------------------Benchmark Completed---------------------------");
            Console.WriteLine($"Total Time: [{Math.Round(totalTime / 10000)}ms] | [{Math.Round(totalTime)} ticks]");
            Console.WriteLine($"Total Build Time: [{Math.Round(buildTime / 10000)}ms] | [{Math.Round(buildTime)} ticks]");
            Console.WriteLine($"Total Execution Time: [{Math.Round(executionTime / 10000)}ms] | [{Math.Round(executionTime)} ticks]");
            Console.WriteLine($"Total Dispose Time: [{Math.Round(disposeTime / 10000)}ms] | [{Math.Round(disposeTime)} ticks]");
            Console.WriteLine("------------------------------------------------------------------------");

            totalTime /= iterations;
            buildTime /= iterations;
            executionTime /= iterations;
            disposeTime /= iterations;

            Console.WriteLine($"Average Time: [{Math.Round(ToMicroseconds(totalTime), 1)}us] | [{Math.Round(totalTime)} ticks]");
            Console.WriteLine($"Average Build Time: [{Math.Round(ToMicroseconds(buildTime), 1)}us] | [{Math.Round(buildTime)} ticks]");
            Console.WriteLine($"Average Execution Time: [{Math.Round(ToMicroseconds(executionTime), 1)}us] | [{Math.Round(executionTime)} ticks]");
            Console.WriteLine($"Average Dispose Time: [{Math.Round(ToMicroseconds(disposeTime), 1)}us] | [{Math.Round(disposeTime)} ticks]");
            Console.WriteLine("------------------------------------------------------------------------");

            Console.ReadKey();
        }

        private static decimal ToMicroseconds(decimal ticks)
        {
            return ticks / (TimeSpan.TicksPerMillisecond / 1000);
        }
    }
}