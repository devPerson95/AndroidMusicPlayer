using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using AndroidMusicPlayer.Manager;
using Java.Lang;
using Newtonsoft.Json;
using Java.Util;
using Orientation = Android.Widget.Orientation;

namespace AndroidMusicPlayer
{
    [Activity(Label = "AndroidMusicPlayer", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar")]
    public class MainActivity : Activity
    {
       
        private Button _openfileButton;
        private ListView listViewItem;
        private ExplorerListView itemsView;
        private FileExplorer _fileExplorer;
        private FileViewAdapter _adapter;
        private LinearLayout linear;
        private ProgressBar progressBar;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
           
           
            _openfileButton = FindViewById<Button>(Resource.Id.OpenFileBtn);
            linear = FindViewById<LinearLayout>(Resource.Id.MainLayout);
            _openfileButton.Click += OpenFileButton_Click;
           
            listViewItem=new ListView(this,null,Android.Resource.Layout.SimpleListItem1);
            var startPath= Android.OS.Environment.ExternalStorageDirectory.Path;
            _fileExplorer = new FileExplorer(startPath);
            itemsView = new ExplorerListView(this, listViewItem,_fileExplorer);

        }

        
        protected  void OpenFileButton_Click(object sender, EventArgs e)
        {
           
           
            CallbackFileExplorer();
            progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleSmall);
            linear.AddView(progressBar);

        }

        public async  void CallbackFileExplorer()
        {
            List<FileListViewModel> items=new List<FileListViewModel>();
           
            var item = _fileExplorer.GetDirectoryAsync();
            var itemFile = _fileExplorer.GetFileAsync();
            items.AddRange(await  item);
            items.AddRange(await itemFile);
           
            linear.RemoveView(_openfileButton);
            linear.RemoveView(progressBar);
            linear.AddView(listViewItem, new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            _adapter = new FileViewAdapter(this,  items.ToArray());
            itemsView.SetCurrentAdapter(_adapter);
            itemsView.DirectoryClick += Directory_click;
            itemsView.FileClick += File_click;
        }
        
        public async void Directory_click(FileListViewModel item)
        {   List<FileListViewModel> listView=new List<FileListViewModel>();
            _fileExplorer.Path = item.FullPath;
            if (!_fileExplorer.IsStartDirectory)
            {
                listView.Add(_fileExplorer.GetPreviousDirectory());
            }
            
            listView.AddRange(await _fileExplorer.GetDirectoryAsync());
            listView.AddRange(await _fileExplorer.GetFileAsync());
            
            var adapter = new FileViewAdapter(this, listView.ToArray());
            itemsView.SetCurrentAdapter(adapter);
        }
        public void File_click(FileListViewModel item)
        {
            Intent playerActivity=new Intent(this,typeof(PlayerActivity));
            var itemJson = JsonConvert.SerializeObject(item);
            playerActivity.PutExtra("File", itemJson);
            StartActivity(playerActivity);
            //SoundPlayer player = new SoundPlayer(item.FullPath);
            //MediaPlayerUserInterface userInterface=new MediaPlayerUserInterface(this,player);
            //SetContentView(userInterface.CreatePlayer());
            //userInterface.DisposeEvent += () => { SetContentView(Resource.Layout.Main); };
        }



    }
}















