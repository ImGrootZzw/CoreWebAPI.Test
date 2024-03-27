using GZY.Quartz.MUI.BaseService;

namespace CoreWebAPI.Test.WebAPI.Job
{
    public class TestJob : IJobService
    {
        public string ExecuteService(string parameter)
        {
            return "定时任务已执行成功!";
        }
    }
}
