using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StateMachineBuddy;

namespace MouseBuddy
{
	/// <summary>
	/// MonoGame componenet that manages some simple mouse state junk.
	/// </summary>
	public class MouseComponent : GameComponent, IMouseManager
	{
		#region Properties

		public MouseManager MouseManager
		{
			get; private set;
		}

		public List<EventArgs> MouseEvents
		{
			get
			{
				return MouseManager.MouseEvents;
            }
		}

		public Vector2 MousePos
		{
			get
			{
				return MouseManager.MousePos;
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public MouseComponent(Game game)
			: base(game)
		{
			MouseManager = new MouseManager();

			//Register ourselves to implement the DI container service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IMouseManager), this);
		}
		
		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Reads the latest state of the keyboard and gamepad.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			MouseManager.Update(Game.IsActive);
		}

		#endregion //Methods
	}
}