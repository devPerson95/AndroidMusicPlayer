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
using System.IO;

namespace AndroidMusicPlayer
{
    public class StorageHandler
    {
        public void AddDirectory(string path)
        {
            string directorPath = path;
            DirectoryInfo directoryInfo = new DirectoryInfo(directorPath);
            int index = 0;
               

            while (directoryInfo.Exists)
            {
                    index++;
                    directoryInfo = new DirectoryInfo(directorPath+index);
             }
            if (index!=0)
            {
                directorPath += index;
            }
            
            var dir = new Java.IO.File(directorPath);
            dir.Mkdir();

        }

        public void DeleteItem(string path)
        {
            
            var item=new Java.IO.File(path);
            if (item.Exists() && item.CanExecute())
            {
                item.Delete();
            }
            else
            {
                throw new Exception();
            }
           
        }

    }
}