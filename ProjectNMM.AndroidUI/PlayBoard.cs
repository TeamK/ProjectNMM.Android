using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using ProjectNMM.Model;

namespace ProjectNMM.AndroidUI
{
	public class PlayBoard : View
	{
		private ModelControl _model;
		private Android.Content.Res.Resources _res;
		private RectF[,] Playstones;
		private int _oneSeventh;

		public PlayBoard (Context context, ModelControl model) : base (context)
		{
			_model = model;
			_res = context.Resources;
			_oneSeventh = (int)_res.DisplayMetrics.WidthPixels / 7;

			InitializeArray ();
		}

		protected override void OnDraw (Canvas canvas)
		{
			DrawLines (canvas);
			DrawPlaystones (canvas);
			DrawInterface (canvas);
		}

		public override bool OnTouchEvent (MotionEvent mEvent)
		{
			switch (mEvent.Action) {
			case MotionEventActions.Down:
				break;
			case MotionEventActions.Up:
				Invalidate ();
				return true;
			default:
				return true;
			}

			float eventX = mEvent.GetX (),
			eventY = mEvent.GetY ();

			for (int index1 = 0; index1 < 7; index1++) {
				for (int index2 = 0; index2 < 7; index2++) {
					RectF rect = Playstones [index1, index2];

					if (rect == null)
						continue;

					if (eventX >= rect.Top && eventX <= rect.Bottom &&
					    eventY >= rect.Left && eventY <= rect.Right) {
						_model.PlaystoneChanged (index1, index2);

						return true;
					}
				}
			}
			return true;
		}

		private void DrawLines (Canvas canvas)
		{
			Paint lineColor = new Paint () {
				Color = Color.White,
				StrokeWidth = 10
			};
			float offset = _oneSeventh / 2.0f;

			#region First Square
			canvas.DrawLine (
				Playstones [0, 0].Top + offset,
				Playstones [0, 0].Left + offset,
				Playstones [6, 0].Top + offset,
				Playstones [6, 0].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [6, 0].Top + offset,
				Playstones [6, 0].Left + offset,
				Playstones [6, 6].Top + offset,
				Playstones [6, 6].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [6, 6].Top + offset,
				Playstones [6, 6].Left + offset,
				Playstones [0, 6].Top + offset,
				Playstones [0, 6].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [0, 6].Top + offset,
				Playstones [0, 6].Left + offset,
				Playstones [0, 0].Top + offset,
				Playstones [0, 0].Left + offset,
				lineColor
			);
			#endregion

			#region Second Square
			canvas.DrawLine (
				Playstones [1, 1].Top + offset,
				Playstones [1, 1].Left + offset,
				Playstones [5, 1].Top + offset,
				Playstones [5, 1].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [5, 1].Top + offset,
				Playstones [5, 1].Left + offset,
				Playstones [5, 5].Top + offset,
				Playstones [5, 5].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [5, 5].Top + offset,
				Playstones [5, 5].Left + offset,
				Playstones [1, 5].Top + offset,
				Playstones [1, 5].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [1, 5].Top + offset,
				Playstones [1, 5].Left + offset,
				Playstones [1, 1].Top + offset,
				Playstones [1, 1].Left + offset,
				lineColor
			);
			#endregion

			#region Third Square
			canvas.DrawLine (
				Playstones [2, 2].Top + offset,
				Playstones [2, 2].Left + offset,
				Playstones [4, 2].Top + offset,
				Playstones [4, 2].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [4, 2].Top + offset,
				Playstones [4, 2].Left + offset,
				Playstones [4, 4].Top + offset,
				Playstones [4, 4].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [4, 4].Top + offset,
				Playstones [4, 4].Left + offset,
				Playstones [2, 4].Top + offset,
				Playstones [2, 4].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [2, 4].Top + offset,
				Playstones [2, 4].Left + offset,
				Playstones [2, 2].Top + offset,
				Playstones [2, 2].Left + offset,
				lineColor
			);
			#endregion

			#region Middle Lines
			canvas.DrawLine (
				Playstones [3, 0].Top + offset,
				Playstones [3, 0].Left + offset,
				Playstones [3, 2].Top + offset,
				Playstones [3, 2].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [4, 3].Top + offset,
				Playstones [4, 3].Left + offset,
				Playstones [6, 3].Top + offset,
				Playstones [6, 3].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [3, 4].Top + offset,
				Playstones [3, 4].Left + offset,
				Playstones [3, 6].Top + offset,
				Playstones [3, 6].Left + offset,
				lineColor
			);
			canvas.DrawLine (
				Playstones [2, 3].Top + offset,
				Playstones [2, 3].Left + offset,
				Playstones [0, 3].Top + offset,
				Playstones [0, 3].Left + offset,
				lineColor
			);
			#endregion
		}

