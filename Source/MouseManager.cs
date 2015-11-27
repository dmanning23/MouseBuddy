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
	public class MouseManager : GameComponent, IMouseManager
	{
		#region Properties

		/// <summary>
		/// pixels^2 from start position to register as a drag and not a click
		/// </summary>
		private const float DragDelta = 25f;

		private MouseState CurrentMouseState { get; set; }

		private MouseState LastMouseState { get; set; }

		private StateMachine LeftButtonState
		{
			get; set;
		}

		private StateMachine RightButtonState
		{
			get; set;
		}

		private Vector2[] ClickStartPosition { get; set; }

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
		/// The left mouse button was up last time we checked, is down now.
		/// </summary>
		private bool LMouseClick
		{
			get
			{
				return ((LastMouseState.LeftButton == ButtonState.Released) &&
					(CurrentMouseState.LeftButton == ButtonState.Pressed));
			}
		}

		/// <summary>
		/// The left mouse button is held down.
		/// </summary>
		private bool LMouseDown
		{
			get
			{
				return (CurrentMouseState.LeftButton == ButtonState.Pressed);
			}
		}

		/// <summary>
		/// The left mouse button was held, now is released.
		/// </summary>
		private bool LMouseRelease
		{
			get
			{
				return ((LastMouseState.LeftButton == ButtonState.Pressed) &&
					(CurrentMouseState.LeftButton == ButtonState.Released));
			}
		}

		// <summary>
		/// The right mouse button was up last time we checked, is down now.
		/// </summary>
		private bool RMouseClick
		{
			get
			{
				return ((LastMouseState.RightButton == ButtonState.Released) &&
					(CurrentMouseState.RightButton == ButtonState.Pressed));
			}
		}

		/// <summary>
		/// The right mouse button is held down.
		/// </summary>
		private bool RMouseDown
		{
			get
			{
				return (CurrentMouseState.RightButton == ButtonState.Pressed);
			}
		}

		/// <summary>
		/// The right mouse button was held, now is released.
		/// </summary>
		private bool RMouseRelease
		{
			get
			{
				return ((LastMouseState.RightButton == ButtonState.Pressed) &&
					(CurrentMouseState.RightButton == ButtonState.Released));
			}
		}

		public List<EventArgs> MouseEvents
		{
			get;
			private set;
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
			MouseEvents = new List<EventArgs>();

			ClickStartPosition = new Vector2[Enum.GetValues(typeof(MouseButton)).Length];

			//set up state machines
			InitStateMachine(LeftButtonState);
			LeftButtonState.StateChangedEvent += OnLButtonStateChange;
			InitStateMachine(RightButtonState);
			RightButtonState.StateChangedEvent += OnRButtonStateChange;

			//Register ourselves to implement the DI container service.
			game.Components.Add(this);
			game.Services.AddService(typeof(IMouseManager), this);
		}

		private void InitStateMachine(StateMachine states)
		{
			states.Set(typeof(MouseButtonState), typeof(MouseButtonMessage));

			//go back to neutral on lost focus
			for (int i = 0; i < states.NumStates; i++)
			{
				states.SetEntry(i, (int)MouseButtonMessage.LostFocus, (int)MouseButtonState.Neutral);
			}

			states.SetEntry((int)MouseButtonState.Neutral,
				(int)MouseButtonMessage.Press,
				(int)MouseButtonState.Held);

			states.SetEntry((int)MouseButtonState.Held,
				(int)MouseButtonMessage.Move,
				(int)MouseButtonState.Dragging);

			states.SetEntry((int)MouseButtonState.Held,
				(int)MouseButtonMessage.Release,
				(int)MouseButtonState.Neutral);

			states.SetEntry((int)MouseButtonState.Dragging,
				(int)MouseButtonMessage.Release,
				(int)MouseButtonState.Neutral);
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Reads the latest state of the keyboard and gamepad.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			//update the mouse states
			LastMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState();

			//clear the mouse events
			MouseEvents.Clear();

			if (Game.IsActive)
			{
				//check if mouse is clicked
				if (LMouseClick)
				{
					LeftButtonState.SendStateMessage((int)MouseButtonMessage.Press);
				}
				else if (LMouseDown)
				{
					var startPos = ClickStartPosition[(int)MouseButton.Left];
					if ((startPos - MousePos).LengthSquared() > DragDelta)
					{
						LeftButtonState.SendStateMessage((int)MouseButtonMessage.Move);
					}
				}
				else if (LMouseRelease)
				{
					LeftButtonState.SendStateMessage((int)MouseButtonMessage.Release);
				}

				//check the r mouse click
				if (RMouseClick)
				{
					RightButtonState.SendStateMessage((int)MouseButtonMessage.Press);
				}
				else if (RMouseRelease)
				{
					RightButtonState.SendStateMessage((int)MouseButtonMessage.Release);
				}
			}
			else
			{
				LeftButtonState.SendStateMessage((int)MouseButtonMessage.LostFocus);
				RightButtonState.SendStateMessage((int)MouseButtonMessage.LostFocus);
			}
		}

		private void OnLButtonStateChange(Object obj, StateChangeEventArgs e)
		{
			OnButtonStateChange(e.OldState, e.NewState, MouseButton.Left);
		}

		private void OnRButtonStateChange(Object obj, StateChangeEventArgs e)
		{
			OnButtonStateChange(e.OldState, e.NewState, MouseButton.Right);
		}

		private void OnButtonStateChange(int oldState, int newState, MouseButton button)
		{
			switch (oldState)
			{
				case (int)MouseButtonState.Neutral:
					{
						if (newState == (int)MouseButtonState.Held)
						{
							//fire off button down event
							ClickStartPosition[(int)button] = MousePos;
							MouseEvents.Add(new ButtonDownEventArgs()
							{
								Position = MousePos,
								Button = button
							});
						}
					}
					break;
				case (int)MouseButtonState.Held:
					{
						switch (newState)
						{
							case (int)MouseButtonState.Neutral:
								{
									//fire off click event
									MouseEvents.Add(new ClickEventArgs()
									{
										Position = MousePos,
										Button = button
									});
								}
								break;
							case (int)MouseButtonState.Dragging:
								{
									//fire off dragging event
									MouseEvents.Add(new DragEventArgs()
									{
										Start = ClickStartPosition[(int)button],
										Current = MousePos,
										Button = button
									});
								}
								break;
						}
					}
					break;
				case (int)MouseButtonState.Dragging:
					{
						if (newState == (int)MouseButtonState.Neutral)
						{
							//fire off drop event
							MouseEvents.Add(new DropEventArgs()
							{
								Start = ClickStartPosition[(int)button],
								Drop = MousePos,
								Button = button
							});
						}
					}
					break;
			}
		}

		#endregion //Methods
	}
}