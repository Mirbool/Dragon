using System.Threading.Tasks;

namespace TGame.Save
{
    public abstract class BaseTask
    {
        public string Path { get; set; }

        public abstract Task Do();
    }
}
