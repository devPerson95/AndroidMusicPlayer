using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;

namespace AndroidMusicPlayer
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : Activity
    {
        private Player _player;
        private ImageButton _playBtn;
        private ImageButton _pauseBtn;
        private ImageButton _stopBtn;
        private ProgressBar _progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Initialization();
            base.OnCreate(savedInstanceState);
           
            // Create your application here
        }

        protected override void OnDestroy()
        {
            Dispose();
            base.OnDestroy();
        }

        private void Initialization()
        {
            SetContentView(Resource.Layout.PlayerLayout);
            var textView = FindViewById<TextView>(Resource.Id.NameTextView);
            var item = Intent.GetStringExtra("File");
            var file = JsonConvert.DeserializeObject<ExplorerListViewModel>(item);
            textView.Text = file.Name;
             _playBtn = FindViewById<ImageButton>(Resource.Id.PlayBtn);
            _playBtn.Click += PlayBtn_click;
             _pauseBtn = FindViewById<ImageButton>(Resource.Id.pauseBtn);
            _pauseBtn.Click += PauseBtn_click;
             _stopBtn = FindViewById<ImageButton>(Resource.Id.stopBtn);
            _stopBtn.Click += StopBtn_click;
            _player = new Player(file.FullPath);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            
        }

        private void Dispose()
        {
            _playBtn.Click -= PlayBtn_click;
            _pauseBtn.Click -= PauseBtn_click;
            _stopBtn.Click -= StopBtn_click;
            _player.Dispose();
        }
        public void PlayBtn_click(object sender, EventArgs e)
        {
         _player.Play();
           UpdateProgress();

        }

        public  void UpdateProgress()
        {

            var animator = ObjectAnimator.OfInt(_progressBar, "progress", 0, 100);
            animator.SetDuration(_player.GetProgress());
            animator.Start();


        }

        
        public void PauseBtn_click(object sender, EventArgs e)
        {
            _player.Pause();
        }
        public void StopBtn_click(object sender, EventArgs e)
        {
            _player.Stop();
        }
    }
}