		private void DrawPlaystones (Canvas canvas)
		{
			for (int index1 = 0; index1 < 7; index1++) {
				for (int index2 = 0; index2 < 7; index2++) {
					RectF rect = Playstones [index1, index2];

					if (rect != null) {
						Paint paint = new Paint ();

						switch (_model.Playstones [index2, index1]) {
						case PlaystoneState.Neutral:
							paint.Color = Color.Black;
							break;
						case PlaystoneState.Player1:
							paint.Color = Color.Blue;
							break;
						case PlaystoneState.Player2:
							paint.Color = Color.Red;
							break;
						case PlaystoneState.Selectable:
							paint.Color = Color.Gray;
							break;
						}

						canvas.DrawOval (rect, paint);
					}
				}
			}
		}

		private void DrawInterface (Canvas canvas)
		{
			Color colorPlayer1;
			Color colorPlayer2;

			if (_model.ActivePlayer == PlaystoneState.Player1) {
				colorPlayer1 = Color.Blue;
				colorPlayer2 = Color.White;
			} else {
				colorPlayer1 = Color.White;
				colorPlayer2 = Color.Red;
			}

			canvas.DrawText (
				_model.PlayerName1 + " (Spielsteine: " + Convert.ToString (_model.PlaystonesPlayer1) + ")",
				10, Playstones [0, 6].Bottom + 30, new Paint () {
				Color = colorPlayer1,
				TextSize = 30
			});
			canvas.DrawText (
				GetGameEvent (PlaystoneState.Player1),
				10, Playstones [0, 6].Bottom + 65, new Paint () {
				Color = Color.White,
				TextSize = 15
			});

			canvas.DrawText (
				_model.PlayerName2 + " (Spielsteine: " + Convert.ToString (_model.PlaystonesPlayer2) + ")",
				10, Playstones [0, 6].Bottom + 100, new Paint () {
				Color = colorPlayer2,
				TextSize = 30
			});
			canvas.DrawText (
				GetGameEvent (PlaystoneState.Player2),
				10, Playstones [0, 6].Bottom + 135, new Paint () {
				Color = Color.White,
				TextSize = 15
			});
		}

