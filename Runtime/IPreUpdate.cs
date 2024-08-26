namespace Lab5.GameSystem
{
	/// <summary>
	/// Interface for systems that require execution during the PreUpdate phase.
	/// </summary>
	public interface IPreUpdate
	{
		void PreUpdate();
	}
}