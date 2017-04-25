using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using String = System.String;

namespace AndroidMusicPlayer.Manager
{
   public class FileExplorer
   {
       private List<string> _pathHistory;
       private string _path;
       private int _id;
       public bool IsStartDirectory;

       public string Path
       {
           get { return _path; }
           set
           {
               if (!value.Equals(_pathHistory.LastOrDefault()))
               {
                   
                    _pathHistory.Add(value);
                    if (_pathHistory.Count > 1)
                    {
                        IsStartDirectory = false;
                    }
                }
               
               _path = value;
               _id = 1;
           }
       }
        public FileExplorer(string startPath)
        {
          _pathHistory=new List<string>();
            _path = startPath;
            _pathHistory.Add(startPath);
            IsStartDirectory = true;
           
            _id = 0;
        }

       public FileExplorer()
       {
            _pathHistory = new List<string>();
            IsStartDirectory = true;

            _id = 0;
        }

       public Task<List<ExplorerListViewModel>> GetDirectoryAsync()
       {
           return Task.Factory.StartNew(() => GetDirectory());
       }
        public Task<List<ExplorerListViewModel>> GetFileAsync()
        {
            return Task.Factory.StartNew(() => GetFileFrom());
        }

       public void RemoveLastFromHistory()
       {
           _pathHistory.RemoveAt(_pathHistory.Count-1);
           if (_pathHistory.Count==1)
           {
               IsStartDirectory = true;
           }
       }

       public List<string> GetPathHistory()
       {
           return _pathHistory;
       }
       public ExplorerListViewModel GetPreviousDirectory()
       {
           var prevoiusFullPath = _pathHistory[_pathHistory.Count - 2];
           if (prevoiusFullPath!=String.Empty)
           {
               
                var item = new ExplorerListViewModel
                {
                    FullPath = prevoiusFullPath,
                    Id = 0,
                    IsFile = false,
                    Name = "..."

                };
               return item;
           }
           else
           {
               return null;
           }
           
       }

        public List<ExplorerListViewModel> GetDirectory()
       {
          
            var directoryInfo=new DirectoryInfo(_path);
           if (directoryInfo.Exists)
           {

               var directories = directoryInfo.EnumerateDirectories();
                var items=new List<ExplorerListViewModel>();
               foreach (var director in directories)
               {
                   items.Add(new ExplorerListViewModel
                   {
                       FullPath = director.FullName,
                       Name = director.Name,
                       Id = _id,
                       IsFile = false

                   });
                   _id++;
               }

               return items;
           }
           else
           {
               throw new DirectoryNotFoundException();
           }
       }

       public List<ExplorerListViewModel> GetFileFrom()
       {
           var directoryInfo=new DirectoryInfo(_path);
           if (directoryInfo.Exists)
           {
               
                var items =new List<ExplorerListViewModel>();
               var files = directoryInfo.EnumerateFiles();
               foreach (var file in files)
               {

                   items.Add(new ExplorerListViewModel
                   {
                       FullPath = file.FullName,
                       Name = file.Name,
                       Id = _id,
                       IsFile = true
                   });
                   _id++;
               }

               return items;
           }
           else
           {
               throw new DirectoryNotFoundException();
           }
       }

     

    }
}