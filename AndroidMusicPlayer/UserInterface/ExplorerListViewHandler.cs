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
    public class ExplorerListViewHandler
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
        float previousX = 0;
        float previousY = 0;

        public ExplorerListViewHandler(ListView listView,FileExplorer fileExplorer)
        {
            
            _listView = listView;
            _fileExplorer = fileExplorer;
            _listView.ItemClick += Item_Click;
            _listView.ItemLongClick += Long_click;
           

        }

       
        private void Long_click(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var itemId = _currentAdapter.GetItemId(e.Position);
            var item = _currentAdapter.GetItemFromId(itemId);
            if (item.IsFile)
            {
                _listView.Touch += List_touch;
                _clickedExplorer = item;
               
                FilePreview?.Invoke(_clickedExplorer, true);
                
            }
        }
       

        private void List_touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                _listView.Touch -= List_touch;
                if (_clickedExplorer != null)
                {
                    FilePreview?.Invoke(_clickedExplorer, false);
                }

            }
            float startX = e.Event.RawX;
            float startY = e.Event.RawY;
            
            if (previousX-startX>100 && previousY-startY<100)
            {
                previousY = 1;
            }
            previousX = startX;
            previousY = startY;

        }
        

        private void Item_Click(object sender, AdapterView.ItemClickEventArgs e)
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

        public void Update(List<ExplorerListViewModel> models)
        {
            if (_currentAdapter == null)
            {
               
                _currentAdapter = new ExplorerViewAdapter(models);
                _listView.Adapter = _currentAdapter;
            }
            else
            {
               _currentAdapter.UpdateAdapter(models);
            }
        }

      


        
    }
}