using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Toplearn.Core.Security.Quartz
{
	public class TopLearnQuartzJobsFactory(IServiceProvider serviceProvider) :IJobFactory
	{
		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			try
			{
				return serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob ?? throw new InvalidOperationException();

			}
			catch 
			{
				// 
				return null;
			}
		}

		public void ReturnJob(IJob job)
		{
			
		}
	}
}
