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
        public event ItemDelegate ItemSlideRight;

        private float startX = 0;
        private float startY = 0;
        private bool _initialTouch;
        private bool _isSlideInvoke;


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
            _clickedExplorer = item;
            _listView.Touch += List_touch;
            _initialTouch = true;
            if (item.IsFile)
            {

                FilePreview?.Invoke(_clickedExplorer, true);
                
            }

        }
       

        private void List_touch(object sender, View.TouchEventArgs e)
        {
            if (_initialTouch)
            {
               startX = e.Event.RawX;
               startY = e.Event.RawY;
            }
            if (e.Event.Action == MotionEventActions.Up)
            {
               
                if (_clickedExplorer != null && _clickedExplorer.IsFile)
                {
                    FilePreview?.Invoke(_clickedExplorer, false);
                   
                }
                _listView.Touch -= List_touch;
                _isSlideInvoke = false;
             

            }
            _initialTouch = false;
            var currentX= e.Event.RawX;
            var currentY = e.Event.RawY;

            if (currentX-startX>100 && currentY-startY<100)
            {
                if (!_isSlideInvoke)
                {
                    _isSlideInvoke = true;
                    ItemSlideRight?.Invoke(_clickedExplorer);
                    
                }

            }
           
           
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