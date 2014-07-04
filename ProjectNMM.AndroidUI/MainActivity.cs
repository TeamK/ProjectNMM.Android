using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Text;

using ProjectNMM.Model;

namespace ProjectNMM.AndroidUI
{
	[Activity (Label = "ProjectNMM", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private ModelControl _model;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_model = new ModelControl ();

			ShowStartScreen ();
		}

		private void ShowMainScreen ()
		{
			SetContentView (Resource.Layout.MainScreen);

			Button btnNewGame = FindViewById<Button> (Resource.Id.BtnNewGame);
			btnNewGame.Click += delegate {
				ShowStartScreen ();
			};
			Button btnAbout = FindViewById<Button> (Resource.Id.BtnAbout);
			btnAbout.Click += delegate {
				ShowAboutScreen ();
			};

			if (!_model.GameInProgress)
				return;

			btnNewGame.Visibility = ViewStates.Gone;
		}

		private void ShowStartScreen ()
		{
			SetContentView (Resource.Layout.StartScreen);

			Spinner spnGameMode = FindViewById<Spinner> (Resource.Id.SpnGameMode);
			ArrayAdapter adapter = ArrayAdapter.CreateFromResource (
				                       this, Resource.Array.StartScreen_SpnGameMode_Entries, Android.Resource.Layout.SimpleSpinnerItem);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spnGameMode.Adapter = adapter;

			Button btnStartGame = FindViewById<Button> (Resource.Id.BtnStartGame);
			btnStartGame.Click += delegate {
				StartNewGame ();
			};
		}

		private void ShowAboutScreen ()
		{
			SetContentView (Resource.Layout.AboutScreen);

			Button lblProjectSite = FindViewById<Button> (Resource.Id.LblProjectSite);
			//lblProjectSite.SetHtml (Html.FromHtml("<a href=\"https://github.com/TeamK/ProjectNMM\">Projektseite</a>"));

			Button btnBack = FindViewById<Button> (Resource.Id.BtnBack);
			btnBack.Click += delegate {
				ShowMainScreen ();
			};
		}

		private void StartNewGame ()
		{
			EditText txtNamePlayer1 = FindViewById<EditText> (Resource.Id.TxtNamePlayer1);
			EditText txtNamePlayer2 = FindViewById<EditText> (Resource.Id.TxtNamePlayer2);
			Spinner spnGameMode = FindViewById<Spinner> (Resource.Id.SpnGameMode);
			ArrayAdapter adapter = ArrayAdapter.CreateFromResource (
				                       this, Resource.Array.StartScreen_SpnGameMode_Entries, Android.Resource.Layout.SimpleSpinnerItem);

			if (string.IsNullOrEmpty (txtNamePlayer1.Text)) {
				txtNamePlayer1.SetBackgroundColor (Color.Red);
				return;
			} else
				txtNamePlayer1.SetBackgroundColor (Color.Transparent);
			if (string.IsNullOrEmpty (txtNamePlayer2.Text)) {
				txtNamePlayer2.SetBackgroundColor (Color.Red);
				return;
			} else
				txtNamePlayer2.SetBackgroundColor (Color.Transparent);

			switch (spnGameMode.SelectedItem.ToString ()) {
			case "Spieler gegen Spieler":
				_model.StartNewGame (GameType.PlayerVsPlayer);
				break;
			case "Spieler gegen Computer":
				_model.StartNewGame (GameType.PlayerVsMachine);
				break;
			case "Computer gegen Computer":
				_model.StartNewGame (GameType.MachineVsMachine);
				break;
			default:
				spnGameMode.SetBackgroundColor (Color.Red);
				return;
			}

			_model.PlayerName1 = txtNamePlayer1.Text;
			_model.PlayerName2 = txtNamePlayer2.Text;

			SetContentView (new PlayBoard (this, _model));
		}
	}
}


