using StructureMap;

namespace GithubDashboard.Github.IoC
{
	public class GithubRegistry : Registry
	{
		public GithubRegistry()
		{
			Scan(
				x =>
				{
					x.TheCallingAssembly();
					x.WithDefaultConventions();
				});
		}
	}
}