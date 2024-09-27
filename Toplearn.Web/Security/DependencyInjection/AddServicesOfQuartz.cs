using Cart_Exam.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Toplearn.Core.Security.Quartz;
using Toplearn.Core.Security.Quartz.Jobs;

namespace Toplearn.Web.Security.DependencyInjection
{
	public static class AddServicesOfQuartz
	{
		public static IServiceCollection AddQuartz(this IServiceCollection service)
		{

			service.AddSingleton<IJobFactory, TopLearnQuartzJobsFactory>();
			service.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

			service.AddSingleton(typeof(RemoveOffsOfCoursesJob));

			service.AddSingleton(new JobSchedule(typeof(RemoveOffsOfCoursesJob), "0 0 0 * * ?"));

			service.AddHostedService<QuartzHostedService>();

			return service;
		}
	}
}
