using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using AndroidMusicPlayer.Manager;
using Java.IO;
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
        private ExplorerListViewHandler _explorerListHandler;
        private FileExplorer _fileExplorer;
        private ExplorerViewAdapter _adapter;
        private LinearLayout _layout;
        private Player _player;
        private Button _changeSource;
        private Button _newDirectory;
        private Button _editMode;
        private bool _isExternal;
        private string _currentPath;
        private bool _isEditMode;
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
            _fileExplorer = new FileExplorer();
            CheckPermission();
           

        }

        public void CheckPermission()
        {

            var permision = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            if (permision != Permission.Granted)
            {
                RequestPermissions(new string[] {Manifest.Permission_group.Storage}, 0);
            }
        }

        private void Dispose()
        {
           
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
            try
            {

                var startPath = GetInternalStorage();
                RemoveStartMenu();
                CreateExplorerMenu("Pamięć zewnętrzna");
                CreateListView();
                Refresh(startPath);

            }
            catch (Exception exception)
            {
                Toast.MakeText(this, "Wystąpił nie oczekiwany błąd", ToastLength.Long).Show();


            }
        }

        public void ItemSlideRight(ExplorerListViewModel item)
        {

            var alertBox = new AlertDialog.Builder(this)
                .SetTitle("Usuń")
                .SetMessage(String.Format("Czy chcesz usunąć {0}?", item.Name))
                .SetPositiveButton("Usuń", (object sender, DialogClickEventArgs e) =>
                {
                    var ioHandler = new StorageHandler();
                    ioHandler.DeleteItem(item.FullPath);
                    Toast.MakeText(this, string.Format("Usunięto {0}!", item.Name), ToastLength.Short).Show();
                    Refresh();
                })
                .SetNegativeButton("Anuluj", (object sender, DialogClickEventArgs e) =>
                {

                });
            var dialog = alertBox.Create();
            dialog.Show();
        }

        protected void ExternalStorageButton_Click(object sender, EventArgs e)
        {

            try
            {

                var startPath = GetExternalStorage();
                RemoveStartMenu();
                CreateExplorerMenu("Pamięć wewnętrzna");
                CreateListView();
                Refresh(startPath);
                _isExternal = true;
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Urządzenie nie posiada pamięci zewnętrznej", ToastLength.Long).Show();
                if (_externalStorageBtn != null)
                {
                    _externalStorageBtn.Click -= ExternalStorageButton_Click;
                    _layout.RemoveView(_externalStorageBtn);
                }

            }

        }



        public void CreateExplorerMenu(string sourceBtnText)
        {
            var menuLayout = LayoutInflater.From(this).Inflate(Resource.Layout.ExplorerMenu, null, false);
            _layout.AddView(menuLayout);
            _changeSource = FindViewById<Button>(Resource.Id.changeSourceBtn);
            _changeSource.Click += ChangeSourceBtn_Click;
            _changeSource.SetText(sourceBtnText, TextView.BufferType.Normal);
            _newDirectory = FindViewById<Button>(Resource.Id.newDirectoryBtn);
            _newDirectory.Click += NewDirectoryBtn_Click;
            _editMode = FindViewById<Button>(Resource.Id.editModeBtn);
            _editMode.Click += EditModeBtn_click;

        }

        public void EditModeBtn_click(object sender, EventArgs e)
        {
            if (!_isEditMode)
            {
                _explorerListHandler.EditModeEnabled();
                _isEditMode = true;
            }
            else
            {
                _explorerListHandler.EditModeDisabled();
                _isEditMode = false;
            }
            
        }
        public void CreateListView()
        {
            _listView = new ListView(this, null, Android.Resource.Layout.SimpleListItem1);
            _layout.AddView(_listView,
                new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, _layout.Height - 200));
            _explorerListHandler = new ExplorerListViewHandler(_listView, _fileExplorer);
            _explorerListHandler.Context = this;
            _explorerListHandler.DirectoryClick += Directory_click;
            _explorerListHandler.FileClick += File_click;
            _explorerListHandler.ItemSlideRight += ItemSlideRight;
        }

        public void RemoveStartMenu()
        {
            _internalStorageBtn.Click -= InternalStorageButton_Click;
            _externalStorageBtn.Click -= ExternalStorageButton_Click;
            _layout.RemoveView(_internalStorageBtn);
            _internalStorageBtn.Dispose();
            _layout.RemoveView(_externalStorageBtn);
            _externalStorageBtn.Dispose();
        }

        public void ChangeSourceBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string path;
                if (_isExternal)
                {
                    path = GetInternalStorage();
                    _changeSource.Text = "Pamięć zewnętrzna";
                    _isExternal = false;
                }
                else
                {
                    path = GetExternalStorage();
                    _changeSource.Text = "Pamięć wewnętrzna";
                    _isExternal = true;
                }
                _fileExplorer.SetAnotherStarPath(path);
                Refresh(path);
            }
            catch (Exception)
            {

                _changeSource.Enabled = false;
            }

        }

        public void NewDirectoryBtn_Click(object sender, EventArgs e)
        {

            var alertBox = new AlertDialog.Builder(this);
            alertBox.SetTitle("Utwórz nowy folder");
            alertBox.SetMessage("Podaj nazwę folderu");
            var textBox = new EditText(this) {Text = "Nowy folder"};
            alertBox.SetView(textBox);
            alertBox.SetPositiveButton("Utwórz", (object senderAlert, DialogClickEventArgs eve) =>
            {
                try
                {
                    var ioHandler = new StorageHandler();
                   ioHandler.AddDirectory(Path.Combine(_fileExplorer.Path, textBox.Text));
                    Toast.MakeText(this, textBox.Text, ToastLength.Short).Show();
                    Refresh();
                }
                catch (Exception)
                {

                    Toast.MakeText(this, "Nie można utworzyć folderu w tej lokalizacji!", ToastLength.Short).Show();
                }
            });
                alertBox.SetNegativeButton("Anuluj", (object senderAlert, DialogClickEventArgs eve) => { });
                var dialog = alertBox.Create();
                dialog.Show();
            
            
           


        }

        public async void Refresh(string path)
        {
            _fileExplorer.SetPath(path);
            List<ExplorerListViewModel> listView = new List<ExplorerListViewModel>();

            if (!_fileExplorer.IsStartDirectory)
            {
                listView.Add(_fileExplorer.GetPreviousDirectory());
            }
           

            listView.AddRange(await _fileExplorer.GetDirectoryAsync());
            listView.AddRange(await _fileExplorer.GetFileAsync());
            if (_adapter==null)
            {
                _adapter=new ExplorerViewAdapter(this,listView.ToArray());
                _explorerListHandler.SetCurrentAdapter(_adapter);
            }

            _explorerListHandler.Update(listView);
        }
        public async void Refresh()
        {
            
            List<ExplorerListViewModel> listView = new List<ExplorerListViewModel>();

            if (!_fileExplorer.IsStartDirectory)
            {
                listView.Add(_fileExplorer.GetPreviousDirectory());
            }


            listView.AddRange(await _fileExplorer.GetDirectoryAsync());
            listView.AddRange(await _fileExplorer.GetFileAsync());
            if (_adapter == null)
            {
                _adapter = new ExplorerViewAdapter(this, listView.ToArray());
                _explorerListHandler.SetCurrentAdapter(_adapter);
            }

            _explorerListHandler.Update(listView);
        }

        public  void Directory_click(ExplorerListViewModel item)
        {   
            _currentPath = item.FullPath;
            Refresh(_currentPath);

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
