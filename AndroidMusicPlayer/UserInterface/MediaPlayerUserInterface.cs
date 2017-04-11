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

namespace AndroidMusicPlayer
{
    public class MediaPlayerUserInterface
    {
        private Context _context;
        private SoundPlayer _player;

        public delegate void DisposePlayerDelegate();

        public event DisposePlayerDelegate DisposeEvent;
        public MediaPlayerUserInterface(Context context, SoundPlayer player)
        {
            _context = context;
            _player = player;
        }

        public View CreatePlayer()
        {
            LinearLayout layout=new LinearLayout(_context,null,Android.Resource.Layout.SimpleListItem1);

            Button btnPlay=new Button(_context);
            btnPlay.Text = "Play";
            btnPlay.Click += (object sender, EventArgs e) => {  _player.Play(); };
            Button btnStop = new Button(_context);
            btnStop.Text = "Stop";
            btnStop.Click += (object sender, EventArgs e) => { _player.Stop();};
            Button btnPause = new Button(_context);
            btnPause.Text = "Pause";
            btnPause.Click += (object sender, EventArgs e) => {_player.Pause(); };
            Button btnBack=new Button(_context);
            btnBack.Text = "Back";
            btnBack.Click += (object sender, EventArgs e) => {_player.Dispose(); DisposeEvent?.Invoke(); };
            layout.AddView(btnPlay);
            layout.AddView(btnStop);
            layout.AddView(btnPause);
            layout.AddView(btnBack);
            return layout;

        }
    }
}