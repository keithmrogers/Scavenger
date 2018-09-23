
using System;
using Android.Media;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundService))]
namespace Scavenger.XForms.Droid.Services
{
    public class SoundService : ISoundService
    {
        public void PlaySound(string fileName)
        {
            var player = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Prepare();
        }
    }
}