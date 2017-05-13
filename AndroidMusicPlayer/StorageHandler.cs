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
using System.IO.IsolatedStorage;
using Android;
using Android.Support.V4.Provider;
using Java.IO;
using Uri = Android.Net.Uri;

namespace AndroidMusicPlayer
{
    public class StorageHandler
    {
        public bool AddDirectoryFromPath(string path)
        {

            string directorPath = path;
            DirectoryInfo directoryInfo = new DirectoryInfo(directorPath);
            int index = 0;


            while (directoryInfo.Exists)
            {
                index++;
                directoryInfo = new DirectoryInfo(directorPath + index);
            }
            if (index != 0)
            {
                directorPath += String.Format(" ({0})",index);
            }

            var dir = new Java.IO.File(directorPath);

            var status = dir.Mkdirs();
            return status;
        }

        public void AddDirectoryFromUri(DocumentFile documentFile,string directoryName)
        {
           
            var name = directoryName;
            var duplicate=documentFile.FindFile(directoryName);
            int index = 0;
            while (duplicate!=null)
            {
                index++;
                duplicate = documentFile.FindFile(name + index);
            }
            if (index == 0)
            {
                name = directoryName;
            }
            documentFile.CreateDirectory(name);
        }

        public bool DeleteItemFromPath(string path)
        {
            
            var item=new Java.IO.File(path);
            if (item.Exists())
            {
               var status = item.Delete();
                return status;
            }
            else
            {
                throw new Exception("Obiekt nie istnieje");
            }
           
        }

        public void DeleteItemFromUri(DocumentFile documentFile, string name)
        {
            var itemToDelete = documentFile.FindFile(name);
            itemToDelete?.Delete();
        }

    }
}