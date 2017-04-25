using System;
using System.Collections.Generic;
using System.IO;
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
   public class Player:IDisposable
   {
       private MediaPlayer _player;
       private string _path;
       private bool isStoped;
       public bool IsPlaying { get { return _player.IsPlaying; } }
        public string Path { get { return _path; } }
       public Player(string path)
       {
           _player=new MediaPlayer();
           _path = path;
           isStoped = true;
           

       }

       public void SetPath(string path)
       {
           var file = new FileInfo(path);
           if (file.Exists)
           {
               _path = path;
           }
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

           return _player.Duration;


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
           if (!isStoped)
           {
                Stop();
            }
           _player.Release();
          _player.Dispose();
           
            
       }
    }
}