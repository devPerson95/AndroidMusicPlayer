using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Object = Java.Lang.Object;

namespace AndroidMusicPlayer
{
   public class ExplorerViewAdapter:BaseAdapter
    {
        private List<ExplorerListViewModel> _explorerList;
        private Context _context;
        private Animation _animation;
        public bool RunAnimation;
        public ExplorerViewAdapter(Context context, ExplorerListViewModel[] explorerList, Animation animation)
        {
            _explorerList = explorerList.ToList();
            _context = context;
            _animation = animation;
           

        }
        public void UpdateAdapter(ExplorerListViewModel[] explorerList)
        {
            _explorerList = explorerList.ToList();
          this.NotifyDataSetChanged();
        }
        public void UpdateAdapter(List<ExplorerListViewModel> explorerList)
        {
            _explorerList = explorerList;
            this.NotifyDataSetChanged();
        }
        public ExplorerViewAdapter(IEnumerable<ExplorerListViewModel> fileList)
        {
            _explorerList = fileList.ToList();
        }
        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _explorerList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ??
                       LayoutInflater.From(_context).Inflate(Resource.Layout.FileListView,null, false);
            var textView = view.FindViewById<TextView>(Resource.Id.ContentText);
            textView.Text = _explorerList[position].Name;
            var imageView = view.FindViewById<ImageView>(Resource.Id.ItemImage);
            if (_explorerList[position].IsFile)
            {
                imageView.SetImageResource(Resource.Drawable.soundFileIcon);
            }
            else
            {
                imageView.SetImageResource(Resource.Drawable.folderIcon);
            }

            if (RunAnimation)
            {
                if (_animation!=null)
                {
                    view.StartAnimation(_animation);
                }
                
            }
            else
            {
                view.Animation?.Cancel();
            }

            return view;
        }

        public override bool IsEnabled(int position)
        {
            return !RunAnimation;
        }

        public ExplorerListViewModel GetItemFromId(long id)
        {
            return _explorerList.FirstOrDefault(m => m.Id == id);
        }
        public override int Count { get { return _explorerList.Count; } }
    }
}