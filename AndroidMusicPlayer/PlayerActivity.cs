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
using Newtonsoft.Json;

namespace AndroidMusicPlayer
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : Activity
    {
        private SoundPlayer _player;
        private ProgressBar _progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PlayerLayout);
            var textView = FindViewById<TextView>(Resource.Id.NameTextView);
            var item = Intent.GetStringExtra("File");
            var file = JsonConvert.DeserializeObject<FileListViewModel>(item);
            textView.Text = file.Name;
            var btnPlay = FindViewById<Button>(Resource.Id.playBtn);
            btnPlay.Click += BtnPlay_click;
            var btnPause = FindViewById<ImageButton>(Resource.Id.PauseBtn);
            btnPause.Click += BtnPause_click;
            var btnStop = FindViewById<Button>(Resource.Id.StopBtn);
            btnStop.Click += BtnStop_click;
            _player = new SoundPlayer(file.FullPath);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            // Create your application here
        }

        public void BtnPlay_click(object sender, EventArgs e)
        {
         _player.Play();
            UpdateProgress();
            
                
           
        }

        public async void UpdateProgress()
        {
            while (_player.IsPlaying)
            {
                _progressBar.Progress = await _player.GetProgressAsync();
                
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