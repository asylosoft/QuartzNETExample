using Quartz;

namespace QuartzNETExample
{

    public class Job : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"Running at: {DateTime.Now.ToString()}");

            return Task.CompletedTask;
        }
    }
}
