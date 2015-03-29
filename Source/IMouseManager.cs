using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MouseBuddy
{
	/// <summary>
	/// Interface for an object that manages some simple mouse state junk.
	/// </summary>
	public interface IMouseManager : IUpdateable
	{
		MouseState CurrentMouseState { get; }

		MouseState LastMouseState { get; }

		/// <summary>
		/// Get the mouse position
		/// </summary>
		Vector2 MousePos { get; }

		/// <summary>
		/// Check for left mouse click
		/// </summary>
		bool LMouseClick { get; }

		/// <summary>
		/// Check for left mouse button held down
		/// </summary>
		bool LMouseDown { get; }
	}
}