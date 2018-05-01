using StructureMap;

namespace OctoView.Github.IoC
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