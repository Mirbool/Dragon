using System;
using System.Threading.Tasks;

namespace Koakuma.Game.Scenes
{
    public struct CreateSceneData
    {
        public bool showLoading;
        public Func<Task> loadingTask;
    }
}
