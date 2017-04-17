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
       
        private Button _internalStorageBtn;
        private Button _externalStorageBtn;
        private ListView _listView;
        private ExplorerListView _explorerList;
        private FileExplorer _fileExplorer;
        private ExplorerViewAdapter _adapter;
        private LinearLayout _layout;
        private ProgressBar _progressBar;
        private Player _player;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
           
           
           Initialization();

        }

        protected override void OnDestroy()
        {
           this.Dispose();
            base.OnDestroy();
        }

        
        private void Initialization()
        {
            _internalStorageBtn = FindViewById<Button>(Resource.Id.internalStorageBtn);
            _externalStorageBtn = FindViewById<Button>(Resource.Id.externalStorageBtn);
            _layout = FindViewById<LinearLayout>(Resource.Id.MainLayout);
            _internalStorageBtn.Click += InternalStorageButton_Click;
            _externalStorageBtn.Click += ExternalStorageButton_Click;
            _listView = new ListView(this, null, Android.Resource.Layout.SimpleListItem1);
            _fileExplorer = new FileExplorer();
            _explorerList = new ExplorerListView(_listView, _fileExplorer);
        }
        private void Dispose()
        {
         _listView.Dispose();
        _layout.Dispose();
        _player.Dispose();
    }
        public string GetInternalStorage()
        {
            var storages = GetExternalFilesDirs(null);
            var internalStorage = storages[0];
            var directories = internalStorage.AbsolutePath.Split('/');
            var path = Path.Combine(directories[1], directories[2],directories[3]);
            return path;
        }
        public string GetExternalStorage()
        {
            var storages = GetExternalFilesDirs(null);
            var externalStorage = storages[1];
            var directories =externalStorage.AbsolutePath.Split('/');
            var path = Path.Combine(directories[1], directories[2]);
            return path;
        }
        protected  void InternalStorageButton_Click(object sender, EventArgs e)
        {
            var startPath = GetInternalStorage();
            _fileExplorer.Path = startPath;
            CallbackFileExplorer();
            _progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleSmall);
            _layout.AddView(_progressBar);

        }
        protected void ExternalStorageButton_Click(object sender, EventArgs e)
        {
            var startPath = GetExternalStorage();
            _fileExplorer.Path = startPath;
            CallbackFileExplorer();
            _progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleSmall);
            _layout.AddView(_progressBar);

        }
        public async  void CallbackFileExplorer()
        {
            List<ExplorerListViewModel> items=new List<ExplorerListViewModel>();
           
            var item = _fileExplorer.GetDirectoryAsync();
            var itemFile = _fileExplorer.GetFileAsync();
            items.AddRange(await  item);
            items.AddRange(await itemFile);
            _internalStorageBtn.Click -= InternalStorageButton_Click;
            _externalStorageBtn.Click -= ExternalStorageButton_Click;
            _layout.RemoveView(_internalStorageBtn);
            _internalStorageBtn.Dispose();
            _layout.RemoveView(_progressBar);
            _progressBar.Dispose();
            _layout.RemoveView(_externalStorageBtn);
            _externalStorageBtn.Dispose();
            _layout.AddView(_listView, new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            _adapter = new ExplorerViewAdapter(this,  items.ToArray());
            _explorerList.SetCurrentAdapter(_adapter);
            _explorerList.DirectoryClick += Directory_click;
            _explorerList.FileClick += File_click;
            _explorerList.FilePreview += FilePreview;


        }

        
        
        public async void Directory_click(ExplorerListViewModel item)
        {   List<ExplorerListViewModel> listView=new List<ExplorerListViewModel>();
            _fileExplorer.Path = item.FullPath;
            if (!_fileExplorer.IsStartDirectory)
            {
                listView.Add(_fileExplorer.GetPreviousDirectory());
            }
            
            listView.AddRange(await _fileExplorer.GetDirectoryAsync());
            listView.AddRange(await _fileExplorer.GetFileAsync());
            
            var adapter = new ExplorerViewAdapter(this, listView.ToArray());
            _explorerList.SetCurrentAdapter(adapter);
        }

        public void FilePreview(ExplorerListViewModel item,bool play)
        {
            if (_player==null)
            {
                _player = new Player(item.FullPath);
            }
            if (!_player.Path.Equals(item.FullPath))
            {
                _player.SetPath(item.FullPath);
            }
            if (play)
            {
                _player.Play();
                Toast.MakeText(this, "Podgląd aktywny", ToastLength.Short).Show();
            }
            else
            {
                _player.Stop();
                Toast.MakeText(this, "Podgląd nieaktywny", ToastLength.Short).Show();
            }
           
            
        }

        


        public void File_click(ExplorerListViewModel item)
        {
            Intent playerActivity = new Intent(this, typeof(PlayerActivity));
            var itemJson = JsonConvert.SerializeObject(item);
            playerActivity.PutExtra("File", itemJson);
            StartActivity(playerActivity);

        }



    }
}
