using Microsoft.Xna.Framework;
using InputHelper;

namespace MouseBuddy
{
	/// <summary>
	/// Interface for an object that manages some simple mouse state junk.
	/// </summary>
	public interface IMouseManager : IInputHelper
	{
		Vector2 MousePos
		{
			get;
		}
	}
}