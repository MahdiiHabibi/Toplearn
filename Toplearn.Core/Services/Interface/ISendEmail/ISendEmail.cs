
namespace Toplearn.Core.Services.Interface.ISendEmail
{

	public interface ISendEmail
	{
		public bool Send(string To, string Subject, string Body);

	}
}