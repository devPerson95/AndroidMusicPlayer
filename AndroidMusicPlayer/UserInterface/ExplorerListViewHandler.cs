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
using Android.OS;
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

        public delegate void ItemDelegate(ExplorerListViewModel item);

        public event ItemDelegate FileClick;
        public event ItemDelegate DirectoryClick;
        public event ItemDelegate ItemSlideRight;

        private float _startX = 0;
        private float _startY = 0;
        private bool _isSlideInvoke;
       

        public Context Context { get; set; }

        public ExplorerListViewHandler(ListView listView, FileExplorer fileExplorer)
        {
            _listView = listView;
            _fileExplorer = fileExplorer;
            _listView.ItemClick += Item_Click;
        }
        private void List_touch(object sender, View.TouchEventArgs e)
        {
            var child = _listView.GetChildAt(0);
            var height = child.Height;
            var scroll = -child.Top + _listView.FirstVisiblePosition*height;
            var point = _listView.PointToPosition((int)e.Event.GetX(),(int) e.Event.GetY());
            var  itemView = _listView.GetChildAt(point-scroll/height);
            var upTime = Android.OS.SystemClock.UptimeMillis();
            var downTime = upTime - e.Event.DownTime;
            if (e.Event.Action == MotionEventActions.Down)
            {
               
                _startX = e.Event.RawX;
                _startY = e.Event.RawY;
                _isSlideInvoke = false;
            }
            

            if (e.Event.Action == MotionEventActions.Up)
            {
                itemView?.SetPadding(0, 0, 0, 0);
            }
            
            if (e.Event.Action == MotionEventActions.Move && downTime>200 )
            {
                
                var currentX = e.Event.RawX;
                var currentY = e.Event.RawY;
                var slide = currentX - _startX;

                if (currentY - _startY < 50 && _startY - currentY < 50)
                {
                    
                        itemView?.SetPadding((int)slide, 0, 0, 0);
                    
                    if (currentX - _startX > 400)
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
                     itemView?.SetPadding(0, 0, 0, 0);
                }

                
              
            }
            e.Handled = false;
        }
        private void Item_Click(object sender, AdapterView.ItemClickEventArgs e)
        {

            var itemId = _currentAdapter.GetItemId(e.Position);
            if (itemId == 0)
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

        
        public void EditModeEnabled()
        {
            _currentAdapter.RunAnimation = true;
            _currentAdapter.NotifyDataSetChanged();
            _listView.Touch += List_touch;
            _listView.ItemClick -= Item_Click;
        }

        public void EditModeDisabled()
        {
            _currentAdapter.RunAnimation = false;
            _currentAdapter.NotifyDataSetChanged();
            _listView.Touch -= List_touch;
            _listView.ItemClick += Item_Click;
        }
    }
}