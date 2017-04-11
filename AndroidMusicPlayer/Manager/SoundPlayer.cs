using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;

namespace AndroidMusicPlayer
{
   public class SoundPlayer
   {
       private MediaPlayer _player;
       private string _path;
       private bool isStoped;
       public bool IsPlaying { get { return _player.IsPlaying; } }

       public delegate void MediaProgressDelegate(int progress);

       public event MediaProgressDelegate MediaProgressEvent;
       public SoundPlayer(string path)
       {
           _player=new MediaPlayer();
           _path = path;
           isStoped = true;
           

       }

       
       public void Play()
       {
           if (!_player.IsPlaying)
           {
               if (isStoped)
               {
                    _player.SetDataSource(_path);
                    _player.Prepare();
                   isStoped = false;
               }
             
                _player.Start();
            }
               
           
       }

       public Task<int> GetProgressAsync()
       {
           return Task<int>.Factory.StartNew(() => { return (_player.CurrentPosition*100)/_player.Duration; });
       }

       public int GetProgress()
       {
           
              return _player.CurrentPosition;


       }

       public void Pause()
       {
           if (_player.IsPlaying)
           {
                _player.Pause();
           }
           
       }
        public void Stop()
        {
            if (_player.IsPlaying)
            {
                _player.Stop();
                _player.Reset();
                isStoped = true;
            }

        }

       public void Dispose()
       {
           _player.Release();
            _player.Dispose();
           
            
       }
    }
}