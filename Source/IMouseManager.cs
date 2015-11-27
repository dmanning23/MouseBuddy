using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MouseBuddy
{
	/// <summary>
	/// Interface for an object that manages some simple mouse state junk.
	/// </summary>
	public interface IMouseManager : IUpdateable
	{
		/// <summary>
		/// Get the list of current mouse events.
		/// This list is flushed and repopulated every update.
		/// </summary>
		List<EventArgs> MouseEvents
		{
			get;
		}
	}
}