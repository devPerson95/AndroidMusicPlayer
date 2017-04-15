using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;

namespace AndroidMusicPlayer
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : Activity
    {
        private Player _player;
        private ProgressBar _progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Initialization();
            base.OnCreate(savedInstanceState);
           
            // Create your application here
        }

        private void Initialization()
        {
            SetContentView(Resource.Layout.PlayerLayout);
            var textView = FindViewById<TextView>(Resource.Id.NameTextView);
            var item = Intent.GetStringExtra("File");
            var file = JsonConvert.DeserializeObject<ExplorerListViewModel>(item);
            textView.Text = file.Name;
            var btnPlay = FindViewById<ImageButton>(Resource.Id.PlayBtn);
            btnPlay.Click += BtnPlay_click;
            var btnPause = FindViewById<ImageButton>(Resource.Id.pauseBtn);
            btnPause.Click += BtnPause_click;
            var btnStop = FindViewById<ImageButton>(Resource.Id.stopBtn);
            btnStop.Click += BtnStop_click;
            _player = new Player(file.FullPath);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
        }
        public void BtnPlay_click(object sender, EventArgs e)
        {
         _player.Play();
           UpdateProgress();

        }

        public  void UpdateProgress()
        {
            
            new Thread(new Runnable(Update)).Start();
          
        }

        public void Update()
        {
            while (_player.IsPlaying)
            {

                _progressBar.Progress = _player.GetProgress();


            }
        }
        public void BtnPause_click(object sender, EventArgs e)
        {
            _player.Pause();
        }
        public void BtnStop_click(object sender, EventArgs e)
        {
            _player.Stop();
        }
    }
}