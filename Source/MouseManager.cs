using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MouseBuddy
{
	/// <summary>
	/// MonoGame componenet that manages some simple mouse state junk.
	/// </summary>
	public class MouseManager : GameComponent, IMouseManager
	{
		#region Properties

		public MouseState CurrentMouseState { get; private set; }

		public MouseState LastMouseState { get; private set; }

		/// <summary>
		/// Get the mouse position... only used in certain games
		/// </summary>
		public Vector2 MousePos
		{
			get
			{
				return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
			}
		}

		/// <summary>
		/// Check for left mouse click... only used in certain games
		/// </summary>
		public bool LMouseClick
		{
			get
			{
				return ((LastMouseState.LeftButton == ButtonState.Pressed) && 
					(CurrentMouseState.LeftButton == ButtonState.Released));
			}
		}

		/// <summary>
		/// Check for left mouse down... only used in certain games
		/// </summary>
		public bool LMouseDown
		{
			get
			{
				return (CurrentMouseState.LeftButton == ButtonState.Pressed);
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public MouseManager(Game game)
			: base(game)
		{
			CurrentMouseState = new MouseState();
			LastMouseState = new MouseState();

			//Register ourselves to implement the DI container service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IMouseManager), this);
		}

		#endregion //Initialization

		#region Public Methods

		/// <summary>
		/// Reads the latest state of the keyboard and gamepad.
		/// </summary>
		public void Update()
		{
			LastMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState();
		}

		#endregion //Public Methods
	}
}