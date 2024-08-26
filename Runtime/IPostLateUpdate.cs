namespace Lab5.GameSystem
{
	/// <summary>
	/// Interface for systems that require execution during the PostLateUpdate phase.
	/// </summary>
	public interface IPostLateUpdate
	{
		void PostLateUpdate();
	}
}