using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Animation;
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
using Javax.Security.Auth;
using Exception = System.Exception;
using Orientation = Android.Widget.Orientation;
using String = System.String;

namespace AndroidMusicPlayer
{
    [Activity(Label = "AndroidMusicPlayer", MainLauncher = true, Icon = "@drawable/icon",
         Theme = "@android:style/Theme.Black.NoTitleBar")]
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
        private Button _changeSource;
        private Button _newDirectory;
        private bool _isExternal;

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
            if (_player != null)
            {
                _player.Dispose();
            }

        }

        public string GetInternalStorage()
        {
            var storages = GetExternalFilesDirs(null);
            var internalStorage = storages[0];
            var directories = internalStorage.AbsolutePath.Split('/');
            int index = 1;
            string path = String.Empty;
            while (!directories[index].Equals("Android"))
            {
                path = Path.Combine(path, directories[index]);
                index++;
            }
            return path;

        }

        public string GetExternalStorage()
        {
            var storages = GetExternalFilesDirs(null);
            if (storages != null && storages.Count() > 1)
            {
                var externalStorage = storages[1];
                var directories = externalStorage.AbsolutePath.Split('/');
                int index = 1;
                string path = String.Empty;
                while (!directories[index].Equals("Android"))
                {
                    path = Path.Combine(path, directories[index]);
                    index++;
                }

                return path;
            }
            else
            {
                throw new System.Exception();
            }
        }

        protected void InternalStorageButton_Click(object sender, EventArgs e)
        {
           OpenInternalStorage();
            _isExternal = false;
        }

        protected void ExternalStorageButton_Click(object sender, EventArgs e)
        {
           
            OpenExternalStorage();
            _isExternal = true;

        }

        private void OpenInternalStorage()
        {
            var startPath = GetInternalStorage();
            _fileExplorer.Path = startPath;
            CallbackFileExplorer();
            _progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleSmall);
            _layout.AddView(_progressBar);
            var menuLayout = LayoutInflater.From(this).Inflate(Resource.Layout.ExplorerMenu, null, false);
            _layout.AddView(menuLayout);
            if (_changeSource==null)
            {
                _changeSource = FindViewById<Button>(Resource.Id.changeSourceBtn);
                _changeSource.Click += ChangeSourceBtn_Click;
            }
           
            _changeSource.SetText("Pamięć zewnętrzna", TextView.BufferType.Normal);
            
        }
        private void OpenExternalStorage()
        {
            try
            {
                var startPath = GetExternalStorage();
                _fileExplorer.Path = startPath;
                CallbackFileExplorer();
                _progressBar = new ProgressBar(this, null, Android.Resource.Attribute.ProgressBarStyleSmall);
                _layout.AddView(_progressBar);
                var menuLayout = LayoutInflater.From(this).Inflate(Resource.Layout.ExplorerMenu, null, false);
                _layout.AddView(menuLayout);
                if (_changeSource == null)
                {
                    _changeSource = FindViewById<Button>(Resource.Id.changeSourceBtn);
                    _changeSource.Click += ChangeSourceBtn_Click;
                }
                _changeSource.SetText("Pamięć wewnętrzna", TextView.BufferType.Normal);
                
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Urządzenie nie posiada pamięci zewnętrznej", ToastLength.Long).Show();
                if (_externalStorageBtn!=null)
                {
                    _externalStorageBtn.Click -= ExternalStorageButton_Click;
                    _layout.RemoveView(_externalStorageBtn);
                }
                
            }
        }
        public async  void CallbackFileExplorer()
        {

            var itemFile = _fileExplorer.GetFileAsync();
            var items = await _fileExplorer.GetDirectoryAsync();
            items.AddRange(await itemFile);
            if (_externalStorageBtn!=null && _internalStorageBtn!=null)
            {
                _internalStorageBtn.Click -= InternalStorageButton_Click;
                _externalStorageBtn.Click -= ExternalStorageButton_Click;
                _layout.RemoveView(_internalStorageBtn);
                _internalStorageBtn.Dispose();
                _layout.RemoveView(_externalStorageBtn);
                _externalStorageBtn.Dispose();
            }
           
            _layout.RemoveView(_progressBar);
            _progressBar.Dispose();
            _layout.AddView(_listView, new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, _layout.Height-200));
            _adapter = new ExplorerViewAdapter(this,  items.ToArray());
            _explorerList.SetCurrentAdapter(_adapter);
            _explorerList.DirectoryClick += Directory_click;
            _explorerList.FileClick += File_click;
            _explorerList.FilePreview += FilePreview;
            _newDirectory = FindViewById<Button>(Resource.Id.newDirectoryBtn);
            _newDirectory.Click += NewDirectoryBtn_Click;
           
        }

        public void ChangeSourceBtn_Click(object sender, EventArgs e)
        {
            
        }
        public void NewDirectoryBtn_Click(object sender, EventArgs e)
        {
            var alertBox = new AlertDialog.Builder(this);
            alertBox.SetTitle("Utwórz nowy folder");
            alertBox.SetMessage("Podaj nazwę folderu");
            var textBox=new EditText(this) {Text = "Nowy folder"};
            alertBox.SetView(textBox);
            alertBox.SetPositiveButton("Utwórz", (object senderAlert, DialogClickEventArgs eve) =>
            {
                var ioHandler=new IOHandler();
                ioHandler.AddDirectory(Path.Combine(_fileExplorer.Path,textBox.Text));
              Toast.MakeText(this, textBox.Text,ToastLength.Short).Show();
            });
            alertBox.SetNegativeButton("Anuluj", (object senderAlert, DialogClickEventArgs eve) => { });
            var dialog = alertBox.Create();
            dialog.Show();


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
            
            _explorerList.UpdateList(listView);
            
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
