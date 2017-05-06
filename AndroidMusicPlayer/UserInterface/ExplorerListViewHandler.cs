using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidMusicPlayer.Manager;
using Java.Lang;
using Java.Util;
using Exception = System.Exception;

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
        private bool _isItemMoved;
      

        public Context Context { get; set; }

        public ExplorerListViewHandler(ListView listView, FileExplorer fileExplorer)
        {

            _listView = listView;
            _fileExplorer = fileExplorer;
            _listView.ItemClick += Item_Click;
            _listView.Touch += List_touch;

        }


        private void Long_click(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var itemId = _currentAdapter.GetItemId(e.Position);
            var item = _currentAdapter.GetItemFromId(itemId);
            _clickedExplorer = item;
           
            _initialTouch = true;
            itemLongClickPos = e.Position;
            if (item.IsFile)
            {

                FilePreview?.Invoke(_clickedExplorer, true);

            }

        }


        private void List_touch(object sender, View.TouchEventArgs e)
        {
            var child = _listView.GetChildAt(0);
            var height = child.Height;
            var scroll = -child.Top + _listView.FirstVisiblePosition*height;
            var point = _listView.PointToPosition((int)e.Event.GetX(),(int) e.Event.GetY());
            var  itemView = _listView.GetChildAt(point-scroll/height);
           
            if (e.Event.Action == MotionEventActions.Down)
            {
                startX = e.Event.RawX;
                startY = e.Event.RawY;
                _isSlideInvoke = false;
                _isItemMoved = false;
            }
            

            if (e.Event.Action == MotionEventActions.Up)
            {
                
                if (itemView!=null)
                {
                    itemView.SetPadding(0, 0, 0, 0);
                }
                
                if (_clickedExplorer != null && _clickedExplorer.IsFile)
                {
                    
                  //  FilePreview?.Invoke(item, false);

                   
                }
                
                
                
            }
            
            if (e.Event.Action == MotionEventActions.Move )
            {
                
               
                var currentX = e.Event.RawX;
                var currentY = e.Event.RawY;
                var slide = currentX - startX;
                _isItemMoved = true;

                if (currentY - startY < 50 && startY - currentY < 50)
                {
                    if (itemView != null)
                    {
                        itemView.SetPadding((int)slide, 0, 0, 0);
                    }
                    if (currentX - startX > 400)
                    {

                        if (!_isSlideInvoke)
                        {
                            _isSlideInvoke = true;
                            var id = _currentAdapter.GetItemId(point);
                            var item = _currentAdapter.GetItemFromId(id);

                            ItemSlideRight?.Invoke(item);

                        }

                    }
                    
                }
                else
                {
                    if (itemView != null)
                    {
                        itemView.SetPadding(0, 0, 0, 0);
                    }
                }

                e.Handled = true;


            }


            e.Handled = false;



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