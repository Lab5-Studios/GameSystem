using System;

namespace Lab5.GameSystem
{
	/// <summary>
	/// An attribute used to mark a class as a game system. Classes marked with this attribute
	/// will be automatically detected and registered into the Unity player loop, allowing them 
	/// to participate in various update phases such as Initialization, Update, FixedUpdate, etc.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GameSystemAttribute : Attribute
	{

	}
}