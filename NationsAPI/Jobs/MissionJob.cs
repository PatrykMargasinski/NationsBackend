using System;
using System.Threading.Tasks;
using SettlersAPI.Models;
using SettlersAPI.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Xml.JobSchedulingData20;

namespace SettlersAPI.Jobs{

 [DisallowConcurrentExecution]
public class MissionJob : IJob
{   
    private readonly ILogger<MissionJob> _logger;
    private readonly IMissionService _missionService;
    public MissionJob(
        ILogger<MissionJob> logger,
        IMissionService missionService
        )
    {
        _logger = logger;
        _missionService = missionService;
    }
    public async Task Execute(IJobExecutionContext context)
	{
        JobKey key = context.JobDetail.Key;

		JobDataMap dataMap = context.JobDetail.JobDataMap;
		long pmId = Int32.Parse(dataMap.GetString("pmId"));
		_missionService.EndMission(pmId);
	}

    public async static void Start(ISchedulerFactory factory, long pmId, DateTime finishTime)
    {
        IScheduler scheduler = await factory.GetScheduler();
        IJobDetail job = prepareJobDetail(pmId);
        ITrigger trigger = prepareTrigger(finishTime, pmId);
    
        await scheduler.ScheduleJob(job, trigger);
    }

    private static IJobDetail prepareJobDetail(long pmId) {
        return JobBuilder.Create<MissionJob>()
            .WithIdentity("missionJob" + pmId, "group1")
            .UsingJobData("pmId", pmId.ToString())
            .Build();
    }

    private static ITrigger prepareTrigger(DateTime finishTime, long pmId) {
        return TriggerBuilder.Create()
            .WithIdentity("missionTrigger" + pmId, "group1")
            .StartAt(finishTime)
            .Build();
    }
}

}