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
using Java.Lang;
using Java.Util;

namespace AndroidMusicPlayer
{
    public class ExplorerListView
    {
        private ListView _listView;
        private ExplorerViewAdapter _currentAdapter;
        private FileExplorer _fileExplorer;
        private ExplorerListViewModel _clickedExplorer;
        public delegate void ItemDelegate(ExplorerListViewModel item);

        public delegate void FilePreviewDelegate(ExplorerListViewModel item, bool startPreview);
        public event ItemDelegate FileClick;
        public event ItemDelegate DirectoryClick;
        public event FilePreviewDelegate FilePreview;
        

        public ExplorerListView(ListView listView,FileExplorer fileExplorer)
        {
            
            _listView = listView;
            _fileExplorer = fileExplorer;
            _listView.ItemClick += Item_Click;
            _listView.ItemLongClick += Long_click;
        }

        public void Long_click(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var itemId = _currentAdapter.GetItemId(e.Position);
            var item = _currentAdapter.GetItemFromId(itemId);
            if (item.IsFile)
            {

                _clickedExplorer = item;
                _listView.Touch += List_touch;
                FilePreview?.Invoke(_clickedExplorer, true);
                
            }
        }

        public void List_touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
                { 
                _listView.Touch -= List_touch;
                if (_clickedExplorer!=null)
                {
                    FilePreview?.Invoke(_clickedExplorer,false);
                }
                
            }

        }
        

        public void Item_Click(object sender, AdapterView.ItemClickEventArgs e)
        {
            var itemId = _currentAdapter.GetItemId(e.Position);
            if (itemId==0)
            {
                _fileExplorer.RemoveLastFromHistory();
            }
            var item = _currentAdapter.GetItemFromId(itemId);
            if (item.IsFile)
            {
              
                FileClick?.Invoke(item);
            }
            else
            {
                DirectoryClick?.Invoke(item);
                
            }

        }

        public void SetCurrentAdapter(ExplorerViewAdapter adapter)
        {
            _listView.Adapter = adapter;
            _currentAdapter = adapter;
        }

      


        
    }
}