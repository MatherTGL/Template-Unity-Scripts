using System.Threading.Tasks;

namespace GameAssets.Meta.Items.Interfaces
{
    public interface IPreloadable
    {
        public Task AsyncPreload();
    }
}