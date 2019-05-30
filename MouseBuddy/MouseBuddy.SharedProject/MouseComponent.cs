using InputHelper;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

		public Vector2 MousePos
		{
			get
			{
				return MouseManager.MousePos;
			}
		}

		public List<ClickEventArgs> Clicks
		{
			get
			{
				return MouseManager.Clicks;
			}
		}

		public List<HighlightEventArgs> Highlights
		{
			get
			{
				return MouseManager.Highlights;
			}
		}

		public List<DragEventArgs> Drags
		{
			get
			{
				return MouseManager.Drags;
			}
		}

		public List<DropEventArgs> Drops
		{
			get
			{
				return MouseManager.Drops;
			}
		}

		public List<FlickEventArgs> Flicks
		{
			get
			{
				return MouseManager.Flicks;
			}
		}

		public List<PinchEventArgs> Pinches
		{
			get
			{
				return MouseManager.Pinches;
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public MouseComponent(Game game, ConvertToGameCoord gameCoord)
			: base(game)
		{
			MouseManager = new MouseManager(gameCoord);

			//Register ourselves to implement the DI container service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IInputHelper), this);
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