		private void InitializeArray ()
		{
			Playstones = new RectF[7, 7];
			for (int index1 = 0; index1 < 7; index1++) {
				for (int index2 = 0; index2 < 7; index2++) {
					Playstones [index1, index2] = null;
				}
			}

			#region First Square
			Playstones [0, 0] = new RectF () {
				Top = _oneSeventh * 0,
				Left = _oneSeventh * 0,
				Bottom = _oneSeventh * 0 + _oneSeventh,
				Right = _oneSeventh * 0 + _oneSeventh
			};
			Playstones [3, 0] = new RectF () {
				Top = _oneSeventh * 0,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 0 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [6, 0] = new RectF () {
				Top = _oneSeventh * 0,
				Left = _oneSeventh * 6,
				Bottom = _oneSeventh * 0 + _oneSeventh,
				Right = _oneSeventh * 6 + _oneSeventh
			};
			Playstones [6, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 6,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 6 + _oneSeventh
			};
			Playstones [6, 6] = new RectF () {
				Top = _oneSeventh * 6,
				Left = _oneSeventh * 6,
				Bottom = _oneSeventh * 6 + _oneSeventh,
				Right = _oneSeventh * 6 + _oneSeventh
			};
			Playstones [3, 6] = new RectF () {
				Top = _oneSeventh * 6,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 6 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [0, 6] = new RectF () {
				Top = _oneSeventh * 6,
				Left = _oneSeventh * 0,
				Bottom = _oneSeventh * 6 + _oneSeventh,
				Right = _oneSeventh * 0 + _oneSeventh
			};
			Playstones [0, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 0,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 0 + _oneSeventh
			};
			#endregion

			#region Second Square
			Playstones [1, 1] = new RectF () {
				Top = _oneSeventh * 1,
				Left = _oneSeventh * 1,
				Bottom = _oneSeventh * 1 + _oneSeventh,
				Right = _oneSeventh * 1 + _oneSeventh
			};
			Playstones [3, 1] = new RectF () {
				Top = _oneSeventh * 1,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 1 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [5, 1] = new RectF () {
				Top = _oneSeventh * 1,
				Left = _oneSeventh * 5,
				Bottom = _oneSeventh * 1 + _oneSeventh,
				Right = _oneSeventh * 5 + _oneSeventh
			};
			Playstones [5, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 5,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 5 + _oneSeventh
			};
			Playstones [5, 5] = new RectF () {
				Top = _oneSeventh * 5,
				Left = _oneSeventh * 5,
				Bottom = _oneSeventh * 5 + _oneSeventh,
				Right = _oneSeventh * 5 + _oneSeventh
			};
			Playstones [3, 5] = new RectF () {
				Top = _oneSeventh * 5,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 5 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [1, 5] = new RectF () {
				Top = _oneSeventh * 5,
				Left = _oneSeventh * 1,
				Bottom = _oneSeventh * 5 + _oneSeventh,
				Right = _oneSeventh * 1 + _oneSeventh
			};
			Playstones [1, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 1,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 1 + _oneSeventh
			};
			#endregion

			#region Third Square
			Playstones [2, 2] = new RectF () {
				Top = _oneSeventh * 2,
				Left = _oneSeventh * 2,
				Bottom = _oneSeventh * 2 + _oneSeventh,
				Right = _oneSeventh * 2 + _oneSeventh
			};
			Playstones [3, 2] = new RectF () {
				Top = _oneSeventh * 2,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 2 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [4, 2] = new RectF () {
				Top = _oneSeventh * 2,
				Left = _oneSeventh * 4,
				Bottom = _oneSeventh * 2 + _oneSeventh,
				Right = _oneSeventh * 4 + _oneSeventh
			};
			Playstones [4, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 4,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 4 + _oneSeventh
			};
			Playstones [4, 4] = new RectF () {
				Top = _oneSeventh * 4,
				Left = _oneSeventh * 4,
				Bottom = _oneSeventh * 4 + _oneSeventh,
				Right = _oneSeventh * 4 + _oneSeventh
			};
			Playstones [3, 4] = new RectF () {
				Top = _oneSeventh * 4,
				Left = _oneSeventh * 3,
				Bottom = _oneSeventh * 4 + _oneSeventh,
				Right = _oneSeventh * 3 + _oneSeventh
			};
			Playstones [2, 4] = new RectF () {
				Top = _oneSeventh * 4,
				Left = _oneSeventh * 2,
				Bottom = _oneSeventh * 4 + _oneSeventh,
				Right = _oneSeventh * 2 + _oneSeventh
			};
			Playstones [2, 3] = new RectF () {
				Top = _oneSeventh * 3,
				Left = _oneSeventh * 2,
				Bottom = _oneSeventh * 3 + _oneSeventh,
				Right = _oneSeventh * 2 + _oneSeventh
			};
			#endregion
		}

		private string GetGameEvent (PlaystoneState player)
		{
			GameEvent gameEvent;

			if (player == PlaystoneState.Player1)
				gameEvent = _model.GameEventPlayer1;
			else
				gameEvent = _model.GameEventPlayer2;

			switch (gameEvent) {
			case GameEvent.PlayerHasMill:
				return _res.GetString (Resource.String.Playevent_PlayerHasMill);
			case GameEvent.WrongPlaystoneAfterMill:
				return _res.GetString (Resource.String.Playevent_WrongPlaystoneAfterMill);
			case GameEvent.CannotBreakMill:
				return _res.GetString (Resource.String.Playevent_CannotBreakMill);
			case GameEvent.InvalidPlaystone:
				return _res.GetString (Resource.String.Playevent_InvalidPlaystone);
			case GameEvent.NoPlaystonesRemovable:
				return _res.GetString (Resource.String.Playevent_NoPlaystonesRemovable);
			case GameEvent.GameOverNoMovesPossible:
				return _res.GetString (Resource.String.Playevent_GameOverNoMovesPossible);
			case GameEvent.GameOverNoPlaystonesLeft:
				return _res.GetString (Resource.String.Playevent_GameOverNoPlaystonesLeft);
			case GameEvent.FinishTurn:
				return _res.GetString (Resource.String.Playevent_FinishTurn);
			case GameEvent.CannotUndoMill:
				return _res.GetString (Resource.String.Playevent_CannotUndoMill);
			default:
				return _res.GetString (Resource.String.Playevent_NoEvent);
			}
		}
	}
}

