using Quartz;
using Quartz.AspNetCore;
using Quartz.Impl;
using Quartz.Spi.MongoDbJobStore;
using QuartzNETExample;

var builder = WebApplication.CreateBuilder(args);

// This throws error: https://github.com/glucaci/mongodb-quartz-net/issues/47
builder.Services.AddQuartz(q =>
{
    q.Properties[$"{StdSchedulerFactory.PropertyObjectSerializer}.type"] = "binary";
    q.Properties[StdSchedulerFactory.PropertySchedulerInstanceName] = "quartzInstance";
    q.Properties[StdSchedulerFactory.PropertySchedulerInstanceId] = Guid.NewGuid().ToString();
    q.Properties[StdSchedulerFactory.PropertyJobStoreType] = typeof(MongoDbJobStore).AssemblyQualifiedName;
    q.Properties[$"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyDataSourceConnectionString}"] = "mongodb://root:example@localhost:27017/quartz";
    q.Properties[$"{StdSchedulerFactory.PropertyJobStorePrefix}.collectionPrefix"] = "qtz"; //Optional
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

var job = JobBuilder.Create<Job>()
    .WithIdentity("myJob", "group1")
    .Build();

var trigger = TriggerBuilder.Create()
    .WithIdentity("myTrigger", "group1")
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInSeconds(40)
        .RepeatForever())
    .Build();

await scheduler.ScheduleJob(job, trigger);

app.Run();