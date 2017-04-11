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
using AndroidMusicPlayer.Manager;
using Java.Util;

namespace AndroidMusicPlayer
{
    public class ExplorerListView
    {
        private Context _context;
        private ListView _listView;
        private FileViewAdapter _curentAdapter;
        private FileExplorer _fileExplorer;

        public delegate void ItemClickDelegate(FileListViewModel item);

        public event ItemClickDelegate FileClick;
        public event ItemClickDelegate DirectoryClick;

        public ExplorerListView(Context context, ListView listView,FileExplorer fileExplorer)
        {
            _context = context;
            _listView = listView;
            _fileExplorer = fileExplorer;
            _listView.ItemClick += Item_Click;

        }

        public void Item_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            var itemId = _curentAdapter.GetItemId(e.Position);
            if (itemId==0)
            {
                _fileExplorer.RemoveLastFromHistory();
            }
            var item = GetSelectedItem(itemId);
            if (item.IsFile)
            {
                FileClick?.Invoke(item);
            }
            else
            {
                DirectoryClick?.Invoke(item);
                
            }

        }

        public void SetCurrentAdapter(FileViewAdapter adapter)
        {
            _listView.Adapter = adapter;
            _curentAdapter = adapter;
        }

        public FileListViewModel GetSelectedItem(long selectedId)
        {
            return _curentAdapter.GetItemWithId(selectedId);
        }

        
        

        
    }
}