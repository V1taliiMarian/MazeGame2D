using System;
using System.Threading.Tasks;
using System.Windows;

namespace LabyrinthGame
{
    public static class LoadingService
    {
        public static async Task ShowLoadingAsync(Type targetPageType, Window owner)
        {
            var loadingWindow = new LoadingWindow(targetPageType)
            {
                Owner = owner
            };

            loadingWindow.Show();

            await Task.Delay(2000); 

            loadingWindow.Close();
        }
    }
}