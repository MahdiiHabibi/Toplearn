using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Convertors
{
	public static class GetProperties<TModel> where TModel : class
	{
		public static string GetNameOfKeyPropertyOfTbl()
		{
			var modelType = typeof(TModel);
			PropertyInfo[] properties = modelType.GetProperties();
			string attributeName;
			foreach (var property in properties)
			{
				if (property.CustomAttributes.Any(x => x.AttributeType.AssemblyQualifiedName
					.Split(",")[0]
					.Split(".")[^1]
					.ToLower()
					.Replace("attribute", "")
					.Trim() == "key"))
				{
					return property.Name;
				}
			}
			return "";
		}

		public static int GetValueOfKeyPropertyOfTbl(TModel model)
		{
			var modelType = typeof(TModel);
			PropertyInfo[] properties = modelType.GetProperties();
			string attributeName;
			foreach (var property in properties)
			{
				if (property.CustomAttributes.Any(x => x.AttributeType.AssemblyQualifiedName
						.Split(",")[0]
						.Split(".")[^1]
						.ToLower()
						.Replace("attribute", "")
						.Trim() == "key"))
				{
					var value = property.GetValue(model);
					return int.Parse(value.ToString());
				}
			}
			return 0;
		}

	}
}
