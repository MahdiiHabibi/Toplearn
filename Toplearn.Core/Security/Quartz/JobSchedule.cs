using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Security.Quartz
{
	public class JobSchedule(Type jobType,string cronExpression)
	{
		public Type JobType { get; set; } = jobType;

		public string CronExpression { get; set; } = cronExpression;
	}
}
