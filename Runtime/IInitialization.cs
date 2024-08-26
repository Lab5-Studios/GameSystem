namespace Lab5.GameSystem
{
	/// <summary>
	/// Interface for systems that require execution during the Initialization phase.
	/// This method is called at the very beginning of each frame.
	/// </summary>
	public interface IInitialization
	{
		void Initialization();
	}
}