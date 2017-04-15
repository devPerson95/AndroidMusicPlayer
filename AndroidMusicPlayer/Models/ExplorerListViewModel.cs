using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidMusicPlayer
{
   public class ExplorerListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsFile { get; set; }
    }
}