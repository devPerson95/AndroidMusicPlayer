using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace AndroidMusicPlayer
{
   public class FileViewAdapter:BaseAdapter
    {
        private List<FileListViewModel> _fileList;
        private Context _context;

        public FileViewAdapter(Context context, FileListViewModel[] fileList)
        {
            _fileList = fileList.ToList();
            _context = context;
        }

        public FileViewAdapter(IEnumerable<FileListViewModel> fileList)
        {
            _fileList = fileList.ToList();
        }
        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _fileList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ??
                       LayoutInflater.From(_context).Inflate(Resource.Layout.FileListView, null, false);
            var textView = view.FindViewById<TextView>(Resource.Id.ContentText);
            textView.Text = _fileList[position].Name;
            var imageView = view.FindViewById<ImageView>(Resource.Id.ItemImage);
            if (_fileList[position].IsFile)
            {
                imageView.SetImageResource(Resource.Drawable.soundFileIcon);
            }
            else
            {
                imageView.SetImageResource(Resource.Drawable.folderIcon);
            }
            
            return view;
        }

        public FileListViewModel GetItemWithId(long id)
        {
            return _fileList.FirstOrDefault(m => m.Id == id);
        }
        public override int Count { get { return _fileList.Count; } }
    }
}