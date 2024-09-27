using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.Security.Quartz.Jobs
{
	[DisallowConcurrentExecution]
	public class RemoveOffsOfCoursesJob(IServiceProvider serviceProvider) : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			try
			{
				await using var scope = serviceProvider.CreateAsyncScope();

				TopLearnContext dbContext = scope.ServiceProvider.GetService<TopLearnContext>() ??
				                            throw new NullReferenceException("dbContext is Null");

				var model = dbContext.CourseOffs
					.Include(x=>x.Course)
					.Where(x => x.OffEndDate < DateTime.Now)
					.ToList();

				foreach (var off in model)
				{
					off.Course.CoursePrice = off.RealPrice;
					dbContext.Courses.Update(off.Course);
					dbContext.CourseOffs.Remove(off);
				}

				await dbContext.SaveChangesAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
