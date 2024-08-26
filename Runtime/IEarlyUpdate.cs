namespace Lab5.GameSystem
{
	/// <summary>
	/// Interface for systems that require execution during the EarlyUpdate phase.
	/// </summary>
	public interface IEarlyUpdate
	{
		void EarlyUpdate();
	}
}