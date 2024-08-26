namespace Lab5.GameSystem
{
	/// <summary>
	/// Interface for systems that require execution during the PreLateUpdate phase.
	/// </summary>
	public interface IPreLateUpdate
	{
		void PreLateUpdate();
	}
}