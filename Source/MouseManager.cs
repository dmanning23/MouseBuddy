using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StateMachineBuddy;
using InputHelper;

namespace MouseBuddy
{
	/// <summary>
	/// MonoGame componenet that manages some simple mouse state junk.
	/// </summary>
	public class MouseManager : IMouseManager
	{
		#region Properties

		/// <summary>
		/// pixels^2 from start position to register as a drag and not a click
		/// </summary>
		private const float DragDelta = 25f;

		private MouseState CurrentMouseState { get; set; }

		private MouseState LastMouseState { get; set; }

		public StateMachine LeftButtonState
		{
			get; private set;
		}

		public StateMachine RightButtonState
		{
			get; private set;
		}

		private Vector2[] ClickStartPosition { get; set; }

		/// <summary>
		/// Get the mouse position... only used in certain games
		/// </summary>
		public virtual Vector2 MousePos
		{
			get
			{
				return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
			}
		}

		/// <summary>
		/// The left mouse button was up last time we checked, is down now.
		/// </summary>
		public virtual bool LMouseClick
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
		public virtual bool LMouseDown
		{
			get
			{
				return (CurrentMouseState.LeftButton == ButtonState.Pressed);
			}
		}

		/// <summary>
		/// The left mouse button was held, now is released.
		/// </summary>
		public virtual bool LMouseRelease
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
		public virtual bool RMouseClick
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
		public virtual bool RMouseDown
		{
			get
			{
				return (CurrentMouseState.RightButton == ButtonState.Pressed);
			}
		}

		/// <summary>
		/// The right mouse button was held, now is released.
		/// </summary>
		public virtual bool RMouseRelease
		{
			get
			{
				return ((LastMouseState.RightButton == ButtonState.Pressed) &&
					(CurrentMouseState.RightButton == ButtonState.Released));
			}
		}

		public List<ClickEventArgs> Clicks
		{
			get; private set;
		}

		public List<HighlightEventArgs> Highlights
		{
			get; private set;
		}

		public List<DragEventArgs> Drags
		{
			get; private set;
		}

		public List<DropEventArgs> Drops
		{
			get; private set;
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// Constructs a new input state.
		/// </summary>
		public MouseManager()
		{
			CurrentMouseState = new MouseState();
			LastMouseState = new MouseState();

			Clicks = new List<ClickEventArgs>();
			Highlights = new List<HighlightEventArgs>();
			Drags = new List<DragEventArgs>();
			Drops = new List<DropEventArgs>();

			ClickStartPosition = new Vector2[Enum.GetValues(typeof(MouseButton)).Length];

			//set up state machines
			LeftButtonState = new StateMachine();
			InitStateMachine(LeftButtonState);
			LeftButtonState.StateChangedEvent += OnLButtonStateChange;

			RightButtonState = new StateMachine();
			InitStateMachine(RightButtonState);
			RightButtonState.StateChangedEvent += OnRButtonStateChange;
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
		/// Update the mouse manager.
		/// </summary>
		public void Update(bool isActive)
		{
			//update the mouse states
			LastMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState();

			//clear the mouse events
			Clicks.Clear();
			Highlights.Clear();
			Drags.Clear();
			Drops.Clear();

			if (isActive)
			{
				//create the highlight event
				Highlights.Add(new HighlightEventArgs()
				{
					Position = MousePos
				});

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

					//if there is a drag event going on, send MouseEvents.Add(new DragEventArgs()
					if ((int)MouseButtonState.Dragging == LeftButtonState.CurrentState)
					{
						Drags.Add(new DragEventArgs()
						{
							Start = ClickStartPosition[(int)MouseButton.Left],
							Current = MousePos,
							Delta = (CurrentMouseState.Position - LastMouseState.Position).ToVector2(),
							Button = MouseButton.Left
						});
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
							//MouseEvents.Add(new ButtonDownEventArgs()
							//{
							//	Position = MousePos,
							//	Button = button
							//});
						}
					}
					break;
				case (int)MouseButtonState.Held:
					{
						if ((int)MouseButtonState.Neutral == newState)
						{
							//fire off click event
							Clicks.Add(new ClickEventArgs()
							{
								Position = MousePos,
								Button = button
							});
						}
					}
					break;
				case (int)MouseButtonState.Dragging:
					{
						if (newState == (int)MouseButtonState.Neutral)
						{
							//Add the last drag event to get the end of the line
							Drags.Add(new DragEventArgs()
							{
								Start = ClickStartPosition[(int)MouseButton.Left],
								Current = MousePos,
								Delta = (CurrentMouseState.Position - LastMouseState.Position).ToVector2(),
								Button = MouseButton.Left
							});

							//fire off drop event
							Drops.Add(new DropEventArgs()
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