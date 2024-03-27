using Confluent.Kafka;
using FreeRedis;
using FreeScheduler;
using System;

namespace CoreWebAPI.Test.WebAPI.FreeScheduler11
{
    class MyTaskHandler : FreeScheduler.TaskHandlers.FreeSqlHandler
    {
        public MyTaskHandler(IFreeSql fsql) : base(fsql) { }

        public override void OnExecuting(Scheduler scheduler, TaskInfo task)
        {

            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {task.Topic} 被执行");
        }
    }
